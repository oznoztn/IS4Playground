using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace RawCodingAuth.Basics.Controllers.Home
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Authenticate()
        {
            List<Claim> grandmaClaims = new List<Claim>()
            {
                // Senin kim olduğunu GRANDMA söylüyor. Burada OTORİTE o.
                // Büyükannene göre senin bilgilerin şunlar:
                
                // Standart claim'ler
                new Claim(ClaimTypes.Name, "Ozan"),
                new Claim(ClaimTypes.Email, "ozan@ozten.com"),
                
                // Büyükannene özel claim'ler.
                new Claim("grandma.garden", "true")
            };

            // Bu claim'leri baz alarak bir kimlik oluşturuyoruz
            ClaimsIdentity grandmaIdentity = new ClaimsIdentity(grandmaClaims, authenticationType: "Grandma Identity");

            ClaimsPrincipal userPrincipal = new ClaimsPrincipal(new[]
            {
                // Birden fazla otorite senin kim olduğuna dair bilgi verebileceğine göre,
                // ClaimsIdentity'ye sahip olabilirsin.
                grandmaIdentity
            });

            // Kullanıcı sisteme sokuyoruz (LOGIN).
            // Arkaplanda client tarayıcısına Grandmas.Cookie isimli çerezi yazdırıyor.
            HttpContext.SignInAsync(userPrincipal);

            return RedirectToAction("Secret");

            /*
             * 1)
             * Claim soyut bir kavramdır. .NET ile alakası değildir. Bir standarttır.
             *
             * 2)
             * Claim kullanıcı hakkındaki bilgileri tutan key/val ikililerinde oluşan bir veri yapısıdır.
             *
             * 3)
             * Birden fazla otorite senin kim olduğuna dair bilgi sağlayabilir.
             * Örneğin; Grandman, Google, Facebook, E-Devlet, vs.
             */
        }

        [Authorize]
        public IActionResult Secret()
        {
            return View();
        }
    }
}
