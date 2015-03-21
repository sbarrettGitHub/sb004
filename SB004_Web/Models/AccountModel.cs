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
      public User() 
      { 
      }
      public User(IIdentity Identity)
      {
          var identity = (ClaimsIdentity)Identity;
          IEnumerable<Claim> claims = identity.Claims;
          var userNameClaim = claims.FirstOrDefault(x => x.Type == "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name");
          var userIdClaim = claims.FirstOrDefault(x => x.Type == "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier");
          UserId = userIdClaim != null?userIdClaim.Value:"";
          UserName = userNameClaim != null?userNameClaim.Value:"";

      }
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