using IdentityServer4.Models;
using IdentityServer4.Test;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Marvin.IDP
{
    public static class Config
    {
        public static List<TestUser> GetUsers()
        {
            return new List<TestUser>
            {
               new TestUser
                {
                    SubjectId = "d860efca-22d9-47fd-8249-791ba61b07c7",
                    Username = "Frank",
                    Password = "password",

                    Claims = new List<Claim>
                    {
                        new Claim("given_name", "Frank"),
                        new Claim("family_name", "Underwood"),
                        new Claim("address", "234 South Street"),
                        new Claim(ClaimTypes.Role, "FreeUser"),
                        new Claim("subscriptionlevel", "FreeUser"),
                        new Claim("country", "nl")
                    }
                },
                new TestUser
                {
                    SubjectId = "b7539694-97e7-4dfe-84da-b4256e1ff5c7",
                    Username = "Claire",
                    Password = "password",

                    Claims = new List<Claim>
                    {
                        new Claim("given_name", "Claire"),
                        new Claim("family_name", "Underwood"),
                        new Claim("address", "123 Main Street"),
                        new Claim(ClaimTypes.Role, "PaidUser"),
                        new Claim("subscriptionlevel", "PaidUser"),
                        new Claim("country", "be")
                    }
                }
            };
        }

        public static IEnumerable<IdentityResource> GetIdentityResources()
        {
            return new List<IdentityResource>
            {
                new IdentityResources.OpenId(),
                new IdentityResources.Profile(),
                new IdentityResources.Address(),
                new IdentityResource("roles","Your role(s)",new List<string>{ ClaimTypes.Role}),
                new IdentityResource("country","The country you're living in",new List<string>{"country"}),
                new IdentityResource("subscriptionlevel", "Your subscription level", new List<string> {"subscriptionlevel"})
            };
        }

        public static IEnumerable<ApiResource> GetApiResources()
        {
            return new List<ApiResource>
            {
                new ApiResource("imagegalleryapi", "Image Gallery API", new List<string>{ClaimTypes.Role })
                {
                    ApiSecrets = {new Secret("apisecret".Sha256())}
                }
            };
        }

        public static IEnumerable<Client> GetClients()
        {
            return new List<Client>
            {
                new Client
                {
                    
                    
                    ClientName = "Image Gallery",
                    ClientId = "imagegalleryclient",
                    ClientSecrets =
                    {
                        new Secret("secret".Sha256())
                    },
                    AccessTokenType = AccessTokenType.Reference,
                    AllowedGrantTypes = GrantTypes.Hybrid,
                    RequireConsent = false,
                    AccessTokenLifetime = 120,
                    RedirectUris = { "https://localhost:44363/signin-oidc" },
                    PostLogoutRedirectUris = { "https://localhost:44363/signout-callback-oidc" },
                    AllowOfflineAccess = true,
                    AllowedScopes = new List<string>
                    {
                        IdentityServer4.IdentityServerConstants.StandardScopes.OpenId,
                        IdentityServer4.IdentityServerConstants.StandardScopes.Profile,
                        IdentityServer4.IdentityServerConstants.StandardScopes.Address,
                        "roles",
                        "imagegalleryapi",
                        "country",
                        "subscriptionlevel"
                    },
                    AlwaysIncludeUserClaimsInIdToken = true
                }
            };
        }
    }
}
