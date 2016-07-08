
using System;

namespace SB004.Business
{
    public interface INotification
    {

        /// <summary>
        /// Notifies the welcome.
        /// </summary>
        /// <param name="userId">The user identifier.</param>
        /// <param name="emailAddress">The email address.</param>
        /// <param name="firstName">The first name.</param>
        void NotifyWelcome(string userId, string emailAddress, string firstName);

        void NotifyResetPassword(string userId, string emailAddress, string name, string resetToken);
    }
}
