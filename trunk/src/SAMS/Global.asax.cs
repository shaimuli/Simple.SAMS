using System;
using System.Collections.Generic;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using SAMS.Controllers;
using Simple;
using Simple.Common;
using Simple.Common.Storage;
using Simple.ComponentModel;
using Simple.SAMS.Competitions.Data;
using Simple.SAMS.Competitions.Services;
using Simple.SAMS.Contracts;
using Simple.SAMS.Contracts.Competitions;
using Simple.SAMS.Contracts.Users;
using Simple.Utilities;

namespace SAMS
{
    // Note: For instructions on enabling IIS6 or IIS7 classic mode, 
    // visit http://go.microsoft.com/?LinkId=9394801

    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            SystemMonitor.Info("SAMS Application started... ");

            AreaRegistration.RegisterAllAreas();

            WebApiConfig.Register(GlobalConfiguration.Configuration);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
            AuthConfig.RegisterAuth();
            
            RegisterConfigurationProvider();
            RegisterRepositories();

            SystemMonitor.Info("SAMS Application initialization completed.");
        }

        protected void Application_Error(object sender, EventArgs e)
        {
            var httpContext = ((MvcApplication)sender).Context;

            var currentRouteData = RouteTable.Routes.GetRouteData(new HttpContextWrapper(httpContext));
            var currentController = " ";
            var currentAction = " ";

            if (currentRouteData != null)
            {
                if (currentRouteData.Values["controller"] != null && !String.IsNullOrEmpty(currentRouteData.Values["controller"].ToString()))
                {
                    currentController = currentRouteData.Values["controller"].ToString();
                }

                if (currentRouteData.Values["action"] != null && !String.IsNullOrEmpty(currentRouteData.Values["action"].ToString()))
                {
                    currentAction = currentRouteData.Values["action"].ToString();
                }
            }

            var ex = Server.GetLastError();

            var controller = new ErrorController();
            var routeData = new RouteData();
            var action = "Index";

            if (ex is HttpException)
            {
                var httpEx = ex as HttpException;

                switch (httpEx.GetHttpCode())
                {
                    case 404:
                        action = "NotFound";
                        break;

                    // others if any

                    default:
                        action = "Index";
                        break;
                }
            }

            httpContext.ClearError();
            httpContext.Response.Clear();
            httpContext.Response.StatusCode = ex is HttpException ? ((HttpException)ex).GetHttpCode() : 500;
            httpContext.Response.TrySkipIisCustomErrors = true;
            routeData.Values["controller"] = "Error";
            routeData.Values["action"] = action;

            controller.ViewData.Model = new HandleErrorInfo(ex, currentController, currentAction);
            ((IController)controller).Execute(new RequestContext(new HttpContextWrapper(httpContext), routeData));
        } 

        private void RegisterConfigurationProvider()
        {
            var configurationKeys = new Dictionary<string, ConfigurationPropertyMetadata[]>()
                                        {
                                            {
                                                "Storage",
                                                new[]
                                                    {
                                                        new ConfigurationPropertyMetadata
                                                            {Name = "Provider", Type = typeof (string)},
                                                        new ConfigurationPropertyMetadata
                                                            {Name = "Storage", Type = typeof (string)}
                                                    }
                                            }
                                        };

            var configurationProvider =
                new SimpleConfigurationProvider(configurationKeys);

            ServiceProvider.Add<IConfigurationProvider>(configurationProvider);
        }

        private void RegisterRepositories()
        {
            ServiceProvider.Add<IUsersRepository, UsersRepository>();
            ServiceProvider.Add<ICompetitionRepository, CompetitionRepository>();
            ServiceProvider.Add<ICompetitionTypeRepository, CompetitionTypeRepository>();
            ServiceProvider.Add<ICompetitionMatchesRepository, CompetitionMatchesRepository>();
            ServiceProvider.Add<IPlayersRepository, PlayerRepository>();

            
            ServiceProvider.Add<ICompetitionsEngine, CompetitionEngineService>();
            ServiceProvider.Add<ICompetitionsManager, CompetitionsManagerService>();
            ServiceProvider.Add<IRemoteStorageProviderFactory, SimpleRemoteStorageProviderFactory>();

            ServiceProvider.Add<IMatchProvisioningEngineFactory, DefaultMatchProvisioningEngineFactory>();
        }
    }
}