using Marvin.IDP.Services;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Marvin.IDP
{
    public static class IdentityServerBuilderExtensions
    {
        public static IIdentityServerBuilder AddMarvinUserStore(this IIdentityServerBuilder builder )
        {
            builder.Services.AddScoped<IMarvinUserRepository, MarvinUserRepository>();
            builder.AddProfileService<MarvinUserProfileService>();
            return builder;
        }
    }
}
