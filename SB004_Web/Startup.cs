

namespace SB004
{
    using Microsoft.Practices.Unity;
    using Owin;
    using SB004.Data;
    using SB004.Utilities;
    using System.Web.Http;
    using Unity.WebApi;
    // Note: By default all requests go through this OWIN pipeline. Alternatively you can turn this off by adding an appSetting owin:AutomaticAppStartup with value “false”. 
    // With this turned off you can still have OWIN apps listening on specific routes by adding routes in global.asax file using MapOwinPath or MapOwinRoute extensions on RouteTable.Routes
    public class Startup
    {

        // Invoked once at startup to configure your application.
        public void Configuration(IAppBuilder app)
        {

            HttpConfiguration config = new HttpConfiguration();

            UnityConfig.RegisterComponents(config);

            WebApiConfig.Register(config);

            app.UseWebApi(config);
        }
    }
}