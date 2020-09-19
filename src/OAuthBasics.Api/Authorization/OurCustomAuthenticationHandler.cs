using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace OAuthBasics.Api.Authorization
{
    /*
     * Şu an anonim bir kullanıcı korunan bir kaynağa erişmek istediğinde
     * sunucu 401 UNAUTHORIZED yerine 500 INTERNAL SERVER ERROR döndürüyor.
     *
     * Bunun nedeni henüz bir Authentication Handler tanımlamadığımızdan kaynaklanıyor.
     *
     * Peki hangisini tanımlayacağız?
     *
     * Çünkü, bir düşün, AddCookie diyemeyiz çünkü cookie kullanmıyoruz.
     * AddJwt, AddOAuth da diyemeyiz.. Bunlara da ihtiyacımız yok.
     *
     *      Hem bizim ilgilendiğimiz tek şey authorization idi, Authentication değil.
     *      Onu da (custom IAuthorizationRequirement örneği ile) access_token'ı okuyarak yapıyorduk.
     *
     *          Her neyse.
     *
     * Bu örnekte bizim custom bir authentication handler yazmamız gerekiyor.
     *
     * 401 döndürmek dışında hiçbir şey yapmayan.
     *
     * Authentication servisi CHALLENGED olmazsan tetiklenmez.
     * Peki ne zaman 'CHALLENGED' olursun? İstek 401 Döndüğünde.
     *
     * Whenever we fail the authorization, we are going to be challenged.
     * Whenever we are going to be challenged, this custom authentication handler will be triggered.
     */

    public class OurCustomAuthenticationHandler : AuthenticationHandler<AuthenticationSchemeOptions>
    {
        public OurCustomAuthenticationHandler(
            IOptionsMonitor<AuthenticationSchemeOptions> options, 
            ILoggerFactory logger, 
            UrlEncoder encoder, 
            ISystemClock clock) : base(options, logger, encoder, clock) 
        {

        }

        protected override Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            // her zaman FAIL olmasını istiyoruz. 
            return Task.FromResult(AuthenticateResult.Fail("Authentication Failure"));
        }
    }
}
