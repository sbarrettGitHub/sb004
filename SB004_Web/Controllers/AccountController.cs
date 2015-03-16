using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace SB004.Controllers
{
  using System.Configuration;
  using System.Security.Claims;
  using System.Threading.Tasks;
  using System.Web.Http.Results;

  using Microsoft.Owin.Security;
  using Microsoft.Owin.Security.OAuth;

  using Newtonsoft.Json.Linq;

  using SB004.Models;

  public class AccountController : ApiController
  {

    [HttpGet]
    public HttpResponseMessage Get(string id)
    {
      return new HttpResponseMessage(HttpStatusCode.OK);
    }
    [AllowAnonymous]
    [HttpGet]
    [Route("ObtainLocalAccessToken")]
    public async Task<IHttpActionResult> ObtainLocalAccessToken(string provider, string externalAccessToken)
    {

      if (string.IsNullOrWhiteSpace(provider) || string.IsNullOrWhiteSpace(externalAccessToken))
      {
        return BadRequest("Provider or external access token is not sent");
      }

      var verifiedAccessToken = await VerifyExternalAccessToken(provider, externalAccessToken);
      if (verifiedAccessToken == null)
      {
        return BadRequest("Invalid Provider or External Access Token");
      }

      // Find the user in our repository
      //IdentityUser user = await _repo.FindAsync(new UserLoginInfo(provider, verifiedAccessToken.user_id));
      User user = this.GetUser(provider, verifiedAccessToken.user_id);

      bool hasRegistered = user != null;

      if (!hasRegistered)
      {
        return BadRequest("External user is not registered");
      }

      //generate access token response
      var accessTokenResponse = GenerateLocalAccessTokenResponse(user.UserName);

      return Ok(accessTokenResponse);

    }
    /// <summary>
    /// Retrieve the user for the given provider access token
    /// </summary>
    /// <param name="provider"></param>
    /// <param name="accessToken"></param>
    /// <returns></returns>
    private User GetUser(string provider, string accessToken)
    {
      return new User();
    }

    private async Task<ParsedExternalAccessToken> VerifyExternalAccessToken(string provider, string accessToken)
    {
      ParsedExternalAccessToken parsedToken = null;

      var verifyTokenEndPoint = "";

      if (provider == "facebook")
      {
        //You can get it from here: https://developers.facebook.com/tools/accesstoken/
        //More about debug_tokn here: http://stackoverflow.com/questions/16641083/how-does-one-get-the-app-access-token-for-debug-token-inspection-on-facebook
        var appToken = ConfigurationManager.AppSettings["facebookAuthOptions.AppId"];
        verifyTokenEndPoint = string.Format("https://graph.facebook.com/debug_token?input_token={0}&access_token={1}", appToken, accessToken);
      }
      else if (provider == "Google")
      {
        verifyTokenEndPoint = string.Format("https://www.googleapis.com/oauth2/v1/tokeninfo?access_token={0}", accessToken);
      }
      else
      {
        return null;
      }

      var client = new HttpClient();
      var uri = new Uri(verifyTokenEndPoint);
      var response = await client.GetAsync(uri);

      if (response.IsSuccessStatusCode)
      {
        var content = await response.Content.ReadAsStringAsync();

        dynamic jObj = (JObject)Newtonsoft.Json.JsonConvert.DeserializeObject(content);

        parsedToken = new ParsedExternalAccessToken();

        if (provider == "Facebook")
        {
          parsedToken.user_id = jObj["data"]["user_id"];
          parsedToken.app_id = jObj["data"]["app_id"];

          if (!string.Equals(ConfigurationManager.AppSettings["facebookAuthOptions.AppId"], parsedToken.app_id, StringComparison.OrdinalIgnoreCase))
          {
            return null;
          }
        }
        else if (provider == "Google")
        {
          parsedToken.user_id = jObj["user_id"];
          parsedToken.app_id = jObj["audience"];

          if (!string.Equals(ConfigurationManager.AppSettings["googleAuthOptions.ClientId"], parsedToken.app_id, StringComparison.OrdinalIgnoreCase))
          {
            return null;
          }

        }

      }

      return parsedToken;
    }

    private JObject GenerateLocalAccessTokenResponse(string userName)
    {

      var tokenExpiration = TimeSpan.FromDays(1);

      ClaimsIdentity identity = new ClaimsIdentity(OAuthDefaults.AuthenticationType);

      identity.AddClaim(new Claim(ClaimTypes.Name, userName));
      identity.AddClaim(new Claim("role", "user"));

      var props = new AuthenticationProperties()
      {
        IssuedUtc = DateTime.UtcNow,
        ExpiresUtc = DateTime.UtcNow.Add(tokenExpiration),
      };

      var ticket = new AuthenticationTicket(identity, props);

      var accessToken = Startup.OAuthBearerOptions.AccessTokenFormat.Protect(ticket);

      JObject tokenResponse = new JObject(
                                  new JProperty("userName", userName),
                                  new JProperty("access_token", accessToken),
                                  new JProperty("token_type", "bearer"),
                                  new JProperty("expires_in", tokenExpiration.TotalSeconds.ToString()),
                                  new JProperty(".issued", ticket.Properties.IssuedUtc.ToString()),
                                  new JProperty(".expires", ticket.Properties.ExpiresUtc.ToString())
     );

      return tokenResponse;
    }
  }
}
