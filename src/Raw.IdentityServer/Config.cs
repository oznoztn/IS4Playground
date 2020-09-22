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
        public static IEnumerable<ApiScope> ApiScopes =>
            new List<ApiScope>
            {
                new ApiScope("Raw.IdentityServer.Api2"),
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
