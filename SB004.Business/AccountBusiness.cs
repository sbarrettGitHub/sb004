namespace SB004.Business
{
    using System;
    using System.Collections.Generic;
    using SB004.Data;
    using SB004.Domain;
    public class AccountBusiness :IAccountBusiness
    {
        readonly IRepository repository;
        readonly INotification notificationService;
        readonly IConfiguration configuration;
        private readonly IPasswordBusiness passwordBusiness;

        public AccountBusiness(IRepository repository, INotification notificationService, IConfiguration configuration, IPasswordBusiness passwordBusiness)
        {
            this.repository = repository;
            this.notificationService = notificationService;
            this.configuration = configuration;
            this.passwordBusiness = passwordBusiness;
        }

        /// <summary>
        /// Creates the new user.
        /// </summary>
        /// <param name="newUser">The new user.</param>
        /// <returns></returns>
        /// <exception cref="UserAlreadyRegisteredException">Thrown if the user is already registered</exception>
        public IUser CreateNewUser(IUser newUser) 
        {
            if (newUser.AuthenticationProvider == null && newUser.Email != null)
            {
                if (repository.GetCredentials(newUser.Email) != null) 
                {
                    throw new UserAlreadyRegisteredException();
                }
            }

            newUser.Active = true;

            return repository.Save(newUser);
        }

        /// <summary>
        /// Signs a new user up. Adds new user to repository, creates hased password and notifies the user by mail
        /// </summary>
        /// <param name="newUser">The new user.</param>
        /// <param name="newUserCredentials">The new user credentials.</param>
        /// <returns></returns>
        public IUser SignUp(IUser newUser, ICredentials newUserCredentials)
        {
            // Ensure password is strong enough
            passwordBusiness.EnforcePasswordStengthRules(newUserCredentials.Password);

            // Save the user
            newUser = CreateNewUser(newUser);

            // Create a Hash and unique salt for the new password
            newUserCredentials.Password = passwordBusiness.Hash(newUserCredentials.Password);
            newUserCredentials.Id = newUser.Id;
			
			// Save the credentials
			repository.Save(newUserCredentials);
			
            // Notify the user and welcome
            notificationService.NotifyWelcome(newUser.Id, newUser.Email, newUser.UserName);

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
            if (userCredentials != null && passwordBusiness.ValidatePassword(password, userCredentials.Password))
            {
                IUser user = repository.GetUser(userCredentials.Id, true);
	            if (user.Active)
	            {
		            return user;
	            }
            }

            throw new InvalidEmailOrPasswordException();
        }
		/// <summary>
		/// Change teh credentials of the spuulied user
		/// </summary>
		/// <param name="user"></param>
		/// <param name="newCredentials"></param>
		public void ChangePassword(IUser user, ICredentials newCredentials)
	    {
            // Ensure password is strong enough
            passwordBusiness.EnforcePasswordStengthRules(newCredentials.Password);

			// Create a Hash and unique salt for the new password
            newCredentials.Password = passwordBusiness.Hash(newCredentials.Password);
			newCredentials.Id = user.Id;

			// Save the credentials
			repository.Save(newCredentials);
	    }
		/// <summary>
		/// Add a user (followed) as a member of a user's (follower's) followed list
		/// </summary>
		/// <param name="userId"></param>
		/// <param name="followedId"></param>
		public IUser Follow(string userId, string followedId)
		{
			IUser user = repository.GetUser(userId);
			IUser followed = repository.GetUser(followedId);
			if (user != null)
			{
				if (user.FollowingIds == null)
				{
					user.FollowingIds = new List<IUserLite>();
				}

				// Remove the id if is already there, because it is being added to the front
				user.FollowingIds.RemoveAll(x => x.Id == followedId);
				
				// Insert at the beginning
				if (followed != null)
				{
					user.FollowingIds.Insert(0, new UserLite {Id = followed.Id, UserName = followed.UserName});

					// Save 
					user = repository.Save(user);

					// Update followedBy list of the followed
					followed.FollowedByIds.Insert(0, new UserLite { Id = user.Id, UserName = user.UserName });

					// Save 
					repository.Save(followed);
				}
				else
				{
					throw new ArgumentException("Followed user does not exist");
				}

				

				return user;
			}
			throw new ArgumentException("User does not exist");
		}
		/// <summary>
		/// Remove a user (followed) as a member from a user's (follower's) followed list
		/// </summary>
		/// <param name="userId"></param>
		/// <param name="followedId"></param>
		public IUser Unfollow(string userId, string followedId)
		{
			IUser user = repository.GetUser(userId);
			IUser followed = repository.GetUser(followedId); 
			if (user != null)
			{
				if (user.FollowingIds == null)
				{
					return user;
				}

				// Remove the id from th elist of followers
				user.FollowingIds.RemoveAll(x => x.Id == followedId);

				// Save 
				repository.Save(user);
				
				// Update followedBy list
				if (followed != null)
				{
					followed.FollowedByIds.RemoveAll(x => x.Id == userId);
					// Save 
					repository.Save(followed);
				}
				

				return user;
			}
			throw new ArgumentException("User does not exist");
		}

        /// <summary>
        /// Flag password of the user as identified by email address for reset
        /// </summary>
        /// <param name="emailAddress">The email address.</param>
        /// <exception cref="InvalidEmailOrPasswordException"></exception>
        public IUser ForgotPassword(string emailAddress)
        {
            // Get the credentials record of the user identified by the email adddress
            ICredentials userCredentials = repository.GetCredentials(emailAddress);
            
            // Raise exception if email address unrecognized
            if (userCredentials == null)
            {
                throw new UnrecognizedEmailAddressException();    
            }
            
            // Set the password reset token on the credentials
            userCredentials = repository.SetResetToken(userCredentials, configuration.ResetTokenDaysToExpire);

            // Get the user 
            IUser user = repository.GetUser(userCredentials.Id);

            // Notify the user of how to reset his/her password
            notificationService.NotifyResetPassword(user.Id,user.Email,user.UserName,userCredentials.ResetToken);

            return user;
        }

        /// <summary>
        /// Resets the password.
        /// </summary>
        /// <param name="newPassword">The new password.</param>
        /// <param name="resetToken">The reset token.</param>
        public IUser ResetPassword(string newPassword, string resetToken)
        {
            // Get the credentials record for the given reset token and email address
            ICredentials credentials = repository.GetCredentialsByResetToken(resetToken);

            // Are the reset token and email address valid?
            if (credentials == null)
            {
                throw new InvalidPasswordResetTokenException();
            }

            // Is the reset token expired?
            if (credentials.ResetTokenExpiryDate.CompareTo(DateTime.Now) < 0)
            {
                throw new ExpiredPasswordResetTokenException();
            }

            // Get the user
            IUser user = repository.GetUser(credentials.Id);

            // Verify that the user exists 
            if (user == null)
            {
                throw new UserNotFoundException();
            }

            // Verify that the user is active
            if (user.Active == false)
            {
                throw new UserNotActiveException();
            }

            // Ensure password is strong enough
            passwordBusiness.EnforcePasswordStengthRules(newPassword);

            // Create password hash for new password
            credentials.Password = passwordBusiness.Hash(newPassword);

            // Remove the reset token
            credentials.ResetToken = null;

            // Save the credentials
            repository.Save(credentials);

            return user;
        }
    }
}
