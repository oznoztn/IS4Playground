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
                    name: "secret", // 'scope' ismi
                    userClaims: new []{ "secret.level", "secret.xp", "secret.mastery", "secret.path" }), // bu scope'a bağlı claim'ler
            };

        public static IEnumerable<ApiResource> ApiResources 
            => new ApiResource[]
        {
            // "The value(s) of the audience claim will be the name of the API resource."
            new ApiResource(name: "Raw.IdentityServer.Api2")
            {
                Scopes = new [] { "Raw.IdentityServer.Api2" }
            }
        };

        public static IEnumerable<ApiScope> ApiScopes =>
            new List<ApiScope>
            {
                new ApiScope("Raw.IdentityServer.Api2")
            };

        public static IEnumerable<Client> GetClients => new Client[]
        {
            new Client()
            {
                // Client Credentials flow type is suitable for machine to machine communication
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
                PostLogoutRedirectUris = { "https://localhost:44399/signout-callback-oidc" },
                AllowedScopes =
                {
                    IdentityServerConstants.StandardScopes.OpenId, // "openid", 
                    IdentityServerConstants.StandardScopes.Profile, // "profile", 
                    "Raw.IdentityServer.Api2",
                    "secret"
                },

                // İstenen scope ile ilgili bilgiler (Claim'ler) id_token içerisinde tutulur.
                // Normalde kullanıcı claim'leri arkaplada userinfo enpoint'ine istek atılarak elde edilir.
                // Avantajı iki ayrı noktaya request atmak yerine (network latency) te noktadan bütün bilgileri alabilmek.
                // Dezavantajı id_token'ın şişmesi.
                AlwaysIncludeUserClaimsInIdToken = true
            }
        };
    }
}

/*
 *          On ApiResource & ApiScope
 *
 * To migrate to v4 you need to split up scope and resource registration,
 * typically by first registering all your scopes (e.g. using the AddInMemoryApiScopes method),
 * and then register the API 1resources (if any) afterwards.
 * The API resources will then reference the prior registered scopes by name.
 *
 * OpenID Connect protokolünü kullanacak olan client'ın 'openid' scope'una erişebiliyor olması gerekir.
 * Dolayısıyla bu scope için IS tarafında tanımlama yapmayı
 * ve Client'a scope için erişim izni vermeyi unutma (Client.AllowedScopes)
 *
 */
