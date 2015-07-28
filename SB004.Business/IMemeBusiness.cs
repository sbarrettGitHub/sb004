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
  }
}