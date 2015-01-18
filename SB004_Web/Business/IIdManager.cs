using System;
namespace SB004.Business
{
    interface IIdManager
    {
        string CreateGuid();
        string DecodeGuid(string encodedGuid);
        string EncodeGuid(string decodedGuid);
    }
}
