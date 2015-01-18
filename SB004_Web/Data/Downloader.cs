using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;

namespace SB004.Data
{
    public class Downloader : SB004.Data.IDownloader
    {
        public byte[] getBytes(string url)
        {
            var webClient = new WebClient();
            byte[] imageBytes = webClient.DownloadData(url);
            return imageBytes;
        }
    }
}