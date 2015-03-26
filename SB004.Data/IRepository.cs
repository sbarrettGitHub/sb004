namespace SB004.Data
{
    using SB004.Domain;

    public interface IRepository
    {
        ISeed SaveSeed(ISeed seed);

        ISeed GetSeedByHash(string seedImageHash);

        ISeed GetSeed(string seedId);

        IMeme SaveMeme(IMeme meme);

        IMeme GetMeme(string memeId);

        IUser SaveUser(IUser user);
        
        IUser GetUser(string authenticationUserId, string AuthenticationProvider);
        
        IUser GetUser(string userId);
    }
}
