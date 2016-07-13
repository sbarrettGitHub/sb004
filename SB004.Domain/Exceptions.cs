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
    public class UnrecognizedEmailAddressException : Exception
    {
    }
    public class InvalidPasswordResetTokenException : Exception
    {
    }
    public class ExpiredPasswordResetTokenException : Exception
    {
    }
    public class UserNotActiveException : Exception
    {
    }
    public class UserNotFoundException : Exception
    {
    }
    public class PasswordTooShortException : Exception
    {
    }
    public class PasswordRequiresNumberException : Exception
    {
    }
}
