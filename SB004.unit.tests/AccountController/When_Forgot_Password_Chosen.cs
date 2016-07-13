



namespace SB004.Unit.Tests.AccountController
{
    using System.Net;
    using System.Web.Http;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using NSubstitute;
    using Controllers;
    using Models;

    [TestClass]
    public class When_Forgot_Password_Chosen: AccountControllerTestBase
    {

        /// <summary>
        /// Initializes the dependencies fo the account controller.
        /// </summary>
        [TestInitialize]
        public void InitializeTest()
        {
            Initialize();
        }

        [TestMethod]
        public void Should_Validate_Email_Supplied()
        {
            // Arrange
            AccountDetailsModel detailsWithOutEmail = new AccountDetailsModel();

            // Act
            try
            {
                accountController.ForgotPassword(detailsWithOutEmail);
            }
            catch (HttpResponseException exception)
            {

                Assert.IsTrue(exception.Response.StatusCode == HttpStatusCode.BadRequest, "Email not supplied. Expected Bad Request");
                return;
            }

            Assert.Fail("Expected Bad Request");
        }

        [TestMethod]
        public void Should_Engage_Business_Rules_ForgotPassword()
        {
            // Arrange
            string testEmail = "test@email.com";
            AccountDetailsModel detailsWithOutEmail = new AccountDetailsModel{ Email = testEmail};

            // Act
            accountController.ForgotPassword(detailsWithOutEmail);
            
            // Assert
            accountBusiness.Received().ForgotPassword(testEmail);

        }
    }
}
