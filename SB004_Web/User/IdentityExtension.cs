namespace SB004.User
{
  using System.Collections.Generic;
  using System.Linq;
  using System.Security.Claims;
  using System.Security.Principal;

  public static class IdentityExtension
  {
    /// <summary>
    /// Read the user id from the claims in the identity
    /// </summary>
    /// <param name="identity"></param>
    /// <returns></returns>
    public static string UserId(this IIdentity identity)
    {
      if (identity.IsAuthenticated == false)
      {
        return null;
      }
      var userIdentity = (ClaimsIdentity)identity;

      if (userIdentity.Claims == null)
      {
        return null;
      }

      IEnumerable<Claim> claims = userIdentity.Claims;
      var userIdClaim = claims.FirstOrDefault(x => x.Type == "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier");
      return userIdClaim != null ? userIdClaim.Value : null;
    }
  }
}