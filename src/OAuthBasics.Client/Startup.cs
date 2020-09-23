using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.OAuth;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json;

namespace OAuthBasics.Client
{
    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services
                .AddAuthentication(config =>
                {
                    // 1) authentication için ne kullanacaðýz
                    //      CEVAP: Cookie, more specifically, OAuthBasics.Client.Cookie
                    //      We check the existence of a cookie called OAuthBasics.Client.Cookie
                    config.DefaultAuthenticateScheme = "OAuthBasics.Client.Cookie";

                    // 2) What we are saying is that when we sign-in we will deal with a cookie named ... [TODO: clarification needed]
                    //      CEVAP: This will also use a cookie, more specifically, the OAuthBasics.Client.Cookie.
                    config.DefaultSignInScheme = "OAuthBasics.Client.Cookie";

                    // 3) authorization nasýl yapýlacak
                    //      use this one to check if we are allowed to do something
                    // Bu örnek önceki örneklerdeki gibi olsaydý, DefaultChallengeScheme OAuthBasics.Client.Cookie olurdu.
                    //      ve anonim bir kullanýcý authentication için bir login sayfasýna gönderilirdi
                    //          bu örnekte bu durum yok.
                    //
                    // (NOT: Bu prop'u o senaryolarda hiç set etmemiþtik)
                    //
                    // Burada DefaultChallengeScheme'i set ediyoruz
                    //      çünkü challenge iþlemi için
                    //          oluþturduðumuz authentication sunucusuna yani OAuthBasics.Server'a gideceðiz. 
                    //
                    // Tüm bunlar OAuthBasics.Client.Cookie'yi almak için yapýlýyor!
                    //      Authentication iþi authentication sunucusuna aktarýldý (delegation)
                    //
                    // Bu aþamaya gelindiðinde .AddOAuth() kýsmýndaki mantýk devreye girecek.
                    // Dolayýsýyla .AddOAuth() içerisinde authentication server bilgilerini tanýmlayacaðýz
                    //      Scheme isimlerinin, OAuthBasics.Server, eþleþtiðine dikkat et!
                    config.DefaultChallengeScheme = "OAuthBasics.Server";
                })
                .AddCookie("OAuthBasics.Client.Cookie")
                .AddOAuth("OAuthBasics.Server", oauthOptions =>
                {
                    // REQUIREMENTS of OAuth:
                    // CallbackPath, CliendId & ClientSecret, AuthorizationEndpoint, TokenEndpoint

                    oauthOptions.ClientId = "OAuthBasics_Client_Id";
                    oauthOptions.ClientSecret = "OAuthBasics_Client_Secret";

                    // 1)
                    // AuthorizationEndpoint bize Authorization Code'u verecek endpoint.
                    //      Eðer öyleyse tabii ki Auth sunucusunda bulunan bir yer olacak:
                    oauthOptions.AuthorizationEndpoint = "https://localhost:44324/oauth/authorize";

                    // 2)
                    // Bu adres middleware'in içerisinde bir yerde.
                    // Bir blackbox, içeride ne oluyor bilmiyoruz. Bilmemize de gerek yok.
                    // Kullanýcý server tarafýndan authenticated olduðunda kullanýcýnýn redirect edildiði adres.
                    // Tek bilmemiz gereken "code" ve "state" bilgisini iþleyen adres burasý
                    //      OAuthBasics.Server > OAuthorizeController.Authorize [POST] redirects here.
                    //      Burasý iþini bitirdikten sonra access token için auth sunucusundaki
                    //          (altta TokenEndpoint'de belirttiðimiz)
                    //              token issue yapan endpointe istekte bulunuyor.
                    //                  Dokümantasyona bakabilirsin.
                    oauthOptions.CallbackPath = "/oauth/callback"; 

                    // 3)
                    // Auth sunucusundan dönen Authorization Kodu'na karþýlýk hangi endpoint'ten TOKEN alacaðýz?
                    //      Tabii ki token issuer olan OAuth sunucusunda bir yere karþýlýk gelecek.
                    oauthOptions.TokenEndpoint = "https://localhost:44324/oauth/token";

                    // Auth sunucusundan aldýðýmýz access ve refresh (biz almadýk aslýnda) tokenlerinin saklanmasý:
                    oauthOptions.SaveTokens = true;
                    
                    // AUTH SERVER'DA OLUÞTURDUÐUMUZ TOKEN NEREDE?
                    // Þu an token hiçbir yerde. Kayboldu. 
                    // Bizim bu örnekteki TOKEN alma amacýmýz
                    //      onun aracýlýðýyla OAuthBasics.Client.Cookie çerezini alabillmekti SADECE!.
                    // Çerezi aldýktan sonra TOKEN ile iþimiz bitiyor. 
                    // Token içerisindeki bilgiler iþine yarayacaksa, örnðin API call yapacaksan,
                    //      alýnan token'in client tarafýnda saklanmasý gerektiði belirtilmelidir.

                    // Bu event'i handle etmemizin nedeni
                    // authentication sunucusundan dönen token'den claim bilgilerini extract etmek
                    // ve bunlarý ClaimsPrincipal yani authenticated user içerisinde tutmak.
                    // Öyleki artýk application genelinde claim bilgileri eriþilebilir olsun.
                    //
                    //      Bu örnekte, buraya kadar olan kýsma kadar,
                    //          bu application daki authentication iþleminin
                    //              sunucudan alýnan access_token ile bir baðlantýsý yoktu, UNUTMA!!!
                    //
                    // Bu da bunlarý bir çereze yazmak demek oluyor. 
                    // ClaimsPrincipal için  tanýmladýðýmýz claim'ler cookie'ye yazýlýyordu, ilk dersleri hatýrla.
                    //
                    // Alýnan token'den claim bilgileri extract ediliyor ve authentic user'a ekleniyor.
                    // Bu aþamada henüz cookie yazýlmýþ deðil.
                    oauthOptions.Events = new OAuthEvents
                    {
                        OnCreatingTicket = context =>
                        {
                            // 3 parçadan oluþuyordu bir JWT token. 
                            // metadata | claims, etc. | validation stuff
                            string infoPartBase64 = context.AccessToken.Split('.')[1];
                            byte[] infoPartDecoded = Convert.FromBase64String(infoPartBase64);
                            string infoPart = Encoding.UTF8.GetString(infoPartDecoded);

                            Dictionary<string, string> claimsDictionary = 
                                JsonConvert.DeserializeObject<Dictionary<string, string>>(infoPart);
                            
                            Claim[] claims = 
                                claimsDictionary
                                    .Select(t => new Claim(t.Key, t.Value))
                                    .ToArray();
                            
                            context.Identity.AddClaims(claims);

                            return Task.CompletedTask;
                        }
                    };
                });

            services.AddHttpClient();
            services.AddControllersWithViews();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapDefaultControllerRoute();
            });
        }
    }
}
