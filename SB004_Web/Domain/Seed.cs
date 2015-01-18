using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using SB004.Domain;
namespace SB004.Domain
{
    public class Seed : ISeed
    {
        public string id { get; set; }
        public string sourceImageUrl { get; set; }
        public string imageUrl { get; set; }
        public byte[] imageData { get; set; }
        public int height { get; set; }
        public int width { get; set; }
    }
}