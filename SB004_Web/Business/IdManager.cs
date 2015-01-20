using System;

namespace SB004.Business
{
    public class IdManager : IIdManager
    {
      public string NewId()
      {
        return Guid.NewGuid().ToString("N");
      }
    }
}