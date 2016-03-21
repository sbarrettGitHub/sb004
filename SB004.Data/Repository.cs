using System.Security.Cryptography;
using System.Text;

namespace SB004.Data
{
    using System;
    using System.Collections.Generic;
    using System.Configuration;
    using System.Linq;
    using System.Linq.Expressions;

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
        private readonly MongoCollection<Repost> repostCollection;
        private readonly MongoCollection<Report> reportCollection;
        private readonly MongoCollection<Credentials> userCredentialCollection;
        private readonly MongoCollection<Image> imageCollection;
        private readonly MongoCollection<TimeLine> timeLineCollection;
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
            
            repostCollection = database.GetCollection<Repost>("reposted");

            reportCollection = database.GetCollection<Report>("reported");

            userCredentialCollection = database.GetCollection<Credentials>("userCredential");

            imageCollection = database.GetCollection<Image>("image");

            timeLineCollection = database.GetCollection<TimeLine>("timeLine");
            try
            {
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
            }
            catch (Exception)
            {

                throw;
            }
        }

	    private string NewId()
	    {
		    const int maxSize = 8;
			char[] chars = new char[62];
			chars =
			"abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890".ToCharArray();
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

	    #region Seed

        /// <summary>
        /// Persist the supplied seed and assign an ID
        /// </summary>
        /// <param name="seed"></param>
        /// <returns></returns>
        public ISeed Save(ISeed seed)
        {
            seed.Id = seed.Id ?? NewId();
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
        /// Persist the supplied seed and assign an ID
        /// </summary>
        /// <param name="meme"></param>
        /// <returns></returns>
        public IMeme Save(IMeme meme)
        {
            meme.Id = meme.Id ?? NewId();
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
            try
            {
                var query = new QueryDocument();
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
            catch (Exception)
            {

                throw;
            }
        }

        /// <summary>
        /// Search for trending memes ordering 
        /// </summary>
        /// <param name="skip"></param>
        /// <param name="take"></param>
        /// <returns></returns>
        public List<IMeme> SearchTrendingMemes(int skip, int take)
        {
            var query = new QueryDocument();
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
        /// <param name="repost"></param>
        /// <returns></returns>
        public IReport Save(IReport report)
        {
            report.Id = report.Id ?? NewId();
            reportCollection.Save(report.ToBsonDocument());
            return report;
        }
        /// <summary>
        /// Search for memes by user id
        /// </summary>
        /// <param name="skip"></param>
        /// <param name="take"></param>
        /// <returns></returns>
        public List<IMeme> SearchMemeByUser(string userId, int skip, int take, out long fullCount)
        {
            try
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
            catch (Exception)
            {
                throw;
            }
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
            user.Id = user.Id ?? NewId();
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
            }

            return user;
        }

        /// <summary>
        /// Retrieve the user id 
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public IUser GetUser(string userId, bool deep = false)
        {
            IUser user = userCollection.FindOne(Query<User>.EQ(e => e.Id, userId));
            if (user != null)
            {
                List<IUser> following = new List<IUser>();
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
                    
                }
                user.SetFollowing(following);

            }
            
            return user;
        }
        
        #endregion

        #region User Comment
        public long GetUserCommentCount(string memeId)
        {
            var query = new QueryDocument();
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
            try
            {
                var query = new QueryDocument();
                List<IUserComment> userComments = new List<IUserComment>();
                var cursor =
                      userCommentCollection.FindAs<UserComment>(Query<UserComment>.EQ(e => e.MemeId, memeId))
                      .SetSortOrder(SortBy.Descending("DateCreated"))
                      .Skip(skip).Take(take);

                foreach (UserComment entity in cursor)
                {
                    userComments.Add(entity);
                }

                return userComments;
            }
            catch (Exception)
            {

                throw;
            }
        }
        /// <summary>
        /// Add a user comment
        /// </summary>
        /// <param name="userComment"></param>
        /// <returns></returns>
        public IUserComment Save(IUserComment userComment)
        {
            userComment.Id = userComment.Id ?? NewId();
            userCommentCollection.Save(userComment.ToBsonDocument());
            return userComment;
        }
        #endregion
       
        #region Image
        public IImage Save(IImage image)
        {
            image.Id = image.Id ?? NewId();
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
	    public List<ITimeLine> GetUserTimeLine(string userId, int skip, int take, TimeLineEntry? type = null)
	    {
		    var cursor =
				  timeLineCollection.FindAs<TimeLine>(Query<TimeLine>.EQ(e => e.UserId, userId)).SetSortOrder(SortBy.Descending("DateOfEntry")).SetSkip(skip).SetLimit(take);

		    return cursor.Cast<ITimeLine>().ToList();
	    }

	    #endregion
    }
}