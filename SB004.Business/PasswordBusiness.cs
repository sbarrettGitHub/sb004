using System.Text.RegularExpressions;
using SB004.Domain;

namespace SB004.Business
{
    public class PasswordBusiness : IPasswordBusiness
    {
        /// <summary>
        /// Hashes the specified password.
        /// </summary>
        /// <param name="password">The password.</param>
        /// <returns>Hashed password</returns>
        public string Hash(string password)
        {
            return PasswordHash.PasswordHash.CreateHash(password);
        }

        /// <summary>
        /// Hashes supplied password and validates the supplied  hash.
        /// </summary>
        /// <param name="password">The password.</param>
        /// <param name="correctHash">The correct hash.</param>
        /// <returns>true if valid, otherwise false</returns>
        public bool ValidatePassword(string password, string correctHash)
        {
            return PasswordHash.PasswordHash.ValidatePassword(password, correctHash);
        }

        /// <summary>
        /// Determines whether supplied password complies with password naming rules.
        /// At least 6 characters long and containing 1 number
        /// </summary>
        /// <param name="password">The password.</param>
        /// <returns></returns>
        public void EnforcePasswordStengthRules(string password)
        {
            if (password == null || password.Length < 6)
            {
                throw new PasswordTooShortException();    
            }
            
            if (Regex.IsMatch(password, ".*[0-9]+.*") == false)
            {
                throw new PasswordRequiresNumberException();
            }
        }
    }
}
