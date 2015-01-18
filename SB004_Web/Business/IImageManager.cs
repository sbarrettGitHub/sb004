using SB004.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SB004.Business
{
    public interface IImageManager
    {
        ISeed PrimeSeed(ISeed seed);
    }
}
