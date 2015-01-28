namespace SB004.Utilities
{
  using System.Net;

  public class Downloader : IDownloader
    {
        public byte[] GetBytes(string url)
        {
            var webClient = new WebClient();
            byte[] imageBytes = webClient.DownloadData(url);
            return imageBytes;
        }
    }
}