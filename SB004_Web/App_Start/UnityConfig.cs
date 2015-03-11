namespace SB004
{
    using Microsoft.Practices.Unity;
    using SB004.Data;
    using SB004.Utilities;
    using System.Web.Http;
    using Unity.WebApi;
    public static class UnityConfig
    {
        public static void RegisterComponents(HttpConfiguration config)
        {
			var container = new UnityContainer();

            container.RegisterType<IRepository, Repository>();
            container.RegisterType<IImageManager, ImageManager>();
            container.RegisterType<IDownloader, Downloader>();

            config.DependencyResolver = new UnityDependencyResolver(container);
        }
    }
}