namespace SB004.Data.Entities
{
  using Microsoft.WindowsAzure.Storage.Table;

  using SB004.Domain;

  public class SeedEntity:TableEntity
  {
    public SeedEntity()
    {
    }
    public SeedEntity(ISeed seed, string idCategory)
    {
      PartitionKey = idCategory;
      RowKey = seed.Id;
      SourceImageUrl = seed.SourceImageUrl;
      ImageUrl = seed.ImageUrl;
      Width = seed.Width;
      Height = seed.Height;
      ImageHash = seed.ImageHash;
    }
    public string SourceImageUrl { get; set; }
    public string ImageUrl { get; set; }
    public int Height { get; set; }
    public int Width { get; set; }
    public string ImageHash { get; set; }
  }
}