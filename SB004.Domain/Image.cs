using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SB004.Domain
{
    public class Image:IImage
    {
        public string Id { get; set; }
        public byte[] ImageData { get; set; }
    }
}
