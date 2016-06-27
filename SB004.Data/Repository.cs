using System.Security.Cryptography;
using System.Text;
using MongoDB.Driver.Linq;

namespace SB004.Data
{
    using System;
    using System.Collections.Generic;
    using System.Configuration;
    using System.Linq;
    using MongoDB.Bson.Serialization;

    using SB004.Domain;
    using MongoDB.Bson;
    using MongoDB.Driver;
    using MongoDB.Driver.Builders;

    public class Repository : IRepository
    {
        private readonly MongoCollection<Seed> seedCollection;
        private readonly MongoCollection<Meme> memeCollection;
        private readonly MongoCollection<User> userCollection;
        private readonly MongoCollection<UserComment> userCommentCollection;
        private readonly MongoCollection<Report> reportCollection;
        private readonly MongoCollection<Credentials> userCredentialCollection;
        private readonly MongoCollection<Image> imageCollection;
        private readonly MongoCollection<TimeLine> timeLineCollection;
		private readonly MongoCollection<HashTag> hashTagCollection;
		private readonly MongoCollection<HashTagMeme> hashTagMemeCollection;
        public Repository()
        {
            var connectionString = ConfigurationManager.ConnectionStrings["MongoDb"].ConnectionString;
            MongoClient client = new MongoClient(connectionString);
            MongoServer server = client.GetServer();
            MongoDatabase database = server.GetDatabase(ConfigurationManager.AppSettings["Database"]);

            seedCollection = database.GetCollection<Seed>("seed");
            seedCollection.CreateIndex(IndexKeys<Seed>.Ascending(_ => _.ImageHash));

            memeCollection = database.GetCollection<Meme>("meme");

            userCollection = database.GetCollection<User>("user");

            userCommentCollection = database.GetCollection<UserComment>("userComment");
            
            reportCollection = database.GetCollection<Report>("reported");

            userCredentialCollection = database.GetCollection<Credentials>("userCredential");

            imageCollection = database.GetCollection<Image>("image");

            timeLineCollection = database.GetCollection<TimeLine>("timeLine");

			hashTagCollection = database.GetCollection<HashTag>("hashtag");
			
			hashTagMemeCollection = database.GetCollection<HashTagMeme>("hashtagMeme");

	        if (!BsonClassMap.IsClassMapRegistered(typeof(List<IComment>)))
	        {
		        BsonClassMap.RegisterClassMap<List<IComment>>();
	        }
	        if (!BsonClassMap.IsClassMapRegistered(typeof(Comment)))
	        {
		        BsonClassMap.RegisterClassMap<Comment>();
	        }
	        if (!BsonClassMap.IsClassMapRegistered(typeof(PositionRef)))
	        {
		        BsonClassMap.RegisterClassMap<PositionRef>();
	        }
	        if (!BsonClassMap.IsClassMapRegistered(typeof(List<IReply>)))
	        {
		        BsonClassMap.RegisterClassMap<List<IReply>>();
	        }
	        if (!BsonClassMap.IsClassMapRegistered(typeof(Reply)))
	        {
		        BsonClassMap.RegisterClassMap<Reply>();
	        }
	        if (!BsonClassMap.IsClassMapRegistered(typeof(Report)))
	        {
		        BsonClassMap.RegisterClassMap<Report>();
	        }
	        if (!BsonClassMap.IsClassMapRegistered(typeof(UserLite)))
	        {
		        BsonClassMap.RegisterClassMap<UserLite>();
	        }
	        if (!BsonClassMap.IsClassMapRegistered(typeof(Image)))
	        {
		        BsonClassMap.RegisterClassMap<Image>();
	        }
	        if (!BsonClassMap.IsClassMapRegistered(typeof(TimeLine)))
	        {
		        BsonClassMap.RegisterClassMap<TimeLine>();
	        }
			if (!BsonClassMap.IsClassMapRegistered(typeof(HashTag)))
	        {
				BsonClassMap.RegisterClassMap<HashTag>();
	        }

			// Create index on hash tag in hashtag meme
			hashTagMemeCollection.CreateIndex(IndexKeys<HashTagMeme>.Ascending(_ => _.HashTag));
        }

	    private string NewShortId()
	    {
		    const int maxSize = 8;
		    char[] chars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890".ToCharArray();
			byte[] data = new byte[1];
			using (RNGCryptoServiceProvider crypto = new RNGCryptoServiceProvider())
			{
				crypto.GetNonZeroBytes(data);
				data = new byte[maxSize];
				crypto.GetNonZeroBytes(data);
			}
			StringBuilder result = new StringBuilder(maxSize);
			foreach (byte b in data)
			{
				result.Append(chars[b % (chars.Length)]);
			}
			return result.ToString();
	    }
	    private string NewLongId()
	    {
			return Guid.NewGuid().ToString("N");
	    }
	    #region Seed

        /// <summary>
        /// Persist the supplied seed and assign an ID
        /// </summary>
        /// <param name="seed"></param>
        /// <returns></returns>
        public ISeed Save(ISeed seed)
        {
            seed.Id = seed.Id ?? NewShortId();
            seedCollection.Save(seed.ToBsonDocument());
            return seed;
        }
        /// <summary>
        /// Retrieve the seed id of a seed for a given hash
        /// </summary>
        /// <param name="seedImageHash"></param>
        /// <returns></returns>
        public ISeed GetSeedByHash(string seedImageHash)
        {
            ISeed seedEntity = seedCollection.FindOne(Query<Seed>.EQ(e => e.ImageHash, seedImageHash));
            if (seedEntity == null)
            {
                return null;
            }

            return seedEntity;
        }
        /// <summary>
        /// Retrieve the seed
        /// </summary>
        /// <param name="seedId"></param>
        /// <returns></returns>
        public ISeed GetSeed(string seedId)
        {
            Seed seedEntity = seedCollection.FindOne(Query<Seed>.EQ(e => e.Id, seedId));
            if (seedEntity == null)
            {
                return null;
            }

            return seedEntity;
        }
        #endregion

        #region Meme
        /// <summary>
        /// Persist the supplied meme and assign an ID
        /// </summary>
        /// <param name="meme"></param>
        /// <returns></returns>
        public IMeme Save(IMeme meme)
        {
            meme.Id = meme.Id ?? NewShortId();
            memeCollection.Save(meme.ToBsonDocument());
            return meme;
        }
        /// <summary>
        /// Retrieve the meme
        /// </summary>
        /// <param name="memeId"></param>
        /// <returns></returns>
        public IMeme GetMeme(string memeId)
        {
            Meme memeEntity = memeCollection.FindOne(Query<Meme>.EQ(e => e.Id, memeId));
            if (memeEntity == null)
            {
                return null;
            }

            return memeEntity.SetCreator(GetUser(memeEntity.CreatedByUserId));
        }
        /// <summary>
        /// Search for memes
        /// </summary>
        /// <param name="skip"></param>
        /// <param name="take"></param>
        /// <returns></returns>
        public List<IMeme> SearchMeme(int skip, int take)
        {
	        List<IMeme> memes = new List<IMeme>();

	        var cursor =
		        memeCollection.FindAllAs<Meme>().SetSortOrder(SortBy.Descending("DateCreated")).SetLimit(take);

	        foreach (Meme entity in cursor)
	        {
		        // Add meme (resolving creator user object)
		        memes.Add(entity.SetCreator(GetUser(entity.CreatedByUserId)));
	        }

	        return memes;
        }

        /// <summary>
        /// Search for trending memes ordering 
        /// </summary>
        /// <param name="skip"></param>
        /// <param name="take"></param>
        /// <returns></returns>
        public List<IMeme> SearchTrendingMemes(int skip, int take)
        {
            List<IMeme> memes = new List<IMeme>();

            var cursor =
                  memeCollection.FindAs<Meme>(Query<Meme>.EQ(e => e.IsTopLevel, true)).SetSortOrder(SortBy.Descending("TrendScore")).SetLimit(take);

            foreach (Meme entity in cursor)
            {
                // Add meme (resolving creator user object)
                memes.Add(entity.SetCreator(GetUser(entity.CreatedByUserId)));
            }

            return memes;
        }
        /// <summary>
        /// Repost a meme
        /// </summary>
        /// <param name="repost"></param>
        /// <returns></returns>
        public IMeme Save(IRepost repost)
        {
            IMeme newMemeFromOriginal = GetMeme(repost.MemeId);
            newMemeFromOriginal.RepostOfId = repost.MemeId;
            newMemeFromOriginal.Id = null;
            newMemeFromOriginal.DateCreated = repost.DateCreated;
            newMemeFromOriginal.CreatedByUserId = repost.UserId;
            newMemeFromOriginal.CreatedBy = repost.UserName;
            newMemeFromOriginal.Likes = 0;
            newMemeFromOriginal.Dislikes = 0;
            newMemeFromOriginal.Reposts = 0;
            newMemeFromOriginal.Views = 0;
            newMemeFromOriginal.IsTopLevel = true;
            return Save(newMemeFromOriginal);
        }

        /// <summary>
        /// Save the report of an offensive meme
        /// </summary>
		/// <param name="report"></param>
        /// <returns></returns>
        public IReport Save(IReport report)
        {
            report.Id = report.Id ?? NewShortId();
            reportCollection.Save(report.ToBsonDocument());
            return report;
        }

	    /// <summary>
	    /// Search for memes by user id
	    /// </summary>
	    /// <param name="userId"></param>
	    /// <param name="skip"></param>
	    /// <param name="take"></param>
	    /// <param name="fullCount"></param>
	    /// <returns></returns>
	    public List<IMeme> SearchMemeByUser(string userId, int skip, int take, out long fullCount)
        {
		    List<IMeme> memes = new List<IMeme>();
 
		    var query = Query.And(
			    Query<Meme>.EQ(e => e.CreatedByUserId, userId), 
			    Query<Meme>.EQ(e => e.IsTopLevel,true));
		    var resultItems = new List<Meme>();
		    var cursor = memeCollection.FindAs<Meme>(query);
		    cursor.SetSortOrder(SortBy.Descending("DateCreated"));
		    cursor.SetSkip(skip).SetLimit(take);
		    resultItems.AddRange(cursor);
		    fullCount = cursor.Count();
		    foreach (Meme entity in resultItems)
		    {
			    // Add meme (resolving creator user object)
			    memes.Add(entity.SetCreator(GetUser(entity.CreatedByUserId)));
		    }

		    return memes;
        }
        #endregion

        #region User
        /// <summary>
        /// Upsert the supplied user
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public IUser Save(IUser user)
        {
            user.Id = user.Id ?? NewShortId();
            userCollection.Save(user.ToBsonDocument());
            return user;
        }
        /// <summary>
        /// Save user credentials
        /// </summary>
        /// <param name="credentials"></param>
        /// <returns></returns>
        public ICredentials Save(ICredentials credentials)
        {
            userCredentialCollection.Save(credentials.ToBsonDocument());
            return credentials;
        }
        /// <summary>
        /// Retrieve the credentials for the given email
        /// </summary>
        /// <param name="email"></param>
        /// <returns></returns>
        public ICredentials GetCredentials(string email) 
        {
            Credentials credentialsEntity = userCredentialCollection.FindOne(Query<Credentials>.EQ(e => e.Email, email));
            if (credentialsEntity == null)
            {
                return null;
            }

            return credentialsEntity;                        
        }
        /// <summary>
        /// Retrieve the user by authentication provider user id and provider name
        /// </summary>
        /// <returns></returns>
        public IUser GetUser(string authenticationUserId, string authenticationProvider)
        {
            IUser user = userCollection.FindOne(Query.And(
                        Query.EQ("AuthenticationUserId", authenticationUserId),
                        Query.EQ("AuthenticationProvider", authenticationProvider)));
            
            if (user != null)
            {
                // Resolve the following
                List<IUser> following = new List<IUser>();
                user.FollowingIds.ForEach(f => following.Add(GetUser(f.Id)));
                user.SetFollowing(following);
				// Resolve the followedBy
				List<IUser> followedBy = new List<IUser>();
				user.FollowedByIds.ForEach(f => followedBy.Add(GetUser(f.Id)));
				user.SetFollowedBy(followedBy);
            }

            return user;
        }

	    /// <summary>
	    /// Retrieve the user by id 
	    /// </summary>
	    /// <param name="userId">user id</param>
		/// <param name="deep">If true resolve list of Following ids and foloowed by ids to a list of users</param>
	    /// <returns></returns>
	    public IUser GetUser(string userId, bool deep = false)
        {
            IUser user = userCollection.FindOne(Query<User>.EQ(e => e.Id, userId));
            if (user != null)
            {
                List<IUser> following = new List<IUser>();
				List<IUser> followedBy = new List<IUser>();
                // Resolve the following
                if(deep)
                {
                    user.FollowingIds.ForEach(f =>
                    {
                        // If following himself, don't resolve ... infinite loop!
                        if (f.Id != user.Id)
                        {
                            following.Add(GetUser(f.Id));
                        }
                    });
					user.SetFollowing(following);

					user.FollowedByIds.ForEach(f =>
					{
						// If following himself, don't resolve ... infinite loop!
						if (f.Id != user.Id)
						{
							followedBy.Add(GetUser(f.Id));
						}
					});
					user.SetFollowedBy(followedBy);
                }
            }
            return user;
        }
        
        #endregion

        #region User Comment
        public long GetUserCommentCount(string memeId)
        {
            var cursor =
                  userCommentCollection.FindAs<UserComment>(Query<UserComment>.EQ(e => e.MemeId, memeId));

            return cursor.Count();
        }
        public IUserComment GetUserComment(string userCommentId)
        {
            return userCommentCollection.FindOne(Query<UserComment>.EQ(e => e.Id, userCommentId));
        }

        /// <summary>
        /// Retrieve the user added comments for a specified meme id
        /// </summary>
        /// <param name="memeId">Meme id that the user comments have been added to</param>
        /// <param name="skip">for pagination</param>
        /// <param name="take">for pagination</param>
        /// <returns></returns>
        public List<IUserComment> GetUserComments(string memeId, int skip, int take)
        {
	        List<IUserComment> userComments = new List<IUserComment>();
	        var cursor =
		        userCommentCollection.FindAs<UserComment>(Query<UserComment>.EQ(e => e.MemeId, memeId))
			        .SetSortOrder(SortBy.Ascending("DateCreated"))
			        .Skip(skip).Take(take);

	        foreach (UserComment entity in cursor)
	        {
		        userComments.Add(entity);
	        }

	        return userComments;
        }
        /// <summary>
        /// Add a user comment
        /// </summary>
        /// <param name="userComment"></param>
        /// <returns></returns>
        public IUserComment Save(IUserComment userComment)
        {
			userComment.Id = userComment.Id ?? NewLongId();
            userCommentCollection.Save(userComment.ToBsonDocument());
            return userComment;
        }
        #endregion
       
        #region Image
        public IImage Save(IImage image)
        {
            image.Id = image.Id ?? NewShortId();
            imageCollection.Save(image.ToBsonDocument());
            return image;
        }
        public IImage GetImage(string id)
        {
            return imageCollection.FindOne(Query<Image>.EQ(e => e.Id, id));
        }

	    public void DeleteImage(string id)
	    {
			imageCollection.Remove(Query.EQ("_id", id));
	    }

	    #endregion

        #region Time line

	    /// <summary>
	    /// Add a time line entry
	    /// </summary>
	    /// <returns></returns>
	    public ITimeLine Save(ITimeLine timeLine)
        {
		    if (timeLine.EntryType == TimeLineEntry.All)
		    {
				throw new Exception("All not a valid type for time line entry. Only when reading!");
		    }
			timeLine.Id = NewLongId();
		    timeLineCollection.Save(timeLine.ToBsonDocument());
            return timeLine;
        }

	    /// <summary>
	    /// Retrieve the time line of a particular user paginated if requested
	    /// </summary>
	    /// <param name="userId">ID of the user</param>
	    /// <param name="skip">Number of itms to skip</param>
	    /// <param name="take"></param>
	    /// <param name="type"></param>
	    /// <returns></returns>
	    public List<ITimeLine> GetUserTimeLine(string userId, int skip, int take, TimeLineEntry type)
	    {
			List<ITimeLine> activity = new List<ITimeLine>();
		    IQueryable<TimeLine> queryTimeline;

		    if (type == TimeLineEntry.All)
		    {
				// Bring back all entry types
				queryTimeline = (from entry in timeLineCollection.AsQueryable()
								 where entry.UserId == userId
								 select entry).OrderByDescending(x => x.DateOfEntry).Skip(skip).Take(take);
		    }
			else if (type == TimeLineEntry.Like)
			{
				queryTimeline = (from entry in timeLineCollection.AsQueryable()
											  where entry.UserId == userId
											  && (entry.EntryType == TimeLineEntry.Like || entry.EntryType == TimeLineEntry.LikeComment)
											  select entry).OrderByDescending(x => x.DateOfEntry).Skip(skip).Take(take);
			}
		    else
		    {
				// Bring back specific entry types
				queryTimeline = (from entry in timeLineCollection.AsQueryable()
								 where entry.UserId == userId
								 && entry.EntryType == type
								 select entry).OrderByDescending(x => x.DateOfEntry).Skip(skip).Take(take);
		    }

			foreach (var entry in queryTimeline)
			{
				// Add the activity 
				activity.Add(entry);
			}

			return activity;

	    }

		/// <summary>
		/// Get all entries to the time line for a specified user update to a number of days in the past
		/// </summary>
		/// <param name="userId"></param>
		/// <param name="days"></param>
		/// <returns></returns>
	    public List<ITimeLine> GetUserTimeLine(string userId, int days)
	    {
			DateTime dateOfEntry = DateTime.Now.AddDays(days * -1);

			List<ITimeLine> activity = new List<ITimeLine>();

			IQueryable<TimeLine> query = (from entry in timeLineCollection.AsQueryable()
										 where entry.UserId == userId
										 && entry.DateOfEntry >= dateOfEntry
										 select entry).OrderByDescending(x=>x.DateOfEntry);
			foreach (var entry in query)
			{
				// Add the activity 
				activity.Add(entry);
			}

			return activity;
	    }
		/// <summary>
		/// Retrieve the time line of a particular meme
		/// </summary>
		/// <param name="memeId">ID of the meme</param>
		/// <param name="days">Number of days activities to take</param>
		/// <returns></returns>
		public List<ITimeLine> GetMemeTimeLine(string memeId, int days)
		{
			DateTime dateOfEntry = DateTime.Now.AddDays(days*-1);

			List<ITimeLine> activity = new List<ITimeLine>();

			IQueryable<TimeLine> query = from entry in timeLineCollection.AsQueryable()
						where entry.TimeLineRefId == memeId 
						&& entry.DateOfEntry >= dateOfEntry
						select entry;

			foreach (var entry in query)
			{
				// Add the activity for the meme posted by the user
				activity.Add(entry);
			}
			return activity;
		}
		/// <summary>
		/// Retrieve the timeline activity of all memes
		/// the user posted or reposted by the specified user for a max number of days
		/// </summary>
		/// <param name="userId">ID of the user</param>
		/// <param name="days">Number of in the past to go</param>
		/// <returns></returns>
		public List<ITimeLine> GetUserMemeTimeLine(string userId, int days)
		{
			// Get all the mems of this user
			IQueryable<TimeLine> query = from entry in timeLineCollection.AsQueryable()
										 where entry.UserId == userId
										 && (entry.EntryType == TimeLineEntry.Post || entry.EntryType == TimeLineEntry.Repost)
										 select entry;

			List<ITimeLine> activity = new List<ITimeLine>();
			foreach (var post in query)
			{
				// Determine the meme id. If a repost user the AlternateId
				string memeId = post.EntryType == TimeLineEntry.Post ? post.TimeLineRefId : post.TimeLineRefAlternateId;

				// Add the activity for the meme posted by the user
				activity.AddRange(GetMemeTimeLine(memeId, days));
			}

			return activity;
		}
		
	    #endregion
		#region HashTags
		/// <summary>
		/// Get a hashtag by name (ID is hash tag name)
		/// </summary>
		/// <param name="hashTagName"></param>
		/// <returns></returns>
		public IHashTag GetHashTag(string hashTagName)
		{
			IHashTag hashTag = hashTagCollection.FindOne(Query<HashTag>.EQ(e => e.Id, hashTagName));
			return hashTag;
		}
		/// <summary>
		/// Save the hashtag
		/// </summary>
		/// <param name="hashTag"></param>
		/// <returns></returns>
	    public IHashTag Save(IHashTag hashTag)
	    {
			hashTagCollection.Save(hashTag.ToBsonDocument());
			return hashTag;
	    }	
    	/// <summary>
    	/// Save the link between an hashtag and a meme
    	/// </summary>
    	/// <param name="hashTagMeme"></param>
    	/// <returns></returns>
		public IHashTagMeme Save(IHashTagMeme hashTagMeme)
    	{
			hashTagMeme.Id = hashTagMeme.Id ?? hashTagMeme.HashTag + hashTagMeme.MemeId;
			hashTagMemeCollection.Save(hashTagMeme.ToBsonDocument());
			return hashTagMeme;
	    }

		public List<IHashTag> SearchTrendingHashTags(int take)
	    {
			IQueryable<HashTag> query = (from entry in hashTagCollection.AsQueryable()
										 select entry).OrderByDescending(x => x.TrendScoreOfAllMemes).Take(take);

			List<IHashTag> hashTags = new List<IHashTag>();
			foreach (var hashTag in query)
			{
				hashTags.Add(hashTag);
			}

			return hashTags;
	    }

	    public List<string> SearchHashTagMemes(string hashTag, int take)
	    {
		    string hashTagToLower = hashTag.ToLower();
			IQueryable<HashTagMeme> query = (from entry in hashTagMemeCollection.AsQueryable()
											 where entry.HashTag == hashTagToLower
										 select entry).OrderByDescending(x => x.TrendScore).Take(take);

			List<string> memeIds = new List<string>();
			foreach (var hashTagMeme in query)
			{
				memeIds.Add(hashTagMeme.MemeId);
			}

			return memeIds;
	    }
		public List<IHashTag> SearchHashTags(string hashTag, int take)
		{
			IQueryable<HashTag> query = (from entry in hashTagCollection.AsQueryable()
										 where entry.Id.Contains(hashTag.ToLower())
										 select entry).OrderByDescending(x => x.TrendScoreOfAllMemes).Take(take);

			List<IHashTag> hashTags = new List<IHashTag>();
			foreach (var tag in query)
			{
				hashTags.Add(tag);
			}

			return hashTags;
		}
	    #endregion

	}
}