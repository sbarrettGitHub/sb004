namespace SB004.Domain
{
    public interface ISeed
    {
        string Id { get; set; }
        string SourceImageUrl { get; set; }
        string ImageUrl { get; set; }
        byte[] ImageData { get; set; }
        int Height { get; set; }
        int Width { get; set; }
        string ImageHash { get; set; }
    }
}