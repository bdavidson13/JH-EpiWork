using System.Web.Mvc;
using EPiServer.Framework;
using EPiServer.Framework.Initialization;
using EPiServer.ServiceLocation;
using AlloyDemo.Business.Rendering;
using EPiServer.Web.Mvc;
using EPiServer.Web.Mvc.Html;
using AlloyDemo.Managers;
using AlloyDemo.Stores;
using AlloyDemo.Profiles;
using AlloyDemo.IdProviders;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
namespace AlloyDemo.Business.Initialization
{
    [InitializableModule]
    public class DependencyResolverInitialization : IConfigurableModule
    {
        public void ConfigureContainer(ServiceConfigurationContext context)
        {
            //Implementations for custom interfaces can be registered here.
            //context.Services.AddScoped<IHttpContextAccessor, HttpContextAccessor>();

            context.ConfigurationComplete += (o, e) =>
            {
                //Register custom implementations that should be used in favour of the default implementations
                context.Services.AddTransient<IContentRenderer, ErrorHandlingContentRenderer>()
                    .AddTransient<ContentAreaRenderer, AlloyContentAreaRenderer>();
                context.Services.AddSingleton<IProfileManager, ProfileManager>();
                context.Services.AddSingleton<IProfileStore, ProfileStore>();
                context.Services.AddTransient<IProfile, DictionaryProfile>();
                context.Services.AddTransient<IIdProvider, CookieIdProvider>();

            };
        }

        public void Initialize(InitializationEngine context)
        {
            DependencyResolver.SetResolver(new ServiceLocatorDependencyResolver(context.Locate.Advanced));
        }

        public void Uninitialize(InitializationEngine context)
        {
        }

        public void Preload(string[] parameters)
        {
        }
    }
}
