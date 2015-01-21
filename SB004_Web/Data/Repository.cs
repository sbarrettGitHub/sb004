
namespace SB004.Data
{
  using System.Configuration;
  using System.IO;
  using System.Text;
  using Microsoft.WindowsAzure.Storage;
  using Microsoft.WindowsAzure.Storage.Blob;
  using Microsoft.WindowsAzure.Storage.Table;
  using SB004.Business;
  using SB004.Data.Entities;
  using SB004.Domain;

  public class Repository : IRepository
  {
    private readonly IIdManager idManager;

    private readonly IImageManager imageManager;

    private readonly string dataTablePrefix;

    private const string SeedEntityName = "Seed";
    private const string SeedHashEntityName = "SeedHash";
    private const string SeedImageEntityName = "seedimage";
    private const string MemeImageEntityName = "memeimage";
    private const string UnknownImageEntityName = "unknownimage";
    // todo: Meme management
    //private const string memeEntityName = "Meme";

    private readonly CloudTableClient client;

    // Create the blob client. 
    private readonly CloudBlobClient blobClient;

    public Repository(IIdManager idManager, IImageManager imageManager)
    {
      this.idManager = idManager;
      this.imageManager = imageManager;

      dataTablePrefix = ConfigurationManager.AppSettings["DataTablePrefix"];

      string connectionString = ConfigurationManager.ConnectionStrings["TableStorage"].ConnectionString;
      CloudStorageAccount storageAccount = CloudStorageAccount.Parse(connectionString);
      client = storageAccount.CreateCloudTableClient();

      blobClient = storageAccount.CreateCloudBlobClient();
      
    }
    /// <summary>
    /// Persist the supplied seed and assign an ID
    /// </summary>
    /// <param name="seed"></param>
    /// <returns></returns>
    public ISeed AddSeed(ISeed seed)
    {
      // Generate a seed id in the format YYDDMMGuid e.g. 150120s4b87f1daf40f4c63829083768596e2d4
      seed.Id = this.idManager.NewId(IdType.Seed);
      
      #region image blob
      
      // Generate the blob container using  "SeedImage" and the YYMM portion (leftmost 4) if the ID
      string seedImageContainerName = this.ConstructTableName(SeedImageEntityName, this.idManager.GetIdPrefix(seed.Id));
      
      CloudBlobContainer imageContainer = blobClient.GetContainerReference(seedImageContainerName);
      imageContainer.CreateIfNotExists(BlobContainerPublicAccessType.Container);

      // Retrieve reference to a blob of the seed id
      CloudBlockBlob blockBlob = imageContainer.GetBlockBlobReference(seed.Id + ".jpg");

      // Create or overwrite the blob with contents from the phot byte array
      blockBlob.UploadFromByteArray(seed.ImageData, 0, seed.ImageData.Length);
      seed.ImageUrl = blockBlob.Uri.ToString();
      #endregion

      #region seed table

      // Generate the seed table name using  "Seed" and the YYMM portion (leftmost 4) if the ID
      string seedTableName = this.ConstructTableName(SeedEntityName, this.idManager.GetIdPrefix(seed.Id));

      // Get the table
      var seedTable = client.GetTableReference(seedTableName);
      
      // Add it if it's not there
      seedTable.CreateIfNotExists();

      TableOperation opInsertSeed = TableOperation.Insert(new SeedEntity(seed, this.idManager.GetIdCategory(seed.Id)));
      seedTable.Execute(opInsertSeed);
      #endregion

      #region seed hash table

      // Generate the table. Not temporal as this is an index. e.f. SB004qSeedHash
      string seedHashTableName = this.ConstructTableName(SeedHashEntityName, "");

      // Get the table
      var seedHashTable = client.GetTableReference(seedHashTableName);

      // Add it if it's not there
      seedHashTable.CreateIfNotExists();

      TableOperation opInsertHash = TableOperation.InsertOrReplace(new SeedHashEntity(seed, this.imageManager.ImageHashCategry(seed.ImageHash)));
      seedHashTable.Execute(opInsertHash);
      #endregion

      

      return seed;
    }
    /// <summary>
    /// Retrieve the seed id of a seed for a given hash
    /// </summary>
    /// <param name="seedImageHash"></param>
    /// <returns></returns>
    public ISeed GetSeedByHash(string seedImageHash)
    {
      // Generate the table. Not temporal as this is an index. e.f. SB004qSeedHash
      string seedHashTableName = this.ConstructTableName(SeedHashEntityName, "");

      // Get the table
      var seedHashTable = client.GetTableReference(seedHashTableName);

      TableOperation retOp = TableOperation.Retrieve<SeedHashEntity>(this.imageManager.ImageHashCategry(seedImageHash), seedImageHash);
      TableResult tr = seedHashTable.Execute(retOp);

      SeedHashEntity seedHashEntity = (SeedHashEntity)tr.Result;
      if (tr.Result != null)
      {
        return GetSeed(seedHashEntity.SeedId);
      }
     
      return null;
    }
    /// <summary>
    /// Retrieve the seed
    /// </summary>
    /// <param name="seedId"></param>
    /// <returns></returns>
    public ISeed GetSeed(string seedId)
    {
      // Generate the table using  "Seed" and the YYMM portion (leftmost 4) if the ID
      string seedTableName = this.ConstructTableName(SeedEntityName, this.idManager.GetIdPrefix(seedId));

      // Get the table
      var seedTable = client.GetTableReference(seedTableName);

      TableOperation retOp = TableOperation.Retrieve<SeedEntity>(this.idManager.GetIdCategory(seedId), seedId);
      TableResult tr = seedTable.Execute(retOp);

      SeedEntity seedEntity = (SeedEntity)tr.Result;
      if (tr.Result != null)
      {
        return new Seed
        {
          Id = seedEntity.RowKey,
          SourceImageUrl = seedEntity.SourceImageUrl,
          Width = seedEntity.Width,
          Height = seedEntity.Height,
          ImageHash = seedEntity.ImageHash
        };
      }
      return null;
    }

