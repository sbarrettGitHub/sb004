
using System;
using System.Drawing;
using System.IO;
using SB004.Data;
using SB004.Domain;
namespace SB004.Business
{
  using System.Globalization;
  using System.Security.Cryptography;
  using System.Text;

  public class ImageManager : IImageManager
  {
    readonly IDownloader downloader;

    public ImageManager(IDownloader downloader)
    {
      this.downloader = downloader;
    }

    /// <summary>
    /// Accecpts an image string which can be an image URL or base64 encoded image data
    /// </summary>
    /// <param name="image">a url or base 64 data string</param>
    /// <returns></returns>
    public byte[] GetImageData(string image)
    {
      byte[] imageData;
      // Is the image a URL or does it contain base 64 data
      if (image.IndexOf("http", StringComparison.Ordinal) >= 0)
      {
        // Download
        imageData = downloader.getBytes(image);
      }
      else
      {
        // Not a url. Load the bytes from the base 64 data
        imageData = Convert.FromBase64String(image);
      }

      return imageData;
    }

    /// <summary>
    /// Resize seed image to required dimensions. Convert to JPeg
    /// </summary>
    /// <param name="seed"></param>
    /// <returns></returns>
    public ISeed PrimeSeed(ISeed seed)
    {
 
      // Resize to required dimensions (converting to JPeg)
      seed.ImageData = CreateThumbnail(seed.ImageData, seed.Width > seed.Height ? seed.Width : seed.Height, System.Drawing.Imaging.ImageFormat.Jpeg);

      return seed;
    }
    /// <summary>
    /// Create a "unique" hash of a byte array including an expected width and height
    /// noOfBytes-width-height-byteHash 
    /// </summary>
    /// <param name="imageData"></param>
    /// <param name="width"></param>
    /// <param name="height"></param>
    /// <returns></returns>
    public string ImageHash(byte[] imageData, int width, int height)
    {
      StringBuilder hash = new StringBuilder();
      using (SHA1CryptoServiceProvider sha1 = new SHA1CryptoServiceProvider())
      {
        hash.Append(imageData.Length.ToString(CultureInfo.InvariantCulture));
        hash.Append("-");
        hash.Append(width.ToString(CultureInfo.InvariantCulture));
        hash.Append("-");
        hash.Append(height.ToString(CultureInfo.InvariantCulture));
        hash.Append("-");
        hash.Append(Convert.ToBase64String(sha1.ComputeHash(imageData)));
      }
      return hash.ToString();
    }

    /// <summary>
    /// Resize the image to the required dimensions (maintaining aspect ratio)
    /// </summary>
    /// <param name="passedImage"></param>
    /// <param name="largestSide"></param>
    /// <param name="format"></param>
    /// <returns></returns>
    public byte[] CreateThumbnail(byte[] passedImage, int largestSide, System.Drawing.Imaging.ImageFormat format)
    {
      byte[] returnedThumbnail;

      using (MemoryStream startMemoryStream = new MemoryStream(),
                          newMemoryStream = new MemoryStream())
      {
        // write the string to the stream  
        startMemoryStream.Write(passedImage, 0, passedImage.Length);

        // create the start Bitmap from the MemoryStream that contains the image  
        Bitmap startBitmap = new Bitmap(startMemoryStream);

        // set thumbnail height and width proportional to the original image.  
        int newHeight;
        int newWidth;
        double hwRatio;
        if (startBitmap.Height > startBitmap.Width)
        {
          newHeight = largestSide;
          hwRatio = largestSide / (double)startBitmap.Height;
          newWidth = (int)(hwRatio * startBitmap.Width);
        }
        else
        {
          newWidth = largestSide;
          hwRatio = largestSide / (double)startBitmap.Width;
          newHeight = (int)(hwRatio * startBitmap.Height);
        }

        // create a new Bitmap with dimensions for the thumbnail.  
        // Copy the image from the START Bitmap into the NEW Bitmap.  
        // This will create a thumnail size of the same image.  
        Bitmap newBitmap = this.ResizeImage(startBitmap, newWidth, newHeight);

        // Save this image to the specified stream in the specified format.  
        newBitmap.Save(newMemoryStream, format);

        // Fill the byte[] for the thumbnail from the new MemoryStream.  
        returnedThumbnail = newMemoryStream.ToArray();
      }

      // return the resized image as a string of bytes.  
      return returnedThumbnail;
    }

    /// <summary>
    /// Given an image hash categorizes it by size. 
    /// Simply return the left 3 chars
    /// </summary>
    /// <param name="imageHash"></param>
    /// <returns></returns>
    public string ImageHashCategry(string imageHash)
    {
      return imageHash.Length > 2 ? imageHash.Substring(0, 3) : "0";
    }

    // Resize a Bitmap  
    private Bitmap ResizeImage(Bitmap image, int width, int height)
    {
      Bitmap resizedImage = new Bitmap(width, height);
      using (Graphics gfx = Graphics.FromImage(resizedImage))
      {
        gfx.DrawImage(image, new Rectangle(0, 0, width, height),
            new Rectangle(0, 0, image.Width, image.Height), GraphicsUnit.Pixel);
      }
      return resizedImage;
    }
  }
}