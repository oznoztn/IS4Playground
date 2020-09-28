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
     * Bunun nedeni henüz bir Authentication Handler tanımlamamış olmamızdır.
     *
     * Peki hangisini tanımlayacağız?
     *
     * Çünkü, bir düşün, AddCookie diyemeyiz çünkü cookie kullanmıyoruz.
     * AddJwt, AddOAuth da diyemeyiz.. Bunlara da ihtiyacımız yok.
     *
     * Dolayısıyla bu örnekte bizim custom bir authentication handler yazmamız gerekiyor.
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
            /*
             * https://docs.microsoft.com/en-us/aspnet/core/security/authentication/?view=aspnetcore-3.1
             *
             * Based on the authentication scheme's configuration and the incoming request context,
             * authentication handlers:

             * Construct AuthenticationTicket objects representing the user's identity
             * if authentication is successful.
             *
             * Return 'no result' or 'failure' if authentication is unsuccessful.
             *
             * Have methods for challenge and forbid actions for when users attempt to access resources:
             *      They are unauthorized to access (forbid).
             *      When they are unauthenticated (challenge).
             *
             */

            return Task.FromResult(AuthenticateResult.Fail("Authentication Failure"));
        }
    }
}
