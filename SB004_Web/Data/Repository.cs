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
      return seedId;
    }

    public ISeed GetSeed(string seedId)
    {
      return null;
    }

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