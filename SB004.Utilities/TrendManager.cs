namespace SB004.Utilities
{
  using System;

  using SB004.Domain;

  public class TrendManager : ITrendManager
  {
    private const double ScorePerDay = 7;
    private const double ScorePerHour = 7;

    private const double ScorePerView = 0.07;
    private const double ScorePerLike = 1;
    private const double ScorePerDislike = -0.5;
    private const double ScorePerComment = 1;
    private const double ScorePerReply = 1.25;


    /// <summary>
    /// Based on a fixed date (01/01/2015) in the past calculate a trend score 
    /// that increments per day and is complemented by activities
    /// </summary>
    /// <param name="meme"></param>
    /// <param name="userCommentCount"></param>
    /// <returns></returns>
    public double CalculateDailyTrendScore(IMeme meme, long userCommentCount)
    {
      // Add a fixed score to various activities performed on the meme
      return this.CalculateDailyTrendScore(meme, userCommentCount, new DateTime(2015, 01, 01));
    }
    /// <summary>
    /// Based on a supplied date in the past calculate a trend score 
    /// that increments per day and is complemented by activities
    /// </summary>
    /// <param name="meme"></param>
    /// <param name="userCommentCount"></param>
    /// <param name="baseDate"></param>
    /// <returns></returns>
    public double CalculateDailyTrendScore(IMeme meme, long userCommentCount, DateTime baseDate)
    {
      double trendScore = 0;

      // Add a fixed score for every day greater than the base date
      trendScore += (meme.DateCreated - baseDate).Days * ScorePerDay;

      // Add a fixed score to various activities performed on the meme
      return this.addActivityToTrendScore(meme, userCommentCount, trendScore);
    }
    
    /// <summary>
    /// Based on a supplied date in the past calculate a trend score 
    /// that increments per hour and is complemented by activities
    /// </summary>
    /// <param name="meme"></param>
    /// <param name="userCommentCount"></param>
    /// <param name="baseDate"></param>
    /// <returns></returns>
    public double CalculateHourlyTrendScore(IMeme meme, long userCommentCount, DateTime baseDate)
    {
      double trendScore = 0;

      // Add a fixed score for every hour since the base date
      trendScore += (meme.DateCreated - baseDate).Hours * ScorePerHour;

      // Add a fixed score to various activities performed on the meme
      return this.addActivityToTrendScore(meme, userCommentCount, trendScore);
    }
    /// <summary>
    /// Add a fixed score to various activities performed on the meme
    /// </summary>
    /// <param name="meme"></param>
    /// <param name="userCommentCount"></param>
    /// <param name="trendScore"></param>
    /// <returns></returns>
    private double addActivityToTrendScore(IMeme meme, long userCommentCount, double trendScore)
    {
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
