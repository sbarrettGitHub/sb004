using SB004.Domain;

namespace SB004.Utilities
{
  public interface IImageManager
  {
    byte[] GetImageData(string image);
    ISeed PrimeSeed(ISeed seed);

    string ImageHash(byte[] imageData, int width, int height);

    string ImageHashCategry(string imageHash);

    byte[] GenerateMemeImage(IMeme meme, byte[] seedImage);
  }
}
