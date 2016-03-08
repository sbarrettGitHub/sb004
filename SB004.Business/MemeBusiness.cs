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
        readonly ITrendManager trendManager;

        public MemeBusiness(IRepository repository, IImageManager imageManager, ITrendManager trendManager)
        {
            this.repository = repository;
            this.imageManager = imageManager;
            this.trendManager = trendManager;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <param name="likesIncrement"></param>
        /// <param name="dislikesIncrement"></param>
        /// <param name="viewsIncrement"></param>
        /// <param name="sharesIncrement"></param>
        /// <param name="favouritesIncrement"></param>
        /// <returns></returns>
        public IMeme UpdateMemeInteraction(string id, int likesIncrement, int dislikesIncrement, int viewsIncrement, int sharesIncrement, int favouritesIncrement)
        {
            IMeme meme = repository.GetMeme(id);
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

            IUser memeCreator = repository.GetUser(meme.CreatedByUserId);
            if (memeCreator != null)
            {
                memeCreator.Likes += likesIncrement;
                memeCreator.Dislikes += dislikesIncrement;
                memeCreator.Views += viewsIncrement;
                memeCreator.Shares += sharesIncrement;
                memeCreator.Favourites += favouritesIncrement;

                // Save meme creator
                repository.Save(memeCreator);
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
                    UpdateMemeInteraction(meme.RepostOfId, likesIncrement, dislikesIncrement, viewsIncrement, sharesIncrement, favouritesIncrement);
                    
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
            long userCommentCount = 0;
            bool isNew = false;
            // Get the number of user comments
            if (meme.Id != null)
            {
                userCommentCount = repository.GetUserCommentCount(meme.Id);
            }
            else 
            {
                isNew = true;
            }

            // Meme is top leve if it has no reposnseToId
            meme.IsTopLevel = meme.ResponseToId == null;

            // Calculate the meme trend score
            meme.TrendScore = trendManager.CalculateDailyTrendScore(meme, userCommentCount);

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
            var savedMeme = repository.Save(meme);

            // Update the users time line
            if (isNew) 
            {
                repository.Save(new TimeLine
                {
                    UserId = savedMeme.CreatedByUserId,
                    DateOfEntry = DateTime.Now,
                    EntryType = TimeLineEntry.Post,
                    TimeLineRefId = savedMeme.Id
                });
            }
            // Add to the time line
            return savedMeme;
        }

        /// <summary>
        /// Adds a reply (with an id) to the replies of the supplied meme.
        /// The reply trend and date added are calculated before appending to the meme
        /// </summary>
        /// <param name="meme"></param>
        /// <param name="replyMemeId"></param>
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
                    DateCreated = DateTime.UtcNow,
                    TrendScore = trendManager.CalculateHourlyTrendScore(replyMeme, 0, meme.DateCreated)
                });

                // Save the meme
                meme = repository.Save(meme);
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
                    // Update the hourly trend score of this meme as a reply to another meme
                    reply.TrendScore = trendManager.CalculateHourlyTrendScore(replyMeme, repository.GetUserCommentCount(replyMeme.Id), parentMeme.DateCreated);

                    // Save the parent meme to which the reply meme is a response to (with the updated reply trend scrore)
                    repository.Save(parentMeme);
                }
            }
        }
        /// <summary>
        /// Repost the specified meme to the specified user
        /// </summary>
        /// <param name="meme"></param>
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
            
            // Get the 
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

            return meme;
        }
    }
}
