namespace SB004.Data
{
    using System.Configuration;
    using System.IO;
    using System.Text;
    using SB004.Utilities;
    using SB004.Data.Entities;
    using SB004.Domain;
    using MongoDB.Bson;
    using MongoDB.Driver;
    using MongoDB.Driver.Builders;

    public class Repository : IRepository
    {
        private readonly MongoClient client;
        private readonly MongoServer server;
        private readonly MongoDatabase database;
        private readonly MongoCollection<SeedEntity> seedCollection;

        public Repository()
        {
            var connectionString = ConfigurationManager.ConnectionStrings["MongoDb"].ConnectionString;
            client = new MongoClient(connectionString);
            server = client.GetServer();
            database = server.GetDatabase(ConfigurationManager.AppSettings["Database"]);
            seedCollection = database.GetCollection<SeedEntity>("entities");
            seedCollection.CreateIndex(IndexKeys<SeedEntity>.Ascending(_ => _.ImageHash));

        }
        /// <summary>
        /// Persist the supplied seed and assign an ID
        /// </summary>
        /// <param name="seed"></param>
        /// <returns></returns>
        public ISeed AddSeed(ISeed seed)
        {
            SeedEntity seedEntity = new SeedEntity(seed);
            seedCollection.Insert(seedEntity);
            seed.Id = seedEntity.Id.ToString();
            return seed;
        }
        /// <summary>
        /// Retrieve the seed id of a seed for a given hash
        /// </summary>
        /// <param name="seedImageHash"></param>
        /// <returns></returns>
        public ISeed GetSeedByHash(string seedImageHash)
        {
            SeedEntity seedEntity = seedCollection.FindOne(Query<SeedEntity>.EQ(e => e.ImageHash, seedImageHash));
            if (seedEntity == null) 
            {
                return null;
            }

            return seedEntity.ToISeed();       
        }
        /// <summary>
        /// Retrieve the seed
        /// </summary>
        /// <param name="seedId"></param>
        /// <returns></returns>
        public ISeed GetSeed(string seedId)
        {
            SeedEntity seedEntity = seedCollection.FindOne(Query<SeedEntity>.EQ(e => e.Id, new ObjectId(seedId)));
            if (seedEntity == null)
            {
                return null;
            }

            return seedEntity.ToISeed();   
        }

    }
}