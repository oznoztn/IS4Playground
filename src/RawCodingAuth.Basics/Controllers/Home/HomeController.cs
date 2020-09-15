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

            List<Claim> secretGardenClaims = new List<Claim>()
            {
                // Bu alttaki bir claim'in ortak olmasına dikkat et.
                // Demek ki her iki yerle aynı e-mail adresi ile bir bağlantım var.
                new Claim(ClaimTypes.Name, "Ozan ZzZz"),
                new Claim(ClaimTypes.Email, "ozan@ozten.com"),

                // secret-garden'a özel claim'ler.
                new Claim("level", "master"),
                new Claim("xp", "12"),
                new Claim("mastery", "archery"),
                new Claim("path", "tao")
            };

            // Bu claim'leri baz alarak bir kimlik oluşturuyoruz
            ClaimsIdentity grandmaIdentity = new ClaimsIdentity(grandmaClaims, authenticationType: "Grandma Identity");

            ClaimsIdentity secretGardenIdentity = new ClaimsIdentity(secretGardenClaims, "Secret Garden Identity");

            ClaimsPrincipal userPrincipal = new ClaimsPrincipal(new[]
            {
                // Birden fazla otorite senin kim olduğuna dair bilgi verebileceğine göre,
                // ClaimsIdentity'ye sahip olabilirsin.
                grandmaIdentity,
                secretGardenIdentity
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
             * Yukarda iki tane otorite var: Grandma ve SecretGarden
             * */
        }

        [Authorize]
        public IActionResult Secret()
        {
            return View();
        }
    }
}
