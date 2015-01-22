
namespace SB004
{
  using SB004.Utilities;
using Microsoft.Practices.Unity;
using SB004.Data;
using System.Web.Http;
  public class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            var container = new UnityContainer();

            container.RegisterType<IIdManager, IdManager>(new HierarchicalLifetimeManager());
            container.RegisterType<IDownloader, Downloader>(new HierarchicalLifetimeManager());
            container.RegisterType<IImageManager, ImageManager>(new HierarchicalLifetimeManager());
            container.RegisterType<IRepository, Repository>(new HierarchicalLifetimeManager());
            
            config.DependencyResolver = new UnityResolver(container);
            
            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional }

            );
        }
    }
}