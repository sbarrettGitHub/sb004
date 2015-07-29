namespace SB004.Business
{
  using SB004.Domain;

  public interface IMemeBusiness
  {
    /// <summary>
    /// Calculate the trend score and save the meme
    /// </summary>
    /// <param name="meme"></param>
    /// <returns></returns>
    IMeme SaveMeme(IMeme meme);
    /// <summary>
    /// Adds a reply (with an id) to the replies of the supplied meme.
    /// The reply trend and date added are calculated before appending to the meme
    /// </summary>
    /// <param name="meme"></param>
    /// <param name="replyMemeId"></param>
    /// <returns></returns>
    IMeme AddReplyToMeme(IMeme meme, string replyMemeId);
  }
}