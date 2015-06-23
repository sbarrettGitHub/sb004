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
            }
            catch (Exception )
            {

                throw;
            }
        }
        #region Seed

        /// <summary>
        /// Persist the supplied seed and assign an ID
        /// </summary>
        /// <param name="seed"></param>
        /// <returns></returns>
        public ISeed SaveSeed(ISeed seed)
        {
            seed.Id = seed.Id ?? Guid.NewGuid().ToString("N");
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
        public IMeme SaveMeme(IMeme meme)
        {
            meme.Id = meme.Id ?? Guid.NewGuid().ToString("N");
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

            return memeEntity;
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
                    memes.Add(entity);
                }

                return memes;
            }
            catch (Exception )
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
        public IUser SaveUser(IUser user)
        {
            user.Id = user.Id ?? Guid.NewGuid().ToString("N");
            userCollection.Save(user.ToBsonDocument());
            return user;
        }
        /// <summary>
        /// Retrieve the user by authentication provider user id and provider name
        /// </summary>
        /// <param name="seedId"></param>
        /// <returns></returns>
        public IUser GetUser(string authenticationUserId, string authenticationProvider)
        {
            return userCollection.FindOne(Query.And(
                        Query.EQ("AuthenticationUserId", authenticationUserId),
                        Query.EQ("AuthenticationProvider", authenticationProvider)));
        }

        /// <summary>
        /// Retrieve the user id 
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public IUser GetUser(string userId)
        {
            return userCollection.FindOne(Query<User>.EQ(e => e.Id, userId));
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
        public IUserComment SaveUserComment(IUserComment userComment)
        {
            userComment.Id = userComment.Id ?? Guid.NewGuid().ToString("N");
            userCommentCollection.Save(userComment.ToBsonDocument());
            return userComment;
        }
        #endregion
    }
}