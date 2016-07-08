using System;
using System.CodeDom;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;
using SB004.Business;
using SB004.Data;
using SB004.Domain;

namespace SB004.Unit.Tests.AccountBusiness
{
    

    [TestClass]
    public class WhenUserForgetsPassword
    {
        private IRepository repository;
        private INotification notification;
        private IAccountBusiness accountBusiness;
        private IConfiguration configuration;

        /// <summary>
        /// Initializes the dependencies fo the account business.
        /// </summary>
        [TestInitialize]
        public void Initialize()
        {
            repository = Substitute.For<IRepository>();
            notification = Substitute.For<INotification>();
            configuration = Substitute.For<IConfiguration>();

            accountBusiness = new Business.AccountBusiness(repository, notification, configuration);
        }

        /// <summary>
        /// Verifies that supplied email address is validated against registered emails.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(UnrecognizedEmailAddress))]
        public void UserSuppliedEmailAddressIsValidatedAgainstRegisteredEmails()
        {
            // Arrange
            repository.GetCredentials(Arg.Any<string>()).Returns((ICredentials)null);

            // Act
            accountBusiness.ForgotPassword("test@email.com");
        }

        /// <summary>
        /// Verify the user credentials are updated with reset token.
        /// </summary>
        [TestMethod]
        public void UserCredentialsUpdatedWithResetToken()
        {
            // Arrange
            repository.GetCredentials(Arg.Any<string>()).Returns(new Credentials { Email = "test@email.com" });

            // Act
            accountBusiness.ForgotPassword("test@email.com");

            // Assert
            repository.Received().SetResetToken(Arg.Any<ICredentials>(), Arg.Any<int>());
        }

        /// <summary>
        /// Verifies that a passwords reset notification is sent to user.
        /// </summary>
        [TestMethod]
        public void PasswordResetNotificationSentToUser()
        {
            // Arrange
            repository.GetCredentials(Arg.Any<string>()).Returns(new Credentials { Email = "test@email.com" });

            // Act
            accountBusiness.ForgotPassword("test@email.com");

            // Assert
            notification.Received().NotifyResetPassword(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<string>(), Arg.Any<string>());
        }
    }
}
