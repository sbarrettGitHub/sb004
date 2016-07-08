using SB004.Domain;

namespace SB004
{
    using Microsoft.Practices.Unity;
    using SB004.Business;
    using SB004.Data;
    using SB004.Utilities;
    using System.Web.Http;
    using Unity.WebApi;
    public static class UnityConfig
    {
        public static void RegisterComponents(HttpConfiguration config)
        {
			var container = new UnityContainer();

            container.RegisterType<IRepository, Repository>(new ContainerControlledLifetimeManager());
            container.RegisterType<IImageManager, ImageManager>();
            container.RegisterType<IDownloader, Downloader>();
            container.RegisterType<IMemeBusiness, MemeBusiness>();
            container.RegisterType<IAccountBusiness, AccountBusiness>();
			container.RegisterType<IUserCommentBusiness, UserCommentBusiness>();
			container.RegisterType<IHashTagBusiness, HashTagBusiness>();
			container.RegisterType<IConfiguration, Configuration>();
            container.RegisterType<INotification, Notification>();
            config.DependencyResolver = new UnityDependencyResolver(container);
        }
    }
}