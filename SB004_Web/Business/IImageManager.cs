using SB004.Domain;

namespace SB004.Business
{
  public interface IImageManager
  {
    byte[] GetImageData(string image);
    ISeed PrimeSeed(ISeed seed);

    string ImageHash(byte[] imageData, int width, int height);
  }
}
