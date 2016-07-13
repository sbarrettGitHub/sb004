using System;
using SB004.Domain;
namespace SB004.Business
{
    public interface IAccountBusiness
    {
        IUser CreateNewUser(IUser newUser);
        IUser SignUp(IUser newUser, ICredentials newUserCredentials);
        IUser SignIn(string email, string password);
	    void ChangePassword(IUser user, ICredentials newCredentials);
	    IUser Follow(string userId, string followedId);
	    IUser Unfollow(string userId, string followedId);
        IUser ForgotPassword(string emailAddress);
        IUser ResetPassword(string newPassword, string resetToken);
    }
}
