namespace SB004.Data
{
  using SB004.Domain;

  public interface IRepository
  {
    ISeed AddSeed(ISeed seed);

    ISeed GetSeedByHash(string seedImageHash);

    ISeed GetSeed(string seedId);

    IMeme AddMeme(IMeme meme);

    IMeme GetMeme(string memeId);
  }
}
