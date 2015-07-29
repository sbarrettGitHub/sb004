namespace SB004.Utilities
{
  using System;

  using SB004.Domain;

  public interface ITrendManager
  {
    /// <summary>
    /// Based on a fixed date in the past calculate a trend score 
    /// that increments per day and is complemented by activities
    /// </summary>
    /// <param name="meme"></param>
    /// <param name="userCommentCount"></param>
    /// <returns></returns>
    double CalculateDailyTrendScore(IMeme meme, long userCommentCount);

    /// <summary>
    /// Based on a supplied date in the past calculate a trend score 
    /// that increments per day and is complemented by activities
    /// </summary>
    /// <param name="meme"></param>
    /// <param name="userCommentCount"></param>
    /// <param name="baseDate"></param>
    /// <returns></returns>
    double CalculateDailyTrendScore(IMeme meme, long userCommentCount, DateTime baseDate);

    /// <summary>
    /// Based on a supplied date in the past calculate a trend score 
    /// that increments per hour and is complemented by activities
    /// </summary>
    /// <param name="meme"></param>
    /// <param name="userCommentCount"></param>
    /// <param name="baseDate"></param>
    /// <returns></returns>
    double CalculateHourlyTrendScore(IMeme meme, long userCommentCount, DateTime baseDate);
  }
}