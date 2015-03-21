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

        /// <summary>
        /// Test that the user is authenticated
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        [Authorize]
        public HttpResponseMessage Get(string id)
        {
            Models.User user = new Models.User(User.Identity);
            if (user.UserId == id)
            {
                return new HttpResponseMessage(HttpStatusCode.OK);
            }
            return new HttpResponseMessage(HttpStatusCode.Unauthorized);
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

            // Verify that the access token supplied is valid
            ParsedExternalAccessToken verifiedAccessToken = await VerifyExternalAccessToken(provider, externalAccessToken);
            
            if (verifiedAccessToken == null)
            {
                return BadRequest("Invalid Provider or External Access Token");
            }

            // Find the user in our repository
            // IdentityUser user = await _repo.FindAsync(new UserLoginInfo(provider, verifiedAccessToken.user_id));
            User user = this.GetUser(provider, verifiedAccessToken.user_id);

            bool hasRegistered = user != null;

            if (!hasRegistered)
            {
                return BadRequest("External user is not registered");
            }

            //generate access token response
            var accessTokenResponse = GenerateLocalAccessTokenResponse(user);

            // Return success with the bearer token for authorized access
            return Ok(accessTokenResponse);

        }
        /// <summary>
        /// Retrieve the user for the given provider access token
        /// </summary>
        /// <param name="provider"></param>
        /// <param name="accessToken"></param>
        /// <returns></returns>
        private User GetUser(string provider, string providerUserId)
        {
            return new User { UserId = Guid.NewGuid().ToString("N"), UserName = "test User" };
        }

        private async Task<ParsedExternalAccessToken> VerifyExternalAccessToken(string provider, string accessToken)
        {
            ParsedExternalAccessToken parsedToken = null;

            var verifyTokenEndPoint = "";

            if (provider == "facebook")
            {
                //You can get it from here: https://developers.facebook.com/tools/accesstoken/
                //More about debug_tokn here: http://stackoverflow.com/questions/16641083/how-does-one-get-the-app-access-token-for-debug-token-inspection-on-facebook
                var appToken = ConfigurationManager.AppSettings["facebookAuthOptions.AppToken"];
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

                if (provider == "facebook")
                {

                    parsedToken.app_id = jObj["data"]["app_id"];

                    // Verify the app id against the access token supplied
                    if (!string.Equals(ConfigurationManager.AppSettings["facebookAuthOptions.AppId"], parsedToken.app_id, StringComparison.OrdinalIgnoreCase))
                    {
                        return null;
                    }

                    // Now get the user data from facebook
                    string verifyTokenEndPointUser = string.Format("https://graph.facebook.com/me?suppress_response_codes=true&access_token={0}", accessToken);
                    response = await client.GetAsync(new Uri(verifyTokenEndPointUser));
                    content = await response.Content.ReadAsStringAsync();

                    jObj = (JObject)Newtonsoft.Json.JsonConvert.DeserializeObject(content);

                    parsedToken.user_id = jObj["id"];
                    parsedToken.username = jObj["name"];

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

        /// <summary>
        /// Create a bearer token
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        private JObject GenerateLocalAccessTokenResponse(User user)
        {

            var tokenExpiration = TimeSpan.FromDays(1);

            ClaimsIdentity identity = new ClaimsIdentity(OAuthDefaults.AuthenticationType);

            identity.AddClaim(new Claim(ClaimTypes.Name, user.UserName));
            identity.AddClaim(new Claim(ClaimTypes.NameIdentifier, user.UserId));
            identity.AddClaim(new Claim("role", "user"));

            var props = new AuthenticationProperties()
            {
                IssuedUtc = DateTime.UtcNow,
                ExpiresUtc = DateTime.UtcNow.Add(tokenExpiration),
            };

            var ticket = new AuthenticationTicket(identity, props);

            var accessToken = Startup.OAuthBearerOptions.AccessTokenFormat.Protect(ticket);

            JObject tokenResponse = new JObject(
                                        new JProperty("userName", user.UserName),
                                        new JProperty("userId", user.UserId),
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
