

namespace SB004.Unit.Tests.AccountController
{
    using System.Net;
    using System.Web.Http;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using NSubstitute;
    using SB004.Models;

    [TestClass]
    public class When_Reset_Password_Link_Followed : AccountControllerTestBase
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
        public void Should_Validate_ResetToken_Supplied()
        {
            // Arrange
            ResetPasswordModel resetPasswordModel = new ResetPasswordModel
            {
                NewPassword = "pass1234",
                ResetToken = null
            };
            
            // Act
            try
            {
                accountController.Validate(resetPasswordModel);
                accountController.ResetPassword(resetPasswordModel);
            }
            catch (HttpResponseException exception)
            {
                Assert.IsTrue(exception.Response.StatusCode == HttpStatusCode.BadRequest, "Reset token not supplied. Expected Bad Request");
                return;
            }

            // Assert
            Assert.Fail("Expected Bad Request");
        }
        
        [TestMethod]
        public void Should_Validate_NewPassword_Supplied()
        {
            // Arrange
            ResetPasswordModel resetPasswordModel = new ResetPasswordModel
            {
                NewPassword = "",
                ResetToken = "ResetToken"
            };

            // Act
            try
            {
                accountController.Validate(resetPasswordModel);
                accountController.ResetPassword(resetPasswordModel);
            }
            catch (HttpResponseException exception)
            {
                Assert.IsTrue(exception.Response.StatusCode == HttpStatusCode.BadRequest, "Reset token not supplied. Expected Bad Request");
                return;
            }

            // Assert
            Assert.Fail("Expected Bad Request");
        }

        [TestMethod]
        public void Should_Engage_Business_Rules_ResetPassword()
        {
            // Arrange
            ResetPasswordModel resetPasswordModel = new ResetPasswordModel
            {
                NewPassword = "pass1234",
                ResetToken = "resetToken"
            };

            // Act
            accountController.Validate(resetPasswordModel);
            accountController.ResetPassword(resetPasswordModel);

            // Assert
            accountBusiness.Received().ResetPassword(resetPasswordModel.NewPassword, resetPasswordModel.ResetToken);
        }
    }
}
