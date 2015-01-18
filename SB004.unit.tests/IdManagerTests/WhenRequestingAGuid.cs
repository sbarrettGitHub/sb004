using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SB004.Business;
namespace SB004.unit.tests.IdManagerTests
{
    [TestClass]
    public class WhenRequestingAGuid
    {
        [TestMethod]
        public void NewGuidIsReturned()
        {
            // Arrange
            Business.IdManager idManager = new Business.IdManager();

            // Act
            string guid = idManager.CreateGuid();

            // Assert
            Assert.IsNotNull(guid, "New Guid not returned");
            Assert.AreEqual(guid.Length, 36, "An improper guid returned");
        }       
    }
}
