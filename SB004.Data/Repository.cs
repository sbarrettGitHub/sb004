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

    public Repository()
    {
      var connectionString = ConfigurationManager.ConnectionStrings["MongoDb"].ConnectionString;
      MongoClient client = new MongoClient(connectionString);
      MongoServer server = client.GetServer();
      MongoDatabase database = server.GetDatabase(ConfigurationManager.AppSettings["Database"]);

      seedCollection = database.GetCollection<Seed>("seed");
      seedCollection.CreateIndex(IndexKeys<Seed>.Ascending(_ => _.ImageHash));

      memeCollection = database.GetCollection<Meme>("meme");
    }
    #region Seed

    /// <summary>
    /// Persist the supplied seed and assign an ID
    /// </summary>
    /// <param name="seed"></param>
    /// <returns></returns>
    public ISeed AddSeed(ISeed seed)
    {
      seed.Id = Guid.NewGuid().ToString("N");
      seedCollection.Insert(seed.ToBsonDocument());
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
    public IMeme AddMeme(IMeme meme)
    {
      meme.Id = Guid.NewGuid().ToString("N");
      memeCollection.Insert(meme.ToBsonDocument());
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
  }
}