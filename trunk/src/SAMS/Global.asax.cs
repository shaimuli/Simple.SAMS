using System.Collections.Generic;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using Simple.Common;
using Simple.Common.Storage;
using Simple.ComponentModel;
using Simple.SAMS.Competitions.Data;
using Simple.SAMS.Competitions.Services;
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
            AreaRegistration.RegisterAllAreas();

            WebApiConfig.Register(GlobalConfiguration.Configuration);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
            AuthConfig.RegisterAuth();
            
            RegisterConfigurationProvider();
            RegisterRepositories();
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
            ServiceProvider.Add<IPlayersRepository, PlayerRepository>();

            ServiceProvider.Add<ICompetitionsEngine, CompetitionEngineService>();
            ServiceProvider.Add<ICompetitionsManager, CompetitionsManagerService>();
            ServiceProvider.Add<IRemoteStorageProviderFactory, SimpleRemoteStorageProviderFactory>();
        }
    }
}