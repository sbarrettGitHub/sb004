namespace SB004.Data.Entities
{

    using MongoDB.Bson;
    using SB004.Domain;

  public class SeedEntity
  {
    public SeedEntity()
    {
    }
    public SeedEntity(ISeed seed)
    {
      SourceImageUrl = seed.SourceImageUrl;
      Width = seed.Width;
      Height = seed.Height;
      ImageHash = seed.ImageHash;
      ImageData = seed.ImageData;
    }

    public ObjectId Id { get; set; }
    public string SourceImageUrl { get; set; }
    public int Height { get; set; }
    public int Width { get; set; }
    public string ImageHash { get; set; }
    public byte[] ImageData { get; set; }

    public ISeed ToISeed()
    {
        return new Seed
        {
            Id = this.Id.ToString(),
            Width = this.Width,
            Height = this.Height,
            ImageHash = this.ImageHash,
            SourceImageUrl = this.SourceImageUrl,
            ImageData = this.ImageData
        };        
    }
  }
}