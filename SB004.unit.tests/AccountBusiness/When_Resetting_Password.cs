

using System;

namespace SB004.Unit.Tests.Business.AccountBusiness
{
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using NSubstitute;
    using SB004.Business;
    using SB004.Data;
    using SB004.Domain;

    [TestClass]
    public class When_Resetting_Password
    {
        private IRepository repository;
        private INotification notification;
        private IAccountBusiness accountBusiness;
        private IConfiguration configuration;
        private IPasswordBusiness passwordBusiness;

        [TestInitialize]
        public void Initialize()
        {
            repository = Substitute.For<IRepository>();
            notification = Substitute.For<INotification>();
            configuration = Substitute.For<IConfiguration>();
            passwordBusiness = Substitute.For<IPasswordBusiness>();

            accountBusiness = new AccountBusiness(repository, notification, configuration, passwordBusiness);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidPasswordResetTokenException))]
        public void Should_Validate_PasswordResetToken_Is_Genuine()
        {
            // Arrange
            repository.GetCredentialsByResetToken(Arg.Any<string>()).Returns((ICredentials)null);
            
            // Act
            accountBusiness.ResetPassword("NewPassword", "resetToken");

        }

        [TestMethod]
        [ExpectedException(typeof(ExpiredPasswordResetTokenException))]
        public void Should_Validate_PasswordResetToken_Is_Not_Expired()
        {
            // Arrange
            repository.GetCredentialsByResetToken(Arg.Any<string>()).Returns(new Credentials
            {
                // Date in the past
                ResetTokenExpiryDate = new DateTime(2000, 01, 01)
            });

            // Act
            accountBusiness.ResetPassword("NewPassword", "resetToken");
        }

        [TestMethod]
        [ExpectedException(typeof(UserNotActiveException))]
        public void Should_Validate_User_Account_Is_Active()
        {
            // Arrange
            repository.GetCredentialsByResetToken(Arg.Any<string>()).Returns(new Credentials
            {
                // Date NOT in the past
                ResetTokenExpiryDate = new DateTime(2100, 01, 01)
            });
            repository.GetUser(Arg.Any<string>()).Returns(new User { Active = false });

            // Act
            accountBusiness.ResetPassword("NewPassword", "resetToken");
        }

        [TestMethod]
        public void Should_Hash_Users_NewPassword()
        {
            // Arrange
            repository.GetCredentialsByResetToken(Arg.Any<string>()).Returns(new Credentials
            {
                // Date NOT in the past
                ResetTokenExpiryDate = new DateTime(2100, 01, 01)
            });
            repository.GetUser(Arg.Any<string>()).Returns(new User { Active = true });

            // Act
            accountBusiness.ResetPassword("NewPassword", "resetToken");

            // Assert
            passwordBusiness.Received().Hash("NewPassword");
        }

        [TestMethod]
        public void Should_Update_User_Password_In_Repository()
        {
            // Arrange
            repository.GetCredentialsByResetToken( Arg.Any<string>()).Returns(new Credentials
            {
                // Date NOT in the past
                ResetTokenExpiryDate = new DateTime(2100, 01, 01)
            });
            repository.GetUser(Arg.Any<string>()).Returns(new User { Active = true });

            // Act
            accountBusiness.ResetPassword("NewPassword", "resetToken");

            // Assert
            repository.Received().Save(Arg.Any<ICredentials>());
        }
        [TestMethod]
        public void Should_Enforce_Password_Strength_Rules()
        {
            // Arrange
            repository.GetCredentialsByResetToken(Arg.Any<string>()).Returns(new Credentials
            {
                // Date NOT in the past
                ResetTokenExpiryDate = new DateTime(2100, 01, 01)
            });
            repository.GetUser(Arg.Any<string>()).Returns(new User { Active = true });

            // Act
            accountBusiness.ResetPassword("NewPassword", "resetToken");

            // Assert
            passwordBusiness.Received().EnforcePasswordStengthRules("NewPassword");
        }
    }
}
