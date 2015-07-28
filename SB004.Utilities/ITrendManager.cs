namespace SB004.Utilities
{
  using SB004.Domain;

  public interface ITrendManager
  {
    /// <summary>
    /// 
    /// </summary>
    /// <param name="meme"></param>
    /// <param name="userCommentCount"></param>
    /// <returns></returns>
    double CalculateTrendScore(IMeme meme, long userCommentCount);
  }
}