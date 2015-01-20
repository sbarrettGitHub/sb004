using SB004.Business;
using SB004.Domain;
namespace SB004.Data
{
  using System.Globalization;
  using System.Web;

  public class Repository : IRepository
  {
    IIdManager IdManager { get; set; }

    public Repository(IIdManager idManager)
    {
      this.IdManager = idManager;
    }
    /// <summary>
    /// Persist the spuulied seed and assign an ID
    /// </summary>
    /// <param name="seed"></param>
    /// <returns></returns>
    public ISeed AddSeed(ISeed seed)
    {
      // Generate a seed id
      seed.Id = this.IdManager.NewId();

      // Save the seed
      HttpContext.Current.Cache.Insert("seed_" + seed.Id, seed);

      // Save the seed image
      HttpContext.Current.Cache.Insert("image_" + seed.Id, seed.ImageData);

      // Save the seed hash
      HttpContext.Current.Cache.Insert("hash_" + seed.ImageHash, seed.Id);

      return seed;
    }
    /// <summary>
    /// Retrieve the seed id of a seed for a given hash
    /// </summary>
    /// <param name="seedImageHash"></param>
    /// <returns></returns>
    public string GetSeedIdByHash(string seedImageHash)
    {
      string seedId = string.Empty;
      object data = HttpContext.Current.Cache.Get("hash_" + seedImageHash);
      if (data != null)
      {
        seedId = (string)data;
      }
      
      return seedId;
    }
    /// <summary>
    /// Retrieve the seed
    /// </summary>
    /// <param name="seedId"></param>
    /// <returns></returns>
    public ISeed GetSeed(string seedId)
    {
      ISeed seed = null;
      object data = HttpContext.Current.Cache.Get("seed_" + seedId);
      if (data != null)
      {
        seed = (ISeed)data;
      }
      return seed;
    }

    /// <summary>
    /// Retrieve the image data of a speficied image id (could be a seed id or meme id or anything)
    /// </summary>
    /// <param name="imageId"></param>
    /// <returns></returns>
    public byte[] ImageBytes(string imageId)
    {
      object data = HttpContext.Current.Cache.Get("image_" + imageId);

      if (data != null)
      {
        return (byte[])data;
      }
      return null;
    }
  }
}