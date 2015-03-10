using Owin;
using System.Web.Http;

namespace SB004
{
  // Note: By default all requests go through this OWIN pipeline. Alternatively you can turn this off by adding an appSetting owin:AutomaticAppStartup with value “false”. 
  // With this turned off you can still have OWIN apps listening on specific routes by adding routes in global.asax file using MapOwinPath or MapOwinRoute extensions on RouteTable.Routes
  public class Startup
  {

    // Invoked once at startup to configure your application.
    public void Configuration(IAppBuilder app)
    {
      
      HttpConfiguration config = new HttpConfiguration();
      
      WebApiConfig.Register(config);

      app.UseWebApi(config);
    }
  }
}