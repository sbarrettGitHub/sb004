using SB004.Data;
using SB004.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PasswordHash;

namespace SB004.Business
{
    public class AccountBusiness :IAccountBusiness
    {
        readonly IRepository repository;
        public AccountBusiness(IRepository repository)
        {
            this.repository = repository;
        }
        public IUser CreateNewUser(IUser newUser) 
        {
            return repository.Save(newUser);
        }
        public IUser CreateNewUserAccount(IUser newUser, ICredentials newUserCredentials)
        {
            // Save the user
            newUser = repository.Save(newUser);

            // Create a Hash and unique salt for the new password
            string[] hashDetail = PasswordHash.PasswordHash.CreateHash(newUserCredentials.Password).Split(':');
            newUserCredentials.Password = hashDetail[1];
            newUserCredentials.Salt = hashDetail[2];
            newUserCredentials.Id = newUser.Id;

            return newUser;
        }
    }
}
