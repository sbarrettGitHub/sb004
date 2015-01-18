using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace SB004.unit.tests.IdManagerTests
{
    [TestClass]
    public class WhenEncodingAGuid
    {
        [TestMethod]
        public void AnEncodedGuidIsReturned()
        {
            // Arrange
            string guid = "55387f3b-a985-4596-901f-e9da8f5d4e93";
            string encodedGuid = "O384VYWplkWQH-naj11Okw";
            Business.IdManager idManager = new Business.IdManager();

            // Act
            string result = idManager.EncodeGuid(guid);

            // Assert
            Assert.AreEqual(result,encodedGuid);

        }
        [TestMethod]
        [ExpectedException(typeof(FormatException))]
        public void AnInvalidGuidRaisesAnException()
        {
            // Arrange
            string guid = "55387f3b-a985";
            Business.IdManager idManager = new Business.IdManager();

            // Act
            idManager.EncodeGuid(guid);

            // Assert- exception thrown
        }
    }
}
