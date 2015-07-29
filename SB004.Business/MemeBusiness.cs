namespace SB004.Business
{
  using System;
  using System.Collections.Generic;

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
      meme.TrendScore = trendManager.CalculateDailyTrendScore(meme, userCommentCount);

      // Fall back on adding the text manually to the seed if mem image not supplied by client
      if (meme.ImageData == null || meme.ImageData.Length == 0)
      {
        // Get the seed image
        byte[] seedData = repository.GetSeed(meme.SeedId).ImageData;

        // Add the meme comments to the seed image to make the meme
        meme.ImageData = imageManager.GenerateMemeImage(meme, seedData);
      }
      
      // If the meme is not a response to another meme then identify it as Top level
      meme.IsTopLevel = meme.ResponseToId != null;

      // Save the meme
      return repository.Save(meme);

    }

    /// <summary>
    /// Adds a reply (with an id) to the replies of the supplied meme.
    /// The reply trend and date added are calculated before appending to the meme
    /// </summary>
    /// <param name="meme"></param>
    /// <param name="replyMemeId"></param>
    /// <returns></returns>
    public IMeme AddReplyToMeme(IMeme meme, string replyMemeId)
    {
      IMeme replyMeme = repository.GetMeme(replyMemeId);
      if (replyMeme != null)
      {
        if (meme.ReplyIds == null)
        {
          meme.ReplyIds = new List<IReply>();
        }

        // Add the repy id to the top of the list of replies. Set an hourly trend
        meme.ReplyIds.Insert(0, new Reply
        {
          Id = replyMemeId,
          DateCreated = DateTime.UtcNow,
          TrendScore = trendManager.CalculateHourlyTrendScore(meme,0,meme.DateCreated)
        });

        // Save the meme
        meme = repository.Save(meme);
      }
      
      return meme;
    }
  }
}
