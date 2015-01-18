using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SB004.Domain
{
    public interface ISeed
    {
        string id { get; set; }
        string sourceImageUrl { get; set; }
        string imageUrl { get; set; }
        byte[] imageData { get; set; }
        int height { get; set; }
        int width { get; set; }
    }
}