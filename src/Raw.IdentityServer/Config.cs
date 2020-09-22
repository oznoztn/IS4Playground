using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using IdentityModel;
using IdentityServer4.Models;

namespace Raw.IdentityServer
{
    public static class Config
    {
        public static IEnumerable<ApiResource> ApiResources 
            => new ApiResource[]
        {
            // "The value(s) of the audience claim will be the name of the API resource."
            new ApiResource(name: "Raw.IdentityServer.Api2")
            {
                Scopes = new [] { "Raw.IdentityServer.Api2" }
            }, 
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
                
                ClientId = "Raw.IdentityServer.Api1.ClientId",
                ClientSecrets = new Secret[]
                {
                    new Secret("Raw.IdentityServer.Api1.ClientSecret".ToSha256())
                },

                AllowedScopes = new []
                {
                    "Raw.IdentityServer.Api2"
                },
            }, 
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
 */
