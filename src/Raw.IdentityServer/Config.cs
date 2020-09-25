using System.Collections.Generic;
using IdentityModel;
using IdentityServer4;
using IdentityServer4.Models;
using Raw.IdentityServer.Constants;

namespace Raw.IdentityServer
{
    public static class IdentityServerConfiguration
    {
        // An identity resource is a named group of claims that can be requested using the scope parameter.

        // IdentityResource olarak tanımlanan bilgiler (scope ve claim'leri) id_token içerisinde tutulur.
        //      Fakat varsayılan olarak bu claim'ler id_token içerisine direk yazılmazlar
        //          AlwaysIncludeUserClaimsInIdToken = true değilse... (alttaki nota bak) 
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
                    name: "secret", // 'scope' ismi ve scope ile alınabilecek claim'ler
                    userClaims: new []{ "secret.level", "secret.xp", "secret.mastery", "secret.path" })
            };

        // ApiScope/ApiResource olarak tanımlanan bilgiler access_token içerisinde tutulur.
        // "The value(s) of the audience claim will be the name of the API resource."
        public static IEnumerable<ApiResource> ApiResources 
            => new ApiResource[]
        {
            new ApiResource(name: "Raw.IdentityServer.Api2")
            {
                Scopes = new [] { "Raw.IdentityServer.Api2" }
            }
        };

        // Tanımlar access_token içerisinde tutulur.
        public static IEnumerable<ApiScope> ApiScopes =>
            new List<ApiScope>
            {
                new ApiScope("Raw.IdentityServer.Api2"),
                
                // Yeni bir scope tanımlıyorum. İsmi "area51":
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
                    "secret",
                    "area51"
                },

                // True olduğunda IdentityResource olarak tanımlanan claim'ler (user claims) de id_token içerisinde tutulur.
                // Normalde kullanıcı claim'leri için arkaplada userinfo enpoint'ine fazladan bir istek daha atılır.
                // Avantajı tek bir request ile bütün bilgileri alabilmek.
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
 *          
 *   
 * M isminde bir IdentityResource tanımlamak,
 * IdentityServer'a kullanılmaya müsait (available) bir M scope'u olduğunu bildirir.
 *
 * Bir Client'in o scope'a ulaşabilmesi için
 * Client.AllowedScopes koleksiyonuna M scope'u eklenmelidir.
 * Bu Client'in onu isteyebileceği anlamına gelir, istediği anlamına gelmez.
 *
 * Yani M scope bilgilerini istemek için Client'i temsil eden web app veya web api yazılım
 * tarafında bu istek belirtilmelidir (config.Scopes.Add("scope_ismi"); )
 *
 *
 *
 * id_token	    => Kullanıcının authentication bilgilerini tutar (AUTHZ)
 * access_token	=> Kullanıcının yetki bilgilerini tutar (AUTHC)
 *
 * id_token ve access_token çerez içerisinde tutuluyor
 *
 * Senin kimlik bilgilerini tutan çerez IdentityServer'in yazdığı çerez.
 * Diğerleri authentication söz konusu olduğunda hava cıva.
 * Diğerlerini silsen dahi, tekrar protected resource'a gitmek istediğinde,
 * IdentityServer çerezindeki bilgilerle, tabii server tarafında o anda authenticated durumdaysan,
 * diğer çerezler tekrar oluşturulur ve sana verilir.
 *
 * id_token içerisindeki claim'ler (ama hepsi değil) .NET tarafında deserialize edilip
 * .NET tarafındaki ClaimsPrincipal.Claims koleksiyonuna atılıyır.
 *
 *
 *
 * Bir claim'in id_token içerisinde bulunmasını istiyorsa IdentityResource olarak;
 * access_token içerisinde bulunmasını istiyorsan ApiResource/ApiScope olarak tanımla.
 */
