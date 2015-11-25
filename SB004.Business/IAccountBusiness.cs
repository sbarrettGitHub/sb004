using System;
using SB004.Domain;
namespace SB004.Business
{
    public interface IAccountBusiness
    {
        IUser CreateNewUser(IUser newUser);
        IUser SignUp(IUser newUser, ICredentials newUserCredentials);
        IUser SignIn(string email, string password);
    }
}