    /// <summary>
    /// Retrieve the image data of a speficied image id (could be a seed id or meme id or anything)
    /// </summary>
    /// <param name="imageId"></param>
    /// <returns></returns>
    public byte[] ImageBytes(string imageId)
    {
      IdType idType = this.idManager.GetIdType(imageId);
      string imageEntityName;
      switch (idType)
      {
          case IdType.Seed:
          imageEntityName = SeedImageEntityName;
          break;
          case IdType.Meme:
          imageEntityName = MemeImageEntityName;
          break;
        default:
          imageEntityName = UnknownImageEntityName;
          break;
      }
      // Generate the blob container using  "seedimage" or "memeimage" or "unknownimage" and the YYMM portion (leftmost 4) if the ID
      string seedImageContainerName = this.ConstructTableName(imageEntityName, this.idManager.GetIdPrefix(imageId));

      CloudBlobContainer imageContainer = blobClient.GetContainerReference(seedImageContainerName);
      imageContainer.CreateIfNotExists(BlobContainerPublicAccessType.Container);

      // Retrieve reference to a blob of the seed id
      CloudBlockBlob blockBlob = imageContainer.GetBlockBlobReference(imageId + ".jpg");

      // Create or overwrite the blob with contents from the phot byte array
      MemoryStream ms = new MemoryStream();
      try
      {
        blockBlob.DownloadRangeToStream(ms, null, null);
        return ms.ToArray();
      }
      catch
      {
        return new byte[] { };
      }
    }

    /// <summary>
    /// Table naming strategy is as follows [Environment][Table][YearMonth]
    /// e.g. SB004dSeed1501 => this respresents the SB004 application development environment "d", Seed table for Jan 2015
    /// This allows for Development, QA, Production all to reside in the cloud but be distinct and to 
    /// It also allows us to break up the entities into years and months for easy deleting after a period
    /// It is envisaged that the day of the month will provide the partition key  
    /// </summary>
    /// <param name="baseTableName">the underlying table entity name e.g. Meme or Seed</param>
    /// <param name="temporalPrefix">A year and month e.g. 1501 for Jan 2015 </param>
    /// <returns></returns>
    private string ConstructTableName(string baseTableName, string temporalPrefix)
    {
      // e.g. SB004dSeed1501 => [Environment][Table][YearMonth]
      return new StringBuilder(dataTablePrefix).Append(baseTableName).Append(temporalPrefix).ToString();
    }
  }
}