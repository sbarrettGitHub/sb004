namespace SB004.Unit.Tests.Business.AccountBusiness
{
    using SB004.Business;
    using Data;
    using Domain;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using NSubstitute;

    /// <summary>
    /// Test the rules around signing a new user up
    /// </summary>
    [TestClass]
    public class When_Signing_Up
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

        /// <summary>
        /// Test that a new user with a duplicate email address is rejected.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(UserAlreadyRegisteredException))]
        public void Should_Reject_Duplicate_EmailAddress()
        {
            // Arrange
            repository.GetCredentials(Arg.Any<string>()).Returns(new Credentials { Email = "test@email.com" });
            IUser newUser = NewTestUser();
            ICredentials credentials = NewTestCredentials();

            // Act
            accountBusiness.SignUp(newUser, credentials);
        }

        /// <summary>
        /// Test that the new user is sent a welcome email.
        /// </summary>
        [TestMethod]
        public void Should_Send_New_User_A_Welcome_Email()
        {
            // Arrange
            repository.GetCredentials(Arg.Any<string>()).Returns((ICredentials)null);
            IUser newUser = NewTestUser();
            ICredentials credentials = NewTestCredentials();

            // Act
            accountBusiness.SignUp(newUser, credentials);

            // Assert
            notification.Received().NotifyWelcome(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<string>());
        }

        /// <summary>
        /// Test that credentials are added to repository.
        /// </summary>
        [TestMethod]
        public void Should_Add_Credentials_To_Repository()
        {
            // Arrange
            repository.GetCredentials(Arg.Any<string>()).Returns((ICredentials)null);
            IUser newUser = NewTestUser();
            ICredentials credentials = NewTestCredentials();

            // Act
            accountBusiness.SignUp(newUser, credentials);

            // Assert
            repository.Received().Save(Arg.Any<ICredentials>());
        }

        /// <summary>
        /// Test that a user is added to repository.
        /// </summary>
        [TestMethod]
        public void Should_Add_User_To_Repository()
        {
            // Arrange
            repository.GetCredentials(Arg.Any<string>()).Returns((ICredentials)null);
            IUser newUser = NewTestUser();
            ICredentials credentials = NewTestCredentials();

            // Act
            accountBusiness.SignUp(newUser, credentials);

            // Assert
            repository.Received().Save(Arg.Any<IUser>());
        }

        /// <summary>
        /// New test credentials.
        /// </summary>
        /// <returns></returns>
        private Credentials NewTestCredentials()
        {
            return new Credentials { Email = "test@email.com", Password = "pass1234" };
        }

        /// <summary>
        /// New test user.
        /// </summary>
        /// <returns></returns>
        private User NewTestUser()
        {
            return new User { UserName = "Test Name", Email = "test@email.com" };
        }
    }
}
