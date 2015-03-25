using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Security.Principal;
using System.Web;

namespace SB004.Models
{
  public class User
  {
    public string UserName { get; set; }

    public string UserId { get; set; }

  }
  public class ParsedExternalAccessToken
  {
    public string user_id { get; set; }
    public string app_id { get; set; }
    public string username { get; set; }
  }
}