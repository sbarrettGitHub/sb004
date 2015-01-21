namespace SB004.Domain
{
    public class Seed : ISeed
    {
        public string Id { get; set; }
        public string SourceImageUrl { get; set; }
        public string ImageUrl { get; set; }
        public byte[] ImageData { get; set; }
        public int Height { get; set; }
        public int Width { get; set; }
        public string ImageHash { get; set; }
    }
}