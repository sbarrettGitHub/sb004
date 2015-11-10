using System;
using SB004.Domain;
namespace SB004.Business
{
    public interface IAccountBusiness
    {
        IUser CreateNewUser(IUser newUser);
        IUser CreateNewUserAccount(IUser newUser, ICredentials newUserCredentials);
    }
}
