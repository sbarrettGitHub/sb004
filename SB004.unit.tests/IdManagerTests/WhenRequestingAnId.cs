using Microsoft.VisualStudio.TestTools.UnitTesting;
using SB004.Business;
namespace SB004.unit.tests.IdManagerTests
{
  [TestClass]
  public class WhenRequestingAnId
  {
    [TestMethod]
    public void NewIdIsReturnedWithSeedIndicator()
    {
      // Arrange
      IIdManager idManager = new IdManager();

      // Act
      string guid = idManager.NewId(IdType.Seed);

      // Assert
      Assert.IsNotNull(guid, "New Guid not returned");
      Assert.AreEqual(39, guid.Length, "An improper id returned");
    }
    [TestMethod]
    public void AndSeedRequestedNewIdIsReturnedWithSeedIndicator()
    {
      // Arrange
      IIdManager idManager = new IdManager();

      // Act
      string guid = idManager.NewId(IdType.Seed);

      // Assert
      Assert.IsNotNull(guid, "New Guid not returned");
      Assert.AreEqual(39, guid.Length, "An improper id returned");
      Assert.AreEqual("s", guid.Substring(6, 1), "Type not indicated in id");
    }
    [TestMethod]
    public void AndMemeRequestedNewIdIsReturnedWithSeedIndicator()
    {
      // Arrange
      IIdManager idManager = new IdManager();

      // Act
      string guid = idManager.NewId(IdType.Meme);

      // Assert
      Assert.IsNotNull(guid, "New Guid not returned");
      Assert.AreEqual(39, guid.Length, "An improper id returned");
      Assert.AreEqual("m", guid.Substring(6, 1), "Type not indicated in id");
    }
    [TestMethod]
    public void AndUnknownRequestedNewIdIsReturnedWithSeedIndicator()
    {
      // Arrange
      IIdManager idManager = new IdManager();

      // Act
      string guid = idManager.NewId(IdType.Unknown);

      // Assert
      Assert.IsNotNull(guid, "New Guid not returned");
      Assert.AreEqual(39, guid.Length, "An improper id returned");
      Assert.AreEqual(guid.Substring(6, 1), "u", "Type not indicated in id");
    }
  }
}
