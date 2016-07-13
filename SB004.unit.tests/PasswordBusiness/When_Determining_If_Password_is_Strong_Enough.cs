using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SB004.Business;
using SB004.Domain;

namespace SB004.Unit.Tests.PasswordBusiness
{
    [TestClass]
    public class When_Determining_If_Password_is_Strong_Enough
    {
        [TestMethod]
        [ExpectedException(typeof(PasswordTooShortException))]
        public void Should_Validate_That_Password_Is_At_Least_6_Characters_Long()
        {
            // Arrange
            IPasswordBusiness passwordBusiness = new SB004.Business.PasswordBusiness();
            string password = "p";

            // Act
            passwordBusiness.EnforcePasswordStengthRules(password);
        }

        [TestMethod]
        [ExpectedException(typeof(PasswordTooShortException))]
        public void Should_Validate_That_Password_Is_Not_Null()
        {
            // Arrange
            IPasswordBusiness passwordBusiness = new SB004.Business.PasswordBusiness();
            string password = null;

            // Act
            passwordBusiness.EnforcePasswordStengthRules(password);
        }

        [TestMethod]
        [ExpectedException(typeof(PasswordRequiresNumberException))]
        public void Should_Validate_That_Password_Has_At_Least_1_Number()
        {
            // Arrange
            IPasswordBusiness passwordBusiness = new SB004.Business.PasswordBusiness();
            string password = "passwordWithNoNumber";

            // Act
            passwordBusiness.EnforcePasswordStengthRules(password);
        }
    }
}
