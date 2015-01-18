using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SB004.Business
{
    public class IdManager : SB004.Business.IIdManager
    {
        public string CreateGuid()
        {
            return Guid.NewGuid().ToString();
        }
        public string EncodeGuid(string decodedGuid) 
        {
            var guid = new Guid(decodedGuid);
            string enc = Convert.ToBase64String(guid.ToByteArray());
            enc = enc.Replace("/", "_");
            enc = enc.Replace("+", "-");
            return enc.Substring(0, 22);
        }
        public string DecodeGuid(string encodedGuid)
        {
            string encoded = encodedGuid.Replace("_", "/");
            encoded = encoded.Replace("-", "+");
            byte[] buffer = Convert.FromBase64String(encoded + "==");
            return new Guid(buffer).ToString();
        }
    }
}