using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SB004.Domain
{
    public class UserAlreadyRegisteredException:Exception
    {
    }
    public class InvalidEmailOrPasswordException : Exception
    {
    }
}
