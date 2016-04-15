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

        ICredentials Save(ICredentials credentials);
        
        ICredentials GetCredentials(string email);

        IUser GetUser(string authenticationUserId, string authenticationProvider);

        IUser GetUser(string userId, bool deep = false);

        List<IUserComment> GetUserComments(string memeId, int skip, int take);

        long GetUserCommentCount(string memeId);

        IUserComment Save(IUserComment userComment);

        IUserComment GetUserComment(string userCommentId);

        IMeme Save(IRepost repost);

        IReport Save(IReport report);

        List<IMeme> SearchMemeByUser(string userId, int skip, int take, out long fullCount);

		IImage Save(IImage image);

        IImage GetImage(string id);
	    
		void DeleteImage(string id);

        ITimeLine Save(ITimeLine timeLine);

		List<ITimeLine> GetUserTimeLine(string userId, int skip, int take, TimeLineEntry type);

		List<ITimeLine> GetUserTimeLine(string userId, int days);

	    List<ITimeLine> GetUserMemeTimeLine(string userId, int days);
    }
}
