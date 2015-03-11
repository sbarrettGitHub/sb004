
using System;
using System.Drawing;
using System.IO;

using SB004.Domain;
namespace SB004.Utilities
{
    using System.Drawing.Drawing2D;
    using System.Drawing.Imaging;
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
                imageData = downloader.GetBytes(image);
            }
            else
            {
                // Not a url. Load the bytes from the base 64 data
                imageData = Convert.FromBase64String(image.Replace("data:image/jpeg;base64,",""));
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

          // Resize to required dimensions (converting to JPeg). Resize to width or height whichever is greater, keeping aspect ratio. 
            seed.ImageData = ResizeImage(seed.ImageData, seed.Width > seed.Height ? seed.Width : seed.Height, ImageFormat.Jpeg);

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
        /// Given an image hash categorizes it by size. 
        /// Simply return the left 3 chars
        /// </summary>
        /// <param name="imageHash"></param>
        /// <returns></returns>
        public string ImageHashCategry(string imageHash)
        {
            return imageHash.Length > 2 ? imageHash.Substring(0, 3) : "0";
        }

        /// <summary>
        /// Using the seed image and comment data in the meme supplied, create a meme image 
        /// </summary>
        /// <param name="memeData"></param>
        /// <param name="seedImage"></param>
        /// <returns></returns>
        public byte[] GenerateMemeImage(IMeme memeData, byte[] seedImage)
        {
            using (MemoryStream ms = new MemoryStream(seedImage),
                                newMemoryStream = new MemoryStream())
            {
                SolidBrush shadowBrush;
                SolidBrush textBrush;
                // create the start Bitmap from the MemoryStream that contains the image  
                Bitmap memeBitmap = new Bitmap(ms);
                Graphics g = Graphics.FromImage(memeBitmap);
                g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;

                StringFormat strFormat = new StringFormat();
                foreach (IComment comment in memeData.Comments)
                {
                    int x = comment.Position.X, width = comment.Position.Width, y = comment.Position.Y, height = comment.Position.Height;

                    //ToDo: Apply formmating
                    strFormat.Alignment = StringAlignment.Near;
                    switch (comment.TextAlign)
                    { 
                        case "left":
                            strFormat.Alignment = StringAlignment.Near;
                            break;
                        case "center":
                            strFormat.Alignment = StringAlignment.Center;
                            break;
                        case "right":
                            strFormat.Alignment = StringAlignment.Far;
                            break;
                    }
                    Font font = new Font(comment.FontFamily, float.Parse(comment.FontSize.Replace("pt", "")),FontStyle.Bold, GraphicsUnit.Point);
                    
                    // Draw the shadow (really a border) by printing the text slightly to the left, then right, then above and below of the real text.                     
                    if (comment.TextShadow != "none")
                    {
                        shadowBrush = new SolidBrush(Color.FromName(comment.TextShadow));                        
                        g.DrawString(comment.Text, font, shadowBrush, new RectangleF(x - 1, y, x - 1 + width, y + height), strFormat);
                        g.DrawString(comment.Text, font, shadowBrush, new RectangleF(x + 1, y, x + 1 + width, y + height), strFormat);
                        g.DrawString(comment.Text, font, shadowBrush, new RectangleF(x, y - 1, x + width, y - 1 + height), strFormat);
                        g.DrawString(comment.Text, font, shadowBrush, new RectangleF(x, y + 1, x + width, y + 1 + height), strFormat);
                    }
                    
                    textBrush = new SolidBrush(Color.FromName(comment.Color));
                    
                    // Print the text
                    g.DrawString(comment.Text, font, textBrush, new RectangleF(x , y, x + width, y + height), strFormat);
                    //g.DrawString(comment.Text, font, textBrush, new RectangleF(1, 50, 250, 100), strFormat);

                }
                memeBitmap.Save(newMemoryStream, ImageFormat.Jpeg);

                return newMemoryStream.ToArray();
            }
        }
        /// <summary>
        /// Resize the image to the required dimensions (maintaining aspect ratio)
        /// </summary>
        /// <param name="passedImage"></param>
        /// <param name="largestSide"></param>
        /// <param name="format"></param>
        /// <returns></returns>
        private byte[] ResizeImage(byte[] passedImage, int largestSide, ImageFormat format)
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
        //Add Watermark to photo.
        private System.Drawing.Image CreateWatermark(System.Drawing.Image imgPhoto, string Copyright)
        {
            Graphics g = Graphics.FromImage(imgPhoto);

            g.SmoothingMode = SmoothingMode.HighQuality;
            g.InterpolationMode = InterpolationMode.HighQualityBicubic;
            g.PixelOffsetMode = PixelOffsetMode.HighQuality;

            foreach (PropertyItem pItem in imgPhoto.PropertyItems)
            {
                imgPhoto.SetPropertyItem(pItem);
            }

            int phWidth = imgPhoto.Width;
            int phHeight = imgPhoto.Height;

            //create a Bitmap the Size of the original photograph
            Bitmap bmPhoto = new Bitmap(phWidth, phHeight, PixelFormat.Format24bppRgb);

            bmPhoto.SetResolution(imgPhoto.HorizontalResolution, imgPhoto.VerticalResolution);

            //load the Bitmap into a Graphics object 
            Graphics grPhoto = Graphics.FromImage(bmPhoto);

            //------------------------------------------------------------
            //Step #1 - Insert Copyright message
            //------------------------------------------------------------

            //Set the rendering quality for this Graphics object
            grPhoto.SmoothingMode = SmoothingMode.AntiAlias;

            //Draws the photo Image object at original size to the graphics object.
            grPhoto.DrawImage(
                imgPhoto,                               // Photo Image object
                new Rectangle(0, 0, phWidth, phHeight), // Rectangle structure
                0,                                      // x-coordinate of the portion of the source image to draw. 
                0,                                      // y-coordinate of the portion of the source image to draw. 
                phWidth,                                // Width of the portion of the source image to draw. 
                phHeight,                               // Height of the portion of the source image to draw. 
                GraphicsUnit.Pixel);                    // Units of measure 

            //-------------------------------------------------------
            //to maximize the size of the Copyright message we will 
            //test multiple Font sizes to determine the largest posible 
            //font we can use for the width of the Photograph
            //define an array of point sizes you would like to consider as possiblities
            //-------------------------------------------------------
            int[] sizes = new int[] { 16, 14, 12, 10, 8, 6, 4 };

            Font crFont = null;
            SizeF crSize = new SizeF();

            //Loop through the defined sizes checking the length of the Copyright string
            //If its length in pixles is less then the image width choose this Font size.
            for (int i = 0; i < 7; i++)
            {
                //set a Font object to Arial (i)pt, Bold
                crFont = new Font("arial", sizes[i], FontStyle.Bold);
                //Measure the Copyright string in this Font
                crSize = grPhoto.MeasureString(Copyright, crFont);

                if ((ushort)crSize.Width < (ushort)phWidth)
                    break;
            }

            //Since all photographs will have varying heights, determine a 
            //position 5% from the bottom of the image
            int yPixlesFromBottom = (int)(phHeight * .05);

            //Now that we have a point size use the Copyrights string height 
            //to determine a y-coordinate to draw the string of the photograph
            float yPosFromBottom = ((phHeight - yPixlesFromBottom) - (crSize.Height / 2));

            //Determine its x-coordinate by calculating the center of the width of the image
            float xCenterOfImg = (phWidth / 2);

            //Define the text layout by setting the text alignment to centered
            StringFormat StrFormat = new StringFormat();
            StrFormat.Alignment = StringAlignment.Near;

            //define a Brush which is semi trasparent black (Alpha set to 153)
            SolidBrush semiTransBrush2 = new SolidBrush(System.Drawing.Color.FromArgb(153, 0, 0, 0));

            //Draw the Copyright string
            grPhoto.DrawString(Copyright,                 //string of text
                crFont,                                   //font
                semiTransBrush2,                           //Brush
                new PointF(xCenterOfImg + 1, yPosFromBottom + 1),  //Position
                StrFormat);

            //define a Brush which is semi trasparent white (Alpha set to 153)
            SolidBrush semiTransBrush = new SolidBrush(System.Drawing.Color.FromArgb(153, 255, 255, 255));

            //Draw the Copyright string a second time to create a shadow effect
            //Make sure to move this text 1 pixel to the right and down 1 pixel
            grPhoto.DrawString(Copyright,                 //string of text
                crFont,                                   //font
                semiTransBrush,                           //Brush
                new PointF(xCenterOfImg, yPosFromBottom),  //Position
                StrFormat);                               //Text alignment
            imgPhoto = bmPhoto;
            return imgPhoto;
        }
    }
}