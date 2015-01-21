
namespace SB004.Data.Entities
{
  using Microsoft.WindowsAzure.Storage.Table;

  using SB004.Domain;

  public class SeedHashEntity:TableEntity
  {
     public SeedHashEntity()
    {
    }
    public SeedHashEntity(ISeed seed, string hashCategory)
    {
      PartitionKey = hashCategory;
      RowKey = seed.ImageHash;
      SeedId = seed.Id;
    }
    public string  SeedId { get; set; }
  }
}