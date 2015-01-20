using Microsoft.VisualStudio.TestTools.UnitTesting;
using SB004.Business;
namespace SB004.unit.tests.IdManagerTests
{
    [TestClass]
    public class WhenRequestingAnId
    {
        [TestMethod]
        public void NewIdIsReturned()
        {
            // Arrange
            IIdManager idManager = new IdManager();

            // Act
            string guid = idManager.NewId();

            // Assert
            Assert.IsNotNull(guid, "New Guid not returned");
            Assert.AreEqual(32, guid.Length, "An improper guid returned");
        }       
    }
}
