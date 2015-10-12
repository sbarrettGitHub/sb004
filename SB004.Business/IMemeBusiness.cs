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

    /// <summary>
    /// Update the parent meme to reflect the new state of the supplied meme 
    /// Recalculate and update the reply trend score of the supplied meme inside the reply list of the parent meme (identified by ResponseToId)
    /// </summary>
    /// <param name="replyMeme"></param>
    /// <returns></returns>
    void UpdateReplyToMeme(IMeme replyMeme);

    /// <summary>
    /// Repost the specified meme to the specified user
    /// </summary>
    /// <param name="meme"></param>
    /// <returns></returns>
    IMeme RepostMeme(IMeme meme, IUser user);

    /// <summary>
    /// Report the specified meme as offensive by the specified user
    /// </summary>
    /// <param name="meme"></param>
    /// <param name="objection"></param>
    /// <param name="user"></param>
    /// <returns></returns>
    IMeme ReportMeme(IMeme meme, string objection,IUser user);
    
    /// <summary>
    /// Apply business rules around the update of meme interaction 
    /// </summary>
    /// <param name="id"></param>
    /// <param name="likesIncrement"></param>
    /// <param name="dislikesIncrement"></param>
    /// <param name="viewsIncrement"></param>
    /// <param name="sharesIncrement"></param>
    /// <param name="favouritesIncrement"></param>
    /// <returns></returns>
    IMeme UpdateMemeInteraction(string id, int likesIncrement, int dislikesIncrement, int viewsIncrement, int sharesIncrement, int favouritesIncrement);

  }
}