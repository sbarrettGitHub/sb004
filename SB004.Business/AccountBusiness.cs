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
            if (newUser.AuthenticationProvider == null && newUser.Email != null)
            {
                if (repository.GetCredentials(newUser.Email) != null) 
                {
                    throw new UserAlreadyRegisteredException();
                }
            }
            return repository.Save(newUser);
        }
        public IUser SignUp(IUser newUser, ICredentials newUserCredentials)
        {
            // Save the user
            newUser = CreateNewUser(newUser);

            // Create a Hash and unique salt for the new password
            newUserCredentials.Password = PasswordHash.PasswordHash.CreateHash(newUserCredentials.Password);
            newUserCredentials.Id = newUser.Id;
			
			// Save the credentials
			repository.Save(newUserCredentials);
			
            return newUser;
        }
        /// <summary>
        /// Validation the email and password. If valid return the appropriate user
        /// </summary>
        /// <param name="email"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        public IUser SignIn(string email, string password)
        {
            ICredentials userCredentials = repository.GetCredentials(email);
            if (userCredentials != null && PasswordHash.PasswordHash.ValidatePassword(password, userCredentials.Password))
            {
                return repository.GetUser(userCredentials.Id, true);
            }

            throw new InvalidEmailOrPasswordException();
        }
    }
}
