namespace SB004.Data
{
  using SB004.Domain;

  public interface IRepository
  {
    ISeed AddSeed(ISeed seed);

    string GetSeedIdByHash(string seedImageHash);

    ISeed GetSeed(string seedId);

    byte[] ImageBytes(string imageId);
  }
}
