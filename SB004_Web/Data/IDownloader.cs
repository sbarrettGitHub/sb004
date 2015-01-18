using System;
namespace SB004.Data
{
    public interface IDownloader
    {
        byte[] getBytes(string url);
    }
}
