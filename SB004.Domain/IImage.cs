namespace SB004.Domain
{
    public interface IImage
    {
        string Id { get; set; }
        byte[] ImageData { get; set; }
    }
}
