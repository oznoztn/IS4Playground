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
                    // 1) authentication i�in ne kullanaca��z
                    //      CEVAP: Cookie, more specifically, OAuthBasics.Client.Cookie
                    //      We check the existence of a cookie called OAuthBasics.Client.Cookie
                    config.DefaultAuthenticateScheme = "OAuthBasics.Client.Cookie";

                    // 2) What we are saying is that when we sign-in we will deal with a cookie named ... [TODO: clarification needed]
                    //      CEVAP: This will also use a cookie, more specifically, the OAuthBasics.Client.Cookie.
                    config.DefaultSignInScheme = "OAuthBasics.Client.Cookie";

                    // 3) authorization nas�l yap�lacak
                    //      use this one to check if we are allowed to do something
                    // Bu �rnek �nceki �rneklerdeki gibi olsayd�, DefaultChallengeScheme OAuthBasics.Client.Cookie olurdu.
                    //      ve anonim bir kullan�c� authentication i�in bir login sayfas�na g�nderilirdi
                    //          bu �rnekte bu durum yok.
                    //
                    // (NOT: Bu prop'u o senaryolarda hi� set etmemi�tik)
                    //
                    // Burada DefaultChallengeScheme'i set ediyoruz
                    //      ��nk� challenge i�lemi i�in
                    //          olu�turdu�umuz authentication sunucusuna yani OAuthBasics.Server'a gidece�iz. 
                    //
                    // T�m bunlar OAuthBasics.Client.Cookie'yi almak i�in yap�l�yor!
                    //      Authentication i�i authentication sunucusuna aktar�ld� (delegation)
                    //
                    // Bu a�amaya gelindi�inde .AddOAuth() k�sm�ndaki mant�k devreye girecek.
                    // Dolay�s�yla .AddOAuth() i�erisinde authentication server bilgilerini tan�mlayaca��z
                    //      Scheme isimlerinin, OAuthBasics.Server, e�le�ti�ine dikkat et!
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
                    //      E�er �yleyse tabii ki Auth sunucusunda bulunan bir yer olacak:
                    oauthOptions.AuthorizationEndpoint = "https://localhost:44324/oauth/authorize";

                    // 2)
                    // Bu adres middleware'in i�erisinde bir yerde.
                    // Bir blackbox, i�eride ne oluyor bilmiyoruz. Bilmemize de gerek yok.
                    // Kullan�c� server taraf�ndan authenticated oldu�unda kullan�c�n�n redirect edildi�i adres.
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

                    // Auth sunucusundan ald���m�z access ve refresh (biz almad�k asl�nda) tokenlerinin saklanmas�:
                    oauthOptions.SaveTokens = true;
                    
                    // AUTH SERVER'DA OLU�TURDU�UMUZ TOKEN NEREDE?
                    // �u an token hi�bir yerde. Kayboldu. 
                    // Bizim bu �rnekteki TOKEN alma amac�m�z
                    //      onun arac�l���yla OAuthBasics.Client.Cookie �erezini alabillmekti SADECE!.
                    // �erezi ald�ktan sonra TOKEN ile i�imiz bitiyor. 
                    // Token i�erisindeki bilgiler i�ine yarayacaksa, �rn�in API call yapacaksan,
                    //      al�nan token'in client taraf�nda saklanmas� gerekti�i belirtilmelidir.

                    // Bu event'i handle etmemizin nedeni
                    // authentication sunucusundan d�nen token'den claim bilgilerini extract etmek
                    // ve bunlar� ClaimsPrincipal yani authenticated user i�erisinde tutmak.
                    // �yleki art�k application genelinde claim bilgileri eri�ilebilir olsun.
                    //
                    //      Bu �rnekte, buraya kadar olan k�sma kadar,
                    //          bu application daki authentication i�leminin
                    //              sunucudan al�nan access_token ile bir ba�lant�s� yoktu, UNUTMA!!!
                    //
                    // Bu da bunlar� bir �ereze yazmak demek oluyor. 
                    // ClaimsPrincipal i�in  tan�mlad���m�z claim'ler cookie'ye yaz�l�yordu, ilk dersleri hat�rla.
                    //
                    // Al�nan token'den claim bilgileri extract ediliyor ve authentic user'a ekleniyor.
                    // Bu a�amada hen�z cookie yaz�lm�� de�il.
                    oauthOptions.Events = new OAuthEvents
                    {
                        OnCreatingTicket = context =>
                        {
                            // 3 par�adan olu�uyordu bir JWT token. 
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
