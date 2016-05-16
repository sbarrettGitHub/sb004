using System.Threading.Tasks;

namespace SB004.Business
{
	using System;
	using System.Collections.Generic;
	using System.Linq;

	using SB004.Data;
	using SB004.Domain;
	using SB004.Utilities;

	public class MemeBusiness : IMemeBusiness
	{
		readonly IRepository repository;
		readonly IImageManager imageManager;
		readonly IHashTagBusiness hashTagBusiness;
		#region constants

		private readonly DateTime baseDate = new DateTime(2015, 1, 1);
		private const double ScorePerDay = 10;
		private const double ScorePerView = 0.07;
		private const double ScorePerLike = 1;
		private const double ScorePerDislike = -0.5;
		private const double ScorePerComment = 0.5;
		private const double ScorePerReply = 2;
		private const double ScorePerRepost = 3;
		#endregion
		public MemeBusiness(IRepository repository, IImageManager imageManager, IHashTagBusiness hashTagBusiness)
		{
			this.repository = repository;
			this.imageManager = imageManager;
			this.hashTagBusiness = hashTagBusiness;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="userId"></param>
		/// <param name="likesIncrement"></param>
		/// <param name="dislikesIncrement"></param>
		/// <param name="viewsIncrement"></param>
		/// <param name="sharesIncrement"></param>
		/// <param name="favouritesIncrement"></param>
		/// <param name="memeId"></param>
		/// <returns></returns>
		public IMeme UpdateMemeInteraction(string memeId, string userId, int likesIncrement, int dislikesIncrement, int viewsIncrement, int sharesIncrement, int favouritesIncrement)
		{
			IMeme meme = repository.GetMeme(memeId);
			if (meme == null)
			{
				throw new Exception("Object not found");
			}
			meme.Likes += likesIncrement;
			meme.Dislikes += dislikesIncrement;
			meme.Views += viewsIncrement;
			meme.Shares += sharesIncrement;
			meme.Favourites += favouritesIncrement;

			// Save meme
			meme = SaveMeme(meme);

			if (userId != null)
			{
				// Add like to time line
				if (likesIncrement > 0)
				{
					repository.Save(new TimeLine(userId, TimeLineEntry.Like, meme.Id, null, null, null));
				}
				// Add dislike to time line
				if (dislikesIncrement > 0)
				{
					repository.Save(new TimeLine(userId, TimeLineEntry.Dislike, meme.Id, null, null, null));
				}

				IUser user = repository.GetUser(userId);
				if (user != null)
				{
					user.Likes += likesIncrement;
					user.Dislikes += dislikesIncrement;
					user.Views += viewsIncrement;
					user.Shares += sharesIncrement;
					user.Favourites += favouritesIncrement;

					// Save meme creator
					repository.Save(user);
				}
			}

			// What if this is a repost?
			if (meme.RepostOfId != null)
			{
				IMeme original = repository.GetMeme(meme.RepostOfId);
				if (original != null)
				{
					// A repost should always reference an original (not another repost)
					if (original.RepostOfId != null)
					{
						throw new Exception(string.Format("Repost of a repost not allowed - {0} - {1}", meme.Id, meme.RepostOfId));
					}

					// Recursive call of this operation to update the original
					UpdateMemeInteraction(meme.RepostOfId, null, likesIncrement, dislikesIncrement, viewsIncrement, sharesIncrement, favouritesIncrement);

				}
			}
			// Update the meme
			return meme;
		}
		/// <summary>
		/// Calculate the trend score and save the meme
		/// </summary>
		/// <param name="meme"></param>
		/// <returns></returns>
		public IMeme SaveMeme(IMeme meme)
		{
			bool isNew = false;
			// Get the number of user comments
			if (meme.Id != null)
			{
				meme.UserCommentCount = repository.GetUserCommentCount(meme.Id);
			}
			else
			{
				isNew = true;
			}

			// Meme is top leve if it has no reposnseToId
			meme.IsTopLevel = meme.ResponseToId == null;

			// Fall back on adding the text manually to the seed if mem image not supplied by client
			if (meme.ImageData == null || meme.ImageData.Length == 0)
			{
				// Get the seed image
				byte[] seedData = repository.GetSeed(meme.SeedId).ImageData;

				// Add the meme comments to the seed image to make the meme
				meme.ImageData = imageManager.GenerateMemeImage(meme, seedData);
			}

			// If this meme is a reply to another then update the reply trend to reflect the new data of this meme with in the reply list
			if (meme.IsTopLevel == false)
			{
				// This meme is a reply to another parent meme. Update the parent to reflect the new state of this meme 
				this.UpdateReplyToMeme(meme);
			}

			// Save the meme
			var savedMeme = Save(meme);

			// Update the users time line and number of posts (Don't count replies)
			if (isNew && (savedMeme.ResponseToId ?? "").Length == 0)
			{
				repository.Save(new TimeLine(savedMeme.CreatedByUserId, TimeLineEntry.Post, savedMeme.Id, null, null, null));
				var creator = repository.GetUser(savedMeme.CreatedByUserId);
				if (creator != null)
				{
					// Increment the number of posts by this user
					creator.Posts++;
					repository.Save(creator);
				}

			}
			// Add to the time line
			return savedMeme;
		}

		/// <summary>
		/// Adds a reply (with an id) to the replies of the supplied meme.
		/// The reply trend and date added are calculated before appending to the meme
		/// </summary>
		/// <param name="meme">The meme being replied to</param>
		/// <param name="replyMemeId">Id of the reply</param>
		/// <returns></returns>
		public IMeme AddReplyToMeme(IMeme meme, string replyMemeId)
		{
			IMeme replyMeme = repository.GetMeme(replyMemeId);
			if (replyMeme != null)
			{
				if (meme.ReplyIds == null)
				{
					meme.ReplyIds = new List<IReply>();
				}

				// Add the repy id to the top of the list of replies. Set an hourly trend
				meme.ReplyIds.Insert(0, new Reply
				{
					Id = replyMemeId,
					DateCreated = DateTime.UtcNow
				});

				meme.ReplyCount++;

				// Save the meme
				meme = Save(meme);

				// Add reply to time line of the replier 
				repository.Save(new TimeLine(replyMeme.CreatedByUserId, TimeLineEntry.Reply, meme.Id, replyMeme.Id, null, null));

				// Update the number of replies added by the replier
				replyMeme.Creator.Replies++;

				repository.Save(replyMeme.Creator);
			}

			return meme;
		}
		/// <summary>
		/// Update the parent meme to reflect the new state of the supplied meme 
		/// Recalculate and update the reply trend score of the supplied meme inside the reply list of the parent meme (identified by ResponseToId)
		/// </summary>
		/// <param name="replyMeme"></param>
		/// <returns></returns>
		public void UpdateReplyToMeme(IMeme replyMeme)
		{
			if (replyMeme != null && replyMeme.ResponseToId != null)
			{
				IMeme parentMeme = repository.GetMeme(replyMeme.ResponseToId);
				if (parentMeme.ReplyIds == null)
				{
					// This meme is not a reply within the parent meme
					return;
				}

				IReply reply = parentMeme.ReplyIds.FirstOrDefault(x => x.Id == replyMeme.Id);
				if (reply != null)
				{
					// Save the parent meme to which the reply meme is a response to (with the updated reply trend scrore)
					Save(parentMeme);
				}
			}
		}

		/// <summary>
		/// Repost the specified meme by the specified user
		/// </summary>
		/// <param name="meme"></param>
		/// <param name="user"></param>
		/// <returns></returns>
		public IMeme RepostMeme(IMeme meme, IUser user)
		{
			IRepost repost = new Repost
			{
				DateCreated = DateTime.Now.ToUniversalTime(),
				MemeId = meme.RepostOfId ?? meme.Id, // If the meme being reposted is also a repost then use the original meme as the "repost of" id 
				UserId = user.Id,
				UserName = user.UserName
			};

			// Save the repost
			repository.Save(repost);

			// Increment the repost count of the meme and its creator
			meme = Reposted(meme);

			// Add repost to time line
			repository.Save(new TimeLine(user.Id, TimeLineEntry.Repost, repost.MemeId, meme.Id, null, null));

			// Update the number of reposts performed by this user
			user.Reposts++;

			// Save commentator
			repository.Save(user);

			return meme;
		}
		/// <summary>
		/// Meme has been reposted
		/// </summary>
		/// <param name="meme"></param>
		/// <returns></returns>
		private IMeme Reposted(IMeme meme)
		{
			// Increment the repost count of the meme
			meme.Reposts++;

			// Save the meme and calculate it's new trend score
			SaveMeme(meme);

			// Increment the reposted number of the creator
			IUser creator = repository.GetUser(meme.CreatedByUserId);
			if (creator != null)
			{
				// Incrememnt the number of time this user has been reposted
				creator.Reposted++;

				// Save
				repository.Save(creator);
			}

			// If this is a repost then do the same for the original
			if (meme.RepostOfId != null)
			{
				IMeme original = repository.GetMeme(meme.RepostOfId);
				if (original != null)
				{
					// A repost should always reference an original (not another repost)
					if (original.RepostOfId != null)
					{
						throw new Exception(string.Format("Repost of a repost not allowed - {0} - {1}", meme.Id, meme.RepostOfId));
					}

					// Recursive call to update the original
					Reposted(original);
				}
			}
			return meme;
		}
		/// <summary>
		/// Report the specified meme as offensive by the specified user
		/// </summary>
		/// <param name="meme"></param>
		/// <param name="objection"></param>
		/// <param name="user"></param>
		/// <returns></returns>
		public IMeme ReportMeme(IMeme meme, string objection, IUser user)
		{
			IReport report = new Report
			{
				DateCreated = DateTime.Now.ToUniversalTime(),
				MemeId = meme.Id,
				UserId = user.Id,
				Objection = objection

			};

			// Save the report
			repository.Save(report);

			// Up the report count of the meme
			meme.ReportCount++;
			Save(meme);

			return meme;
		}

		/// <summary>
		/// Save the meme. Calculate the meme trend score. Set the meme hash tags trend score.
		/// </summary>
		/// <param name="meme">Meme to save</param>
		/// <returns></returns>
		public IMeme Save(IMeme meme)
		{
			double previousTrendScore = meme.TrendScore;

			// Calculate the Trend score
			meme.TrendScore = CalculateTrendScore(meme);

			// Save meme
			IMeme savedMeme = repository.Save(meme);

			// On an new thread, update the trendscore of the meme's hash tags. Associate the hash tag and meme
			UpdateHashTags(savedMeme, previousTrendScore);

			return savedMeme;
		}

		/// <summary>
		/// Invoke firwe and forget to save the hash tags, calulate the hash tags new trend score and 
		/// link the hash tag to the meme for speedy search by HashTag
		/// </summary>
		/// <param name="savedMeme"></param>
		/// <param name="previousTrendScore"></param>
		private void UpdateHashTags(IMeme savedMeme, double previousTrendScore)
		{
#pragma warning disable 4014
			Task.Run(() =>
			{
				// Update the trendscore of the meme's hash tags. Associate the hash tag and meme
				foreach (string hashTag in savedMeme.HashTags)
				{
					hashTagBusiness.SaveMemeTags(hashTag, savedMeme.Id, previousTrendScore, savedMeme.TrendScore);
				}
			}).ConfigureAwait(false);
#pragma warning restore 4014
		}

		/// <summary>
		/// Based on a supplied date in the past calculate a trend score 
		/// that increments per day and is complemented by activities
		/// </summary>
		/// <param name="meme"></param>
		/// <returns></returns>
		private double CalculateTrendScore(IMeme meme)
		{
			double trendScore = 0;

			// Add a fixed score for every day greater than the base date
			trendScore += (meme.DateCreated - baseDate).Days * ScorePerDay;

			// Add a fixed score for each view
			trendScore += meme.Views * ScorePerView;

			// Add a fixed score for each like
			trendScore += meme.Likes * ScorePerLike;

			// Add a fixed score for each dislike (a negative)
			trendScore += meme.Dislikes * ScorePerDislike;

			// Add a fixed score for each comment
			trendScore += meme.UserCommentCount * ScorePerComment;

			// Add a fixed score for each reply
			trendScore += meme.ReplyCount * ScorePerReply;

			// Add a fixed score for each repost
			trendScore += meme.Reposts * ScorePerRepost;

			return trendScore;
		}


	}
}
