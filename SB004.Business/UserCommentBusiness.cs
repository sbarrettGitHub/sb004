using SB004.Data;
using SB004.Domain;

namespace SB004.Business
{
	public class UserCommentBusiness : IUserCommentBusiness
	{
		readonly IRepository repository;
		readonly IMemeBusiness memeBusiness;

		public UserCommentBusiness(IRepository repository)
		{
		  this.repository = repository;
		}
		/// <summary>
		/// Inser the comment and record on user time line
		/// </summary>
		/// <param name="userComment"></param>
		/// <returns></returns>
		public IUserComment PostUserComment(IUserComment userComment)
		{
			// Post the comment
			userComment = repository.Save(userComment);
			
			// Record on time line
			repository.Save(new TimeLine(userComment.UserId, TimeLineEntry.Comment, userComment.MemeId, userComment.Id, null, null));
			
			// Update the number of comments added by this user
			IUser user = repository.GetUser(userComment.UserId);
			if (user != null)
			{
				user.Comments ++;

				// Save commentator
				repository.Save(user);
			}

			// Update the number of comments on the meme
			IMeme meme = repository.GetMeme(userComment.MemeId);
			if (meme != null)
			{
				meme.UserCommentCount++;

				// Save comment count
				memeBusiness.Save(meme);
			}
			return userComment;
		}

		/// <summary>
		/// Record the like of a user comment on the comment and in the time line
		/// </summary>
		/// <param name="userCommentId"></param>
		/// <param name="userId"></param>
		/// <returns></returns>
		public IUserComment LikeComment(string userCommentId, string userId)
		{
			IUserComment userComment = repository.GetUserComment(userCommentId);
			if (userComment == null)
			{
				return null;
			}

			// Increment the likes of the comment
			userComment.Likes++;

			// Record on the user comment
			userComment = repository.Save(userComment);

			// If not anonymous then record in the timeline of the user doing the action
			if (userId != null)
			{
				repository.Save(new TimeLine(userId, TimeLineEntry.LikeComment, userComment.MemeId, userCommentId, null, null));
			}
			return userComment;
		}
		/// <summary>
		/// Record the dislike of a user comment on the comment and in the time line
		/// </summary>
		/// <param name="userCommentId"></param>
		/// <param name="userId"></param>
		/// <returns></returns>
		public IUserComment DislikeComment(string userCommentId, string userId)
		{
			IUserComment userComment = repository.GetUserComment(userCommentId);
			if (userComment == null)
			{
				return null;
			}

			// Increment the likes of the comment
			userComment.Dislikes++;

			// Record on the user comment
			userComment = repository.Save(userComment);

			// If not anonymous then record in the timeline of the user doing the action
			if (userId != null)
			{
				repository.Save(new TimeLine(userId, TimeLineEntry.DislikeComment, userComment.MemeId, userCommentId, null, null));
			}

			return userComment;
		}
	}
}
