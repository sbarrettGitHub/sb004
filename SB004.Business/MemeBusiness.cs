namespace SB004.Business
{
  using SB004.Data;
  using SB004.Domain;
  using SB004.Utilities;

  public class MemeBusiness : IMemeBusiness
  {
    readonly IRepository repository;
    readonly IImageManager imageManager;
    readonly ITrendManager trendManager;

    public MemeBusiness(IRepository repository, IImageManager imageManager, ITrendManager trendManager)
    {
      this.repository = repository;
      this.imageManager = imageManager;
      this.trendManager = trendManager;
    }
    /// <summary>
    /// Calculate the trend score and save the meme
    /// </summary>
    /// <param name="meme"></param>
    /// <returns></returns>
    public IMeme SaveMeme(IMeme meme)
    {
      long userCommentCount = 0;
      
      // Get the number of user comments
      if (meme.Id != null)
      {
        userCommentCount = repository.GetUserCommentCount(meme.Id);
      }

      // Calculate the meme trend score
      meme.TrendScore = trendManager.CalculateTrendScore(meme, userCommentCount);

      // Fall back on adding the text manually to the seed if mem image not supplied by client
      if (meme.ImageData == null || meme.ImageData.Length == 0)
      {
        // Get the seed image
        byte[] seedData = repository.GetSeed(meme.SeedId).ImageData;

        // Add the meme comments to the seed image to make the meme
        meme.ImageData = imageManager.GenerateMemeImage(meme, seedData);
      }

      // Save the meme
      return repository.Save(meme);

    }
  }
}
