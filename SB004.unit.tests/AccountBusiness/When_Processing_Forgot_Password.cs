namespace SB004.Unit.Tests.Business.AccountBusiness
{
    using System;
    using System.CodeDom;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using NSubstitute;
    using SB004.Business;
    using SB004.Data;
    using SB004.Domain;  

    [TestClass]
    public class When_Processing_Forgot_Password
    {
        private IRepository repository;
        private INotification notification;
        private IAccountBusiness accountBusiness;
        private IConfiguration configuration;
        private IPasswordBusiness passwordBusiness;

        /// <summary>
        /// Initializes the dependencies fo the account business.
        /// </summary>
        [TestInitialize]
        public void Initialize()
        {
            repository = Substitute.For<IRepository>();
            notification = Substitute.For<INotification>();
            configuration = Substitute.For<IConfiguration>();
            passwordBusiness = Substitute.For<IPasswordBusiness>();

            accountBusiness = new AccountBusiness(repository, notification, configuration, passwordBusiness);
        }

        /// <summary>
        /// Verifies that supplied email address is validated against registered emails.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(UnrecognizedEmailAddressException))]
        public void Should_Validate_UserSupplied_EmailAddress_Against_Registered_Emails()
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
        public void Should_Update_User_Credentials_With_Reset_Token()
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
        public void Should_Send_Password_Reset_Notification_To_User()
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
