namespace SB004.Data
{
  using System.Collections.Generic;

  using SB004.Domain;

    public interface IRepository
    {
        ISeed Save(ISeed seed);

        ISeed GetSeedByHash(string seedImageHash);

        ISeed GetSeed(string seedId);

        IMeme Save(IMeme meme);

        IMeme GetMeme(string memeId);

        List<IMeme> SearchMeme(int skip, int take);

        List<IMeme> SearchTrendingMemes(int skip, int take);

        IUser Save(IUser user);
        
        IUser GetUser(string authenticationUserId, string authenticationProvider);
        
        IUser GetUser(string userId);

        List<IUserComment> GetUserComments(string memeId, int skip, int take);

        long GetUserCommentCount(string memeId);

        IUserComment Save(IUserComment userComment);

        IUserComment GetUserComment(string userCommentId);

        IMeme Save(IRepost repost);

        IReport Save(IReport report);

        List<IMeme> SearchMemeByUser(string userId, int skip, int take, out long fullCount);
    }
}
