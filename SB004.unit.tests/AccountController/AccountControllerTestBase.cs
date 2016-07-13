

namespace SB004.Unit.Tests.AccountController
{
    using NSubstitute;
    using SB004.Business;
    using SB004.Controllers;
    using SB004.Data;
    using System.Net.Http;
    using System.Web.Http;
    using System.Web.Http.Controllers;
    using System.Web.Http.Hosting;
    using System.Web.Http.Routing;

    public abstract class AccountControllerTestBase
    {
        protected IRepository repository;
        protected IAccountBusiness accountBusiness;
        protected AccountController accountController;

        protected void Initialize()
        {
            repository = Substitute.For<IRepository>();

            accountBusiness = Substitute.For<IAccountBusiness>();


            var config = new HttpConfiguration();
            var request = new HttpRequestMessage(HttpMethod.Post, "http://localhost/api/account");
            var route = config.Routes.MapHttpRoute("DefaultApi", "api/{controller}/{id}");
            var routeData = new HttpRouteData(route, new HttpRouteValueDictionary { { "controller", "account" } });

            accountController = new AccountController(repository, accountBusiness);
            accountController.ControllerContext = new HttpControllerContext(config, routeData, request);
            accountController.Request = request;
            accountController.Request.Properties[HttpPropertyKeys.HttpConfigurationKey] = config;
        }
    }
}
