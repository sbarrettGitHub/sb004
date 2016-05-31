

using System.Linq;

namespace SB004.Controllers
{
	using System;
	using System.Configuration;
	using System.Net;
	using System.Net.Http;
	using System.Security.Claims;
	using System.Threading.Tasks;
	using System.Web.Http;
	using Microsoft.Owin.Security;
	using Microsoft.Owin.Security.OAuth;
	using Newtonsoft.Json.Linq;
	using SB004.Models;
	using SB004.User;
	using SB004.Domain;
	using SB004.Data;
	using Newtonsoft.Json.Serialization;
	using System.Net.Http.Formatting;
	using System.Collections.Generic;
	using SB004.Business;
	[RoutePrefix("api/account")]
	public class AccountController : ApiController
	{
		readonly IRepository repository;
		readonly IAccountBusiness accountBusiness;

		public AccountController(IRepository repository, IAccountBusiness accountBusiness)
		{
			this.repository = repository;
			this.accountBusiness = accountBusiness;
		}
		/// <summary>
		/// Test that the user is authenticated
		/// </summary>
		/// <param name="id"></param>
		/// <returns>User profile</returns>
		[HttpGet]
		[Authorize]
		[Route("{id}")]
		public IHttpActionResult Get(string id)
		{
			if (User.Identity.UserId() == id)
			{
				IUser profile = repository.GetUser(id);

				if (profile != null)
				{
					profile.Email = WebUtility.HtmlEncode(profile.Email);
					profile.StatusMessage = WebUtility.HtmlEncode(profile.StatusMessage);
					profile.UserName = WebUtility.HtmlEncode(profile.UserName);
					
					return Ok(profile);
				}
			}
			return BadRequest("User not authorized");
		}
		/// <summary>
		/// Add a user to the current user's list of users he follows
		/// </summary>
		/// <param name="id"></param>
		/// <param name="followedId"></param>
		/// <returns>User profile</returns>
		[HttpPatch]
		[Authorize]
		[Route("{id}/follow/{followedId}")]
		public HttpResponseMessage Follow(string id, string followedId)
		{

			if (User.Identity.UserId() == id)
			{
				// Can't follow yourself
				if (id == followedId)
				{
					throw new HttpResponseException(new HttpResponseMessage(HttpStatusCode.PreconditionFailed));
				}
				try
				{
					accountBusiness.Follow(id, followedId);
				}
				catch (ArgumentException)
				{
					
					throw new HttpResponseException(new HttpResponseMessage(HttpStatusCode.NotFound));
				}

				HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK, repository.GetUser(id, true));
				response.Headers.Location = new Uri(Request.RequestUri, "/api/account/" + id);
				return response;
			}
			throw new HttpResponseException(new HttpResponseMessage(HttpStatusCode.NotFound));
		}
		/// <summary>
		/// Remove a user from the current user's list of users he follows
		/// </summary>
		/// <param name="id"></param>
		/// <param name="followedId"></param>
		/// <returns>User profile</returns>
		[HttpPatch]
		[Authorize]
		[Route("{id}/unfollow/{followedId}")]
		public HttpResponseMessage Unfollow(string id, string followedId)
		{
			if (User.Identity.UserId() == id)
			{
				try
				{
					accountBusiness.Unfollow(id, followedId);

					HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK, repository.GetUser(id, true));
					response.Headers.Location = new Uri(Request.RequestUri, "/api/account/" + id);
					return response;
				}
				catch (ArgumentException)
				{

					throw new HttpResponseException(new HttpResponseMessage(HttpStatusCode.NotFound));
				}
			}
			throw new HttpResponseException(new HttpResponseMessage(HttpStatusCode.NotFound));
		}

		/// <summary>
		/// Update the current users profile name
		/// </summary>
		/// <param name="id"></param>
		/// <param name="details"></param>
		/// <returns>User profile</returns>
		[HttpPatch]
		[Authorize]
		[Route("{id}/name")]
		public HttpResponseMessage UpdateProfileName(string id, [FromBody] AccountDetailsModel details)
		{
			if ((details.UserName ?? "").Length == 0 || ModelState.IsValid == false)
			{
				throw new HttpResponseException(new HttpResponseMessage(HttpStatusCode.BadRequest));
			}
			if (User.Identity.UserId() == id)
			{
				IUser profile = repository.GetUser(id);

				if (profile != null)
				{
					profile.UserName = details.UserName;

					// Save 
					repository.Save(profile);

					HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK, profile);
					response.Headers.Location = new Uri(Request.RequestUri, "/api/account/" + id);
					return response;
				}
				// No such user
				throw new HttpResponseException(new HttpResponseMessage(HttpStatusCode.NotFound));
			}
			throw new HttpResponseException(new HttpResponseMessage(HttpStatusCode.Forbidden));
		}
		/// <summary>
		/// Update the current users profile name
		/// </summary>
		/// <param name="id"></param>
		/// <param name="details"></param>
		/// <returns>User profile</returns>
		[HttpPatch]
		[Authorize]
		[Route("{id}/status")]
		public HttpResponseMessage UpdateProfileStatusMessage(string id, [FromBody] AccountDetailsModel details)
		{
			if (User.Identity.UserId() == id)
			{
				IUser profile = repository.GetUser(id);

				if (profile != null)
				{
					profile.StatusMessage = details.StatusMessage ?? "";

					// Save 
					repository.Save(profile);

					HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK, profile);
					response.Headers.Location = new Uri(Request.RequestUri, "/api/account/" + id);
					return response;
				}
				// No such user
				throw new HttpResponseException(new HttpResponseMessage(HttpStatusCode.NotFound));
			}
			throw new HttpResponseException(new HttpResponseMessage(HttpStatusCode.Forbidden));
		}
		/// <summary>
		/// Update the current users profile email
		/// </summary>
		/// <param name="id"></param>
		/// <param name="details"></param>
		/// <returns>User profile</returns>
		[HttpPatch]
		[Authorize]
		[Route("{id}/email")]
		public HttpResponseMessage UpdateProfileEmail(string id, [FromBody] AccountDetailsModel details)
		{
			if ((details.Email ?? "").Length == 0 || ModelState.IsValid == false)
			{
				throw new HttpResponseException(new HttpResponseMessage(HttpStatusCode.BadRequest));
			}
			if (User.Identity.UserId() == id)
			{
				IUser profile = repository.GetUser(id);
				if (profile != null)
				{
					// User must supply a password to change his email address. Valid password and existing email
					try
					{
						accountBusiness.SignIn(profile.Email, details.Password);
					}
					catch (InvalidEmailOrPasswordException)
					{

						throw new HttpResponseException(new HttpResponseMessage(HttpStatusCode.Forbidden));
					}


					Domain.ICredentials credentials = repository.GetCredentials(profile.Email);

					if (credentials != null)
					{
						profile.Email = details.Email;
						credentials.Email = details.Email;

						// Save 
						repository.Save(profile);
						repository.Save(credentials);

						HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK, profile);
						response.Headers.Location = new Uri(Request.RequestUri, "/api/account/" + id);
						return response;
					}
				}
				
				// No such user
				throw new HttpResponseException(new HttpResponseMessage(HttpStatusCode.NotFound));
			}
			throw new HttpResponseException(new HttpResponseMessage(HttpStatusCode.Forbidden));
		}
		/// <summary>
		/// Update the current users profile email
		/// </summary>
		/// <param name="id"></param>
		/// <param name="details"></param>
		/// <returns>User profile</returns>
		[HttpPatch]
		[Authorize]
		[Route("{id}/password")]
		public HttpResponseMessage ChangePassword(string id, [FromBody] AccountDetailsModel details)
		{
			if ((details.Email ?? "").Length == 0 || (details.Password ?? "").Length == 0 || (details.NewPassword ?? "").Length == 0)
			{
				throw new HttpResponseException(new HttpResponseMessage(HttpStatusCode.BadRequest));
			}
			if (User.Identity.UserId() == id)
			{
				IUser profile = repository.GetUser(id);
				if (profile != null)
				{
					// Ensure the email of the user logged in is being used
					if (profile.Email != details.Email)
					{
						throw new HttpResponseException(new HttpResponseMessage(HttpStatusCode.Forbidden));
					}
					// User must supply existing password to change his email address. Valid password and existing email
					try
					{
						accountBusiness.SignIn(profile.Email, details.Password);
					}
					catch (InvalidEmailOrPasswordException)
					{

						throw new HttpResponseException(new HttpResponseMessage(HttpStatusCode.Forbidden));
					}

					// Retrieve the user credentials
					Domain.ICredentials credentials = repository.GetCredentials(profile.Email);

					if (credentials != null)
					{
						credentials.Password = details.NewPassword;

						// Save user credentials
						accountBusiness.ChangePassword(profile, credentials);

						HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK, profile);
						response.Headers.Location = new Uri(Request.RequestUri, "/api/account/" + id);
						return response;
					}
				}

				// No such user or user has no credentials
				throw new HttpResponseException(new HttpResponseMessage(HttpStatusCode.NotFound));
			}
			throw new HttpResponseException(new HttpResponseMessage(HttpStatusCode.Forbidden));
		}		
		/// <summary>
		/// Update the current users profile email
		/// </summary>
		/// <param name="id"></param>
		/// <param name="credentials"></param>
		/// <returns>User profile</returns>
		[HttpPost]
		[Authorize]
		[Route("{id}/close")]
		public HttpResponseMessage CloseAccount(string id, [FromBody] SignInModel credentials)
		{

			if (User.Identity.UserId() == id)
			{
				IUser profile = repository.GetUser(id);

				if (profile != null)
				{
					try
					{
						// Validate the credentials
						IUser authenicatedUser = accountBusiness.SignIn(credentials.Email, credentials.Password);
						if (authenicatedUser.Id != profile.Id)
						{
							throw new HttpResponseException(new HttpResponseMessage(HttpStatusCode.Unauthorized));
						}
					}
					catch (InvalidEmailOrPasswordException)
					{
						throw new HttpResponseException(new HttpResponseMessage(HttpStatusCode.Unauthorized));
					}

					// Deactivate the user profile
					profile.Active = false;

					// Save 
					repository.Save(profile);

					HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK, profile);
					response.Headers.Location = new Uri(Request.RequestUri, "/api/account/" + id);
					return response;
				}
				// No such user
				throw new HttpResponseException(new HttpResponseMessage(HttpStatusCode.NotFound));
			}
			throw new HttpResponseException(new HttpResponseMessage(HttpStatusCode.Forbidden));
		}
		[AllowAnonymous]
		[HttpPost]
		[Route("RegisterNewUser")]
		public IHttpActionResult RegisterNewUser(NewAccountModel newAccount)
		{
			try
			{
				IUser newUser = accountBusiness.SignUp(new User
				{
					UserName = newAccount.UserName,
					Email = newAccount.Email
				},
				new Credentials
				{
					Email = newAccount.Email,
					Password = newAccount.Password
				});
				//generate access token response
				var accessTokenResponse = GenerateLocalAccessTokenResponse(newUser);

				// Return success with the bearer token for authorized access
				return Ok(new { token = accessTokenResponse, profile = newUser });
			}
			catch (UserAlreadyRegisteredException)
			{

				return BadRequest("A user with this email address is already registered.");
			}

		}
		[AllowAnonymous]
		[HttpPost]
		[Route("SignIn")]
		public IHttpActionResult SignIn(SignInModel credentials)
		{
			try
			{
				// Validate credentials and return user
				IUser user = accountBusiness.SignIn(credentials.Email, credentials.Password);

				//generate access token response
				var accessTokenResponse = GenerateLocalAccessTokenResponse(user);

				// Return success with the bearer token for authorized access
				return Ok(new { token = accessTokenResponse, profile = user });
			}
			catch (InvalidEmailOrPasswordException)
			{
				throw new HttpResponseException(new HttpResponseMessage(HttpStatusCode.NotFound));
			}



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
			IUser user = repository.GetUser(verifiedAccessToken.user_id, provider);

			if (user == null)
			{
				// Register now
				user = accountBusiness.CreateNewUser(
					new User
					{
						AuthenticationUserId = verifiedAccessToken.user_id,
						AuthenticationProvider = provider,
						UserName = verifiedAccessToken.username
					});
			}

			//generate access token response
			var accessTokenResponse = GenerateLocalAccessTokenResponse(user);

			// Return success with the bearer token for authorized access
			return Ok(new { token = accessTokenResponse, profile = user });

		}

		/// <summary>
		/// Verify the access token supplied against the authentication provider
		/// </summary>
		/// <param name="provider"></param>
		/// <param name="accessToken"></param>
		/// <returns></returns>
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
		private JObject GenerateLocalAccessTokenResponse(IUser user)
		{

			var tokenExpiration = TimeSpan.FromDays(1);

			ClaimsIdentity identity = new ClaimsIdentity(OAuthDefaults.AuthenticationType);

			identity.AddClaim(new Claim(ClaimTypes.Name, user.UserName));
			identity.AddClaim(new Claim(ClaimTypes.NameIdentifier, user.Id));
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
										new JProperty("userId", user.Id),
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
