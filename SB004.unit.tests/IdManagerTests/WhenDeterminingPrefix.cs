namespace SB004.Unit.Tests.IdManagerTests
{
  using Microsoft.VisualStudio.TestTools.UnitTesting;
  using SB004.Utilities;

  [TestClass]
  public class WhenDeterminingPrefix
  {
    [TestMethod]
    public void ProperPrefixReturned()
    {
      // Arrange
      IIdManager idManager = new IdManager();
      string guid = "150120s4b87f1daf40f4c63829083768596e2d4";

      // Act
      string typeIndicator = idManager.GetIdPrefix(guid);

      // Assert
      Assert.AreEqual("1501", typeIndicator, "An improper id prefix returned");
    }
  }
}
