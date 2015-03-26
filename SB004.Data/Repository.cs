namespace SB004.Data
{
    using System;
    using System.Configuration;

    using SB004.Domain;
    using MongoDB.Bson;
    using MongoDB.Driver;
    using MongoDB.Driver.Builders;

    public class Repository : IRepository
    {
        private readonly MongoCollection<Seed> seedCollection;
        private readonly MongoCollection<Meme> memeCollection;
        private readonly MongoCollection<User> userCollection;

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
            meme.Id = meme.Id??Guid.NewGuid().ToString("N");
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
        public IUser GetUser(string authenticationUserId, string AuthenticationProvider)
        {           
            return userCollection.FindOne(Query.And(
                        Query.EQ("AuthenticationUserId", authenticationUserId),
                        Query.EQ("AuthenticationProvider", AuthenticationProvider)));           
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
    }
}