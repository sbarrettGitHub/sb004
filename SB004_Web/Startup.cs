

namespace SB004
{
  using System.Web.Http;
  using Owin;
  using Microsoft.Owin.Security.OAuth;
  // Note: By default all requests go through this OWIN pipeline. Alternatively you can turn this off by adding an appSetting owin:AutomaticAppStartup with value “false”. 
    // With this turned off you can still have OWIN apps listening on specific routes by adding routes in global.asax file using MapOwinPath or MapOwinRoute extensions on RouteTable.Routes
    public class Startup
    {
        public static OAuthBearerAuthenticationOptions OAuthBearerOptions { get; private set; }

        // Invoked once at startup to configure your application.
        public void Configuration(IAppBuilder app)
        {

            HttpConfiguration config = new HttpConfiguration();
          
            ConfigureOAuth(app);

            UnityConfig.RegisterComponents(config);

            WebApiConfig.Register(config);

            app.UseWebApi(config);
        }

        private void ConfigureOAuth(IAppBuilder app)
        {
          OAuthBearerOptions = new OAuthBearerAuthenticationOptions();
          //Token Consumption
          app.UseOAuthBearerAuthentication(OAuthBearerOptions);
        }
    }
}