using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace SB004.unit.tests.IdManagerTests
{
    [TestClass]
    public class WhenDecodingAnEncodedGuid
    {
        [TestMethod]
        public void AnDecodedGuidIsReturned()
        {
            // Arrange
            string guid = "55387f3b-a985-4596-901f-e9da8f5d4e93";
            string encodedGuid = "O384VYWplkWQH-naj11Okw";
            Business.IdManager idManager = new Business.IdManager();

            // Act
            string result = idManager.DecodeGuid(encodedGuid);

            // Assert
            Assert.AreEqual(result, guid);
        }
        [TestMethod]
        [ExpectedException(typeof(FormatException))]
        public void AnInvalidEncodedGuidRaisesAnException()
        {
            // Arrange
            string encodedGuid = "O384VYWplkWQ";
            Business.IdManager idManager = new Business.IdManager();

            // Act
            idManager.DecodeGuid(encodedGuid);

            // Assert- exception thrown
        }

    }
}
