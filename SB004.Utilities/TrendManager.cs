namespace SB004.Utilities
{
  using System;

  using SB004.Domain;

  public class TrendManager : ITrendManager
  {
    private const double ScorePerDay = 7;
    private const double ScorePerView = 0.07;
    private const double ScorePerLike = 1;
    private const double ScorePerDislike = -0.5;
    private const double ScorePerComment = 1;
    private const double ScorePerReply = 1;

    /// <summary>
    /// Calculate a score from all the data in the meme
    /// </summary>
    /// <param name="meme"></param>
    /// <param name="userCommentCount"></param>
    /// <returns></returns>
    public double CalculateTrendScore(IMeme meme, long userCommentCount)
    {
      double trendScore = 0;

      // Add a fixed score for every day greater than 01/01/2015
      trendScore += (meme.DateCreated - new DateTime(2015,01,01)).Days * ScorePerDay;
      
      // Add a fixed score for each view
      trendScore += meme.Views * ScorePerView;

      // Add a fixed score for each like
      trendScore += meme.Likes * ScorePerLike;

      // Add a fixed score for each dislike (a negative)
      trendScore += meme.Dislikes * ScorePerDislike;

      // Add a fixed score for each comment
      trendScore += userCommentCount * ScorePerComment;

      // Add a fixed score for each reply
      trendScore += meme.ReplyIds.Count * ScorePerReply;

      return trendScore;
    }
  }
}
