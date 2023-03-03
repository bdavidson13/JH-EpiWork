using System;
using System.Web;
using EPiServer.Cms.UI.AspNetIdentity;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin;
using Microsoft.Owin.Security.Cookies;
using Owin;
using AlloyDemo.Features.RegisterPersonas;
using AlloyDemo.Features.ResetAdmin;
using StructureMap;
using StructureMap.Pipeline;
using AlloyDemo.Managers;
using AlloyDemo.Stores;
using AlloyDemo.Profiles;
using AlloyDemo.IdProviders;

[assembly: OwinStartup(typeof(AlloyDemo.Startup))]

namespace AlloyDemo
{
    public class MyRegistry : Registry
    {
        public MyRegistry()
        {
            //For<ISample>().Use<Sample>().ContainerScoped();
            
            For<Microsoft.AspNetCore.Http.IHttpContextAccessor>().Use<Microsoft.AspNetCore.Http.HttpContextAccessor>().ContainerScoped();

            //For<IProfileStore>().Use<ProfileStore>().ContainerScoped();
            //For<IProfile>().Use<DictionaryProfile>().ContainerScoped();
            //For<IIdProvider>().Use<CookieIdProvider>().ContainerScoped();
            //For<IProfileManager>().Use<ProfileManager>().ContainerScoped();
            //For<ProfileManagerOptions>();
            //For<CookieIdProviderOptions>();


            //if (options != null)
            //{
            //    services.Configure<ProfileManagerOptions>(options);
            //}

            //services.AddCookieIdProvider();

            //return services;
        }
    }
    public class Startup
    {

        public void Configuration(IAppBuilder app)
        {
            var container = new Container(x =>
            {
                x.AddRegistry<MyRegistry>();
            });
            // Add CMS integration for ASP.NET Identity
            app.AddCmsAspNetIdentity<ApplicationUser>();

            // Remove to block registration of administrators
            app.UseAdministratorRegistrationPage(() => HttpContext.Current.Request.IsLocal);

            // Use cookie authentication
            app.UseCookieAuthentication(new CookieAuthenticationOptions
            {
                AuthenticationType = DefaultAuthenticationTypes.ApplicationCookie,
                LoginPath = new PathString(Global.LoginPath),
                Provider = new CookieAuthenticationProvider
                {
                    // If the "/util/login.aspx" has been used for login otherwise you don't need it you can remove OnApplyRedirect.
                    OnApplyRedirect = cookieApplyRedirectContext =>
                    {
                        app.CmsOnCookieApplyRedirect(cookieApplyRedirectContext, cookieApplyRedirectContext.OwinContext.Get<ApplicationSignInManager<ApplicationUser>>());
                    },

                    // Enables the application to validate the security stamp when the user logs in.
                    // This is a security feature which is used when you change a password or add an external login to your account.
                    OnValidateIdentity = SecurityStampValidator.OnValidateIdentity<ApplicationUserManager<ApplicationUser>, ApplicationUser>(
                        validateInterval: TimeSpan.FromMinutes(30),
                        regenerateIdentity: (manager, user) => manager.GenerateUserIdentityAsync(user))
                }
            });

            //app.UseRegisterPersonas(() => HttpContext.Current.Request.IsLocal);
            //app.UseResetAdmin(() => HttpContext.Current.Request.IsLocal);
        }
    }
}
