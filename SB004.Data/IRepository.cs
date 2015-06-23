namespace SB004.Data
{
  using System.Collections.Generic;

  using SB004.Domain;

    public interface IRepository
    {
        ISeed SaveSeed(ISeed seed);

        ISeed GetSeedByHash(string seedImageHash);

        ISeed GetSeed(string seedId);

        IMeme SaveMeme(IMeme meme);

        IMeme GetMeme(string memeId);

        List<IMeme> SearchMeme(int skip, int take);

        IUser SaveUser(IUser user);
        
        IUser GetUser(string authenticationUserId, string authenticationProvider);
        
        IUser GetUser(string userId);

        List<IUserComment> GetUserComments(string memeId, int skip, int take);

        long GetUserCommentCount(string memeId);

        IUserComment SaveUserComment(IUserComment userComment);

        IUserComment GetUserComment(string userCommentId);
    }
}
