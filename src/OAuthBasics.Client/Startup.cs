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
                    // 1) authentication i�in ne kullanaca��z
                    //      CEVAP: Cookie, more specifically, OAuthBasics.Client.Cookie
                    config.DefaultAuthenticateScheme = "OAuthBasics.Client.Cookie";

                    // 2) sign-in olundu�unda ne olacak
                    //      CEVAP: This will also use a cookie, more specifically, the OAuthBasics.Client.Cookie.
                    config.DefaultSignInScheme = "OAuthBasics.Client.Cookie";

                    // 3) authorization nas�l yap�lacak
                    //      use this one to check if we are allowed to do something
                    // Bu �rnek �ndeki �rneklerdeki gibi olsayd�, default challenge scheme OAuthBasics.Client.Cookie olurdu.
                    //      (NOT: Bu prop'u o senaryolarda hi� set etmemi�tik)
                    // Burada set ediyoruz ��nk� challenge i�lemi i�in bizim OAuthBasics.Server'a gidece�iz. 
                    //      OAuthBasics.Client.Cookie'yi almak i�in!
                    // Bu a�amaya geldi�inde art�k .AddOAuth() k�sm�ndaki mant�k devreye girecek.
                    // Ve orada oauth server bilgilerini tan�mlayaca��z
                    //      �simlerin, OAuthBasics.Server, e�le�ti�ine dikkat et!
                    config.DefaultChallengeScheme = "OAuthBasics.Server";
                })
                .AddCookie("OAuthBasics.Client.Cookie")
                .AddOAuth("OAuthBasics.Server", oauthOptions =>
                {
                    // REQUIREMENTS: CallbackPath, CliendId & ClientSecret, AuthorizationEndpoint, TokenEndpoint

                    oauthOptions.ClientId = "OAuthBasics_Client_Id";
                    oauthOptions.ClientSecret = "OAuthBasics_Client_Secret";

                    // 1)
                    // Buras� san�r�m bize Authorization Code'u verecek endpoint.
                    //      E�er �yleyse tabii ki Auth sunucusunda bulunan bir yer olacak:
                    oauthOptions.AuthorizationEndpoint = "https://localhost:44324/oauth/authorize";

                    // 2)
                    // Bu adres middleware'in i�erisinde bir yerde.
                    // Bir blackbox, i�eride ne oluyor bilmemize gerek yok.
                    // Kullan�c� server taraf�ndan authenticated oldu�unda kullan�c�n�n redired edildi�i adres.
                    // Tek bilmemiz gereken "code" ve "state" bilgisini i�leyen adres buras�
                    //      OAuthBasics.Server > OAuthorizeController.Authorize [POST] redirects here.
                    //      Buras� i�ini bitirdikten sonra access token i�in auth sunucusundaki
                    //          (altta TokenEndpoint'de belirtti�imiz)
                    //              token issue yapan endpointe istekte bulunuyor.
                    //                  Dok�mantasyona bakabilirsin.
                    oauthOptions.CallbackPath = "/oauth/callback"; 

                    // 3)
                    // Auth sunucusundan d�nen Authorization Kodu'na kar��l�k hangi endpoint'ten TOKEN alaca��z?
                    //      Tabii ki token issuer olan OAuth sunucusunda bir yere kar��l�k gelecek.
                    oauthOptions.TokenEndpoint = "https://localhost:44324/oauth/token";


                    // AUTH SERVER'DA OLU�TURDU�UMUZ TOKEN NEREDE?
                    // �u an token hi�bir yerde. Kayboldu. 
                    // Bizim bu �rnekteki TOKEN alma amac�m�z sadece onun arac�l���yla OAuthBasics.Client.Cookie �erezini alabillmekti.
                    // �erezi ald�ktan sonra TOKEN ile i�imiz bitiyor. 
                    // Dolay�s�yla �u an ald���m�z token bir yerde tutulmuyor. 
                    //      Token ile eri�ebilece�imiz bir API olsayd� tutabilirdik.
                    // Token i�erisindeki bilgiler i�ine yarayacaksa, al�nan token'in client taraf�nda saklanmas� gerekti�i belirtilmelidir.
                    // Bu prop bunu yap�yor.

                    // Auth sunucusundan ald���m�z access ve refresh (biz almad�k asl�nda) tokenlerinin saklanmas�:
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
