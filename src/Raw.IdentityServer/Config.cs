using System.Collections.Generic;
using IdentityModel;
using IdentityServer4;
using IdentityServer4.Models;
using Raw.IdentityServer.Constants;

namespace Raw.IdentityServer
{
    public static class IdentityServerConfiguration
    {
        public static IEnumerable<IdentityResource> IdentityResources
            => new IdentityResource[]
            {
                //new IdentityResources.OpenId(),
                //new IdentityResources.Profile(), 
                new IdentityResource(
                    name: "openid",
                    userClaims: new []{ "sub" }),
                new IdentityResource(
                    name: "profile",
                    userClaims: new []{ "name", "email", "website" }),
                new IdentityResource(
                    name: "secret",
                    userClaims: new []{ "secret.level", "secret.xp", "secret.mastery", "secret.path" })
            };

        public static IEnumerable<ApiResource> ApiResources 
            => new ApiResource[]
        {
            new ApiResource(name: "Raw.IdentityServer.Api2")
            {
                Scopes = new [] { "Raw.IdentityServer.Api2" }
            }
        };

        public static IEnumerable<ApiScope> ApiScopes =>
            new List<ApiScope>
            {
                new ApiScope("Raw.IdentityServer.Api2"),
                
                new ApiScope("area51", new List<string>
                {
                    "area51.access_level",
                    "area51.department"
                }) 
            };

        public static IEnumerable<Client> GetClients => new Client[]
        {
            new Client()
            {
                AllowedGrantTypes = GrantTypes.ClientCredentials,
                
                ClientId = RawClientId.Api1,
                ClientSecrets = new Secret[]
                {
                    new Secret(RawClientSecret.Api1.ToSha256())
                },

                AllowedScopes = { "Raw.IdentityServer.Api2" }
            },

            new Client()
            {
                AllowedGrantTypes = GrantTypes.Code,
                ClientId = RawClientId.Mvc,
                ClientSecrets = 
                {
                    new Secret(RawClientSecret.Mvc.ToSha256())
                },
                RedirectUris = { "https://localhost:44399/signin-oidc" },
                PostLogoutRedirectUris =
                {
                    "https://localhost:44399/signout-callback-oidc",
                    "https://localhost:44399/good-bye"
                },
                AllowedScopes =
                {
                    IdentityServerConstants.StandardScopes.OpenId, // "openid", 
                    IdentityServerConstants.StandardScopes.Profile, // "profile", 
                    "Raw.IdentityServer.Api2",
                    "secret",
                    "area51"
                },

                AlwaysIncludeUserClaimsInIdToken = false
            }
        };
    }
}