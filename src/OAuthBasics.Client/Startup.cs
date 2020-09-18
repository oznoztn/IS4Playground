using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

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
                    config.DefaultAuthenticateScheme = "OAuthBasics.Client.Cookie";

                    // 2) sign-in olunduðunda ne olacak
                    //      CEVAP: This will also use a cookie, more specifically, the OAuthBasics.Client.Cookie.
                    config.DefaultSignInScheme = "OAuthBasics.Client.Cookie";

                    // 3) authorization nasýl yapýlacak
                    //      use this one to check if we are allowed to do something
                    // Bu örnek öndeki örneklerdeki gibi olsaydý, default challenge scheme OAuthBasics.Client.Cookie olurdu.
                    //      (NOT: Bu prop'u o senaryolarda hiç set etmemiþtik)
                    // Burada set ediyoruz çünkü challenge iþlemi için bizim OAuthBasics.Server'a gideceðiz. 
                    //      OAuthBasics.Client.Cookie'yi almak için!
                    // Bu aþamaya geldiðinde artýk .AddOAuth() kýsmýndaki mantýk devreye girecek.
                    // Ve orada oauth server bilgilerini tanýmlayacaðýz
                    //      Ýsimlerin, OAuthBasics.Server, eþleþtiðine dikkat et!
                    config.DefaultChallengeScheme = "OAuthBasics.Server";
                })
                .AddCookie("OAuthBasics.Client.Cookie")
                .AddOAuth("OAuthBasics.Server", oauthOptions =>
                {
                    // REQUIREMENTS: CallbackPath, CliendId & ClientSecret, AuthorizationEndpoint, TokenEndpoint

                    oauthOptions.ClientId = "OAuthBasics_Client_Id";
                    oauthOptions.ClientSecret = "OAuthBasics_Client_Secret";

                    // 1)
                    // Burasý sanýrým bize Authorization Code'u verecek endpoint.
                    //      Eðer öyleyse tabii ki Auth sunucusunda bulunan bir yer olacak:
                    oauthOptions.AuthorizationEndpoint = "https://localhost:44324/oauth/authorize";

                    // 2)
                    // Bu adres middleware'in içerisinde bir yerde.
                    // Bir blackbox, içeride ne oluyor bilmemize gerek yok.
                    // Kullanýcý server tarafýndan authenticated olduðunda kullanýcýnýn redired edildiði adres.
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


                    // AUTH SERVER'DA OLUÞTURDUÐUMUZ TOKEN NEREDE?
                    // Þu an token hiçbir yerde. Kayboldu. 
                    // Bizim bu örnekteki TOKEN alma amacýmýz sadece onun aracýlýðýyla OAuthBasics.Client.Cookie çerezini alabillmekti.
                    // Çerezi aldýktan sonra TOKEN ile iþimiz bitiyor. 
                    // Dolayýsýyla þu an aldýðýmýz token bir yerde tutulmuyor. 
                    //      Token ile eriþebileceðimiz bir API olsaydý tutabilirdik.
                    // Token içerisindeki bilgiler iþine yarayacaksa, alýnan token'in client tarafýnda saklanmasý gerektiði belirtilmelidir.
                    // Bu prop bunu yapýyor.

                    // Auth sunucusundan aldýðýmýz access ve refresh (biz almadýk aslýnda) tokenlerinin saklanmasý:
                    oauthOptions.SaveTokens = true;
                });

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
