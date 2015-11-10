using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Security.Principal;
using System.Web;

namespace SB004.Models
{

  public class ParsedExternalAccessToken
  {
    public string user_id { get; set; }
    public string app_id { get; set; }
    public string username { get; set; }
  }
  public class NewAccount 
  {
      public string UserName { get; set; }
      public string Email { get; set; }
      public string Password { get; set; }
  }
}