using Marvin.IDP.Entities;
using Marvin.IDP.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Authentication.Google;
using IdentityServer4;
using Microsoft.AspNetCore.Authentication.Cookies;

namespace Marvin.IDP
{
    public class Startup
    {
        public static IConfigurationRoot Configuration;

        public Startup(IHostingEnvironment environment)
        {
            var builder = new ConfigurationBuilder()
                    .SetBasePath(environment.ContentRootPath)
                    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                    .AddJsonFile($"appsettings.{environment.EnvironmentName}.json", optional: true, reloadOnChange: false)
                    .AddEnvironmentVariables();

            Configuration = builder.Build();

        }
        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {

            var connectionString = Configuration["connectionStrings:marvinUserDBConnectionString"];
            services.AddDbContext<MarvinUserContext>(o => o.UseSqlServer(connectionString));

            services.AddScoped<IMarvinUserRepository, MarvinUserRepository>();

            services.AddIdentityServer()
                .AddDeveloperSigningCredential()
                .AddMarvinUserStore()
                //.AddTestUsers(Config.GetUsers())
                .AddInMemoryIdentityResources(Config.GetIdentityResources())
                .AddInMemoryClients(Config.GetClients())
                .AddInMemoryApiResources(Config.GetApiResources());

            services.AddAuthentication() //CookieAuthenticationDefaults.AuthenticationScheme)
                .AddGoogle("Google", o =>
                {
                    o.ClientId = "428263776906-migfnplkenutb2c9k79gi6kemcu2eqh3.apps.googleusercontent.com";
                    o.ClientSecret = "IVdCZw7YM_m0YzNnHlE0aX91";
                    o.SignInScheme = IdentityServerConstants.ExternalCookieAuthenticationScheme;
                });
            services.AddMvc();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory, MarvinUserContext marvinUserContext)
        {
            loggerFactory.AddConsole();
            loggerFactory.AddDebug();

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            marvinUserContext.Database.Migrate();
            marvinUserContext.EnsureSeedDataForContext();

            
            app.UseIdentityServer();
          //  app.UseAuthentication();


            //app.UseGoogleAuthentication(new GoogleOptions
            //{
            //    ClientId = "428263776906-migfnplkenutb2c9k79gi6kemcu2eqh3.apps.googleusercontent.com",
            //    ClientSecret= "IVdCZw7YM_m0YzNnHlE0aX91",
            //    SignInScheme = IdentityServerConstants.ExternalCookieAuthenticationScheme
                
            //});
            app.UseStaticFiles();
            app.UseMvcWithDefaultRoute();
            
            //app.Run(async (context) =>
            //{
            //    await context.Response.WriteAsync("Hello World!");
            //});
        }
    }
}
