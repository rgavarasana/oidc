using ImageGallery.Client.Services;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Threading.Tasks;
using System.IdentityModel.Tokens.Jwt;

namespace ImageGallery.Client
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc();

            JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();

            services.AddAuthentication(options =>
                {
                    options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                    options.DefaultChallengeScheme = "oidc";                  
                })
                .AddCookie("Cookies", options=>
                {
                    options.AccessDeniedPath = "/Authorization/AccessDenied";
                })
                
                .AddOpenIdConnect("oidc", options =>
                {
                    options.SignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                    options.Authority = "https://localhost:44356/";
                    options.RequireHttpsMetadata = true;
                    options.ClientId = "imagegalleryclient";                    
                    options.SaveTokens = true;
                    options.ResponseType = "code id_token";
                    options.ClientSecret = "secret";
                    options.GetClaimsFromUserInfoEndpoint = true;
                    
                    options.Scope.Add("offline_access");
                    options.Scope.Add("address");
                    options.Scope.Add("roles");                    
                    options.Scope.Add("imagegalleryapi");
                    options.Scope.Add("country");
                    options.Scope.Add("subscriptionlevel");

                    //options.Events = new OpenIdConnectEvents()
                    //{
                    //    //OnTokenValidated = tokenValidatedContext =>
                    //    //{

                    //    //    var identity = tokenValidatedContext.Principal.Identity as ClaimsIdentity;

                    //    //    var subjectClaim = identity.Claims.FirstOrDefault(c => c.Type == "sid");

                    //    //    var newClaimsIdentity = new ClaimsIdentity(tokenValidatedContext.Scheme.Name, "given_name", "role");
                    //    //    newClaimsIdentity.AddClaim(subjectClaim);

                    //    //    var newPrincipal = new ClaimsPrincipal(newClaimsIdentity);

                    //    //    tokenValidatedContext = new TokenValidatedContext(tokenValidatedContext.HttpContext, tokenValidatedContext.Scheme, tokenValidatedContext.Options, newPrincipal, tokenValidatedContext.Properties);

                    //    //    return Task.CompletedTask;
                    //    //},

                    //    OnUserInformationReceived = userInfoReceivedContext =>
                    //    {

                    //        userInfoReceivedContext.User.Remove("address");
                    //        return Task.CompletedTask;
                    //    }

                    //};
                });

            services.AddAuthorization(authorizationOptions =>
            {
                authorizationOptions.AddPolicy("CanOrderFrame", policyBuilder =>
                {
                    policyBuilder.RequireAuthenticatedUser();
                    policyBuilder.RequireClaim("country", "be");
                    policyBuilder.RequireClaim("subscriptionlevel", "PaidUser");
                });
            });

            // register an IHttpContextAccessor so we can access the current
            // HttpContext in services by injecting it
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

            // register an IImageGalleryHttpClient
            services.AddScoped<IImageGalleryHttpClient, ImageGalleryHttpClient>();

            
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseBrowserLink();
            }
            else
            {
                app.UseExceptionHandler("/Shared/Error");
            }

            app.UseAuthentication();
                


            app.UseStaticFiles();

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Gallery}/{action=Index}/{id?}");
            });

            //app.UseMvc(routes =>
            //{
            //    routes.MapRoute(
            //        name: "default",
            //        template: "{controller=Home}/{action=Index}/{id?}");
            //});
        }
    }
}
