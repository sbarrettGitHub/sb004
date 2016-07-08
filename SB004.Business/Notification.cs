using System;

namespace SB004.Business
{
    using System.Net;
    using System.Net.Mail;
    using System.Text;
    using Domain;
    using SB004.Business.Properties;
    using SB004.Data;

    public class Notification : INotification
    {

        readonly IConfiguration configuration;
        readonly IRepository repository;

        public Notification(IConfiguration configuration, IRepository repository)
        {
            this.configuration = configuration;
            this.repository = repository;
        }

        /// <summary>
        /// Welcomes the specified user to "sb004".
        /// </summary>
        /// <param name="userId">The user identifier.</param>
        /// <param name="emailAddress">The email address.</param>
        /// <param name="firstName">The first name.</param>
        public void NotifyWelcome(string userId, string emailAddress, string firstName)
        {
            string subject = MailTemplates.welcome_subject;

            string body = MailTemplates.welcome_body;

            subject = transformPlaceholders(subject, name: firstName);

            body = transformPlaceholders(body, name: firstName, homePageUrl: this.configuration.RootUrl, systemName: this.configuration.SystemName);

            Notify(new Mail(emailAddress, subject, body));
        }

        /// <summary>
        /// Notifies the user with a reset password mail.
        /// </summary>
        /// <param name="userId">The user identifier.</param>
        /// <param name="emailAddress">The email address.</param>
        /// <param name="name">The name.</param>
        /// <param name="resetToken">The reset token.</param>
        public void NotifyResetPassword(string userId, string emailAddress, string name, string resetToken)
        {
            string subject = MailTemplates.forgotPassword_subject;

            string body = MailTemplates.forgotPassword_body;

            subject = transformPlaceholders(subject, name: name, systemName: this.configuration.SystemName);

            body = transformPlaceholders(body, name: name, 
                                                homePageUrl: this.configuration.RootUrl, 
                                                systemName: this.configuration.SystemName, 
                                                resetPassordAction: "api/account/resetpassword", 
                                                userId: userId, 
                                                resetToken: resetToken);

            Notify(new Mail(emailAddress, subject, body));
        }

        /// <summary>
        /// Adds the specified message to the repository. A mailing service wil pick it up and delivery out of process
        /// </summary>
        /// <param name="message">The message.</param>
        /// <returns></returns>
        private IMail Notify(IMail message)
        {
            return repository.Save(message);
        }

        /// <summary>
        /// Transforms the placeholders.
        /// </summary>
        /// <param name="content">The content.</param>
        /// <param name="name">The name.</param>
        /// <param name="homePageUrl">The home page URL.</param>
        /// <param name="systemName">Name of the system.</param>
        /// <param name="userId"></param>
        /// <param name="resetPassord"></param>
        /// <param name="resetToken"></param>
        /// <returns></returns>
        private string transformPlaceholders(string content, string name = null, string homePageUrl = null, string systemName = null, string userId = null, string resetPassordAction = null, string resetToken = null)
        {
            string transformedContents = content;
            StringBuilder transformedContent = new StringBuilder(content);

            if (name != null)
            {
                transformedContent = transformedContent.Replace("{{name}}", name);
            }

            if (homePageUrl != null)
            {
                transformedContent = transformedContent.Replace("{{homePageUrl}}", homePageUrl);
            }

            if (systemName != null)
            {
                transformedContent = transformedContent.Replace("{{systemName}}", systemName);
            }

            if (userId != null)
            {
                transformedContent = transformedContent.Replace("{{userId}}", systemName);
            }

            if (resetPassordAction != null)
            {
                transformedContent = transformedContent.Replace("{{resetPassordAction}}", resetPassordAction);
            }

            if (resetToken != null)
            {
                transformedContent = transformedContent.Replace("{{resetToken}}", resetToken);
            }    
        
            return transformedContent.ToString();
        }
    }
}
