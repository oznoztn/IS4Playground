using System.Collections.Generic;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace RawCodingAuth.Basics.Controllers
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
            List<Claim> roleClaims = new List<Claim>()
            {
                new Claim(ClaimTypes.Role, "root"),
                new Claim(ClaimTypes.Role, "admin")
            };

            List<Claim> grandmaClaims = new List<Claim>()
            {
                // Standard claims:
                new Claim(ClaimTypes.Name, "Ozan"),
                new Claim(ClaimTypes.Email, "ozan@ozten.com"),
                
                // Non-standard claims:
                new Claim("grandma.garden", "true")
            };

            List<Claim> secretGardenClaims = new List<Claim>()
            {
                // Alttaki e-mail claim'inin ortak olmasına dikkat et.
                //   Demek ki her iki yerle aynı e-mail adresiyle bir bağlantım var.
                new Claim(ClaimTypes.Name, "Ozan ZzZz"),
                new Claim(ClaimTypes.Email, "ozan@ozten.com"),

                // secret-garden'a özel claim'ler.
                new Claim("secretGarden:level", "master"),
                new Claim("secretGarden:xp", "12"),
                new Claim("secretGarden:mastery", "archery"),
                new Claim("secretGarden:path", "tao")
            };

            // Bu claim'leri baz alarak bir kimlik oluşturuyoruz
            ClaimsIdentity grandmaIdentity = 
                new ClaimsIdentity(grandmaClaims, authenticationType: "Grandma Identity");

            ClaimsIdentity secretGardenIdentity = 
                new ClaimsIdentity(secretGardenClaims, "Secret Garden Identity");

            ClaimsIdentity rolesIdentity = 
                new ClaimsIdentity(roleClaims, "RawCodingAuth Identity");

            ClaimsPrincipal userPrincipal = new ClaimsPrincipal(new[]
            {
                // Birden fazla otorite senin kim olduğuna dair bilgi verebileceğine göre,
                // birden fazla ClaimsIdentity'ye sahip olabilirsin.
                grandmaIdentity,
                secretGardenIdentity,
                rolesIdentity
            });

            // Kullanıcıyı sisteme sokuyoruz.
            // Kullanıcının tarayıcısına RawCodingAuth.Basics.Cookie isimli çerezi yazdırıyoruz.
            HttpContext.SignInAsync(userPrincipal);

            return RedirectToAction("Secret");

            /*
             * 1)
             * Claim soyut bir kavramdır. .NET ile alakalı değildir. Bir standarttır.
             *
             * 2)
             * Claim kullanıcı hakkındaki bilgileri tutan key/val ikilisinden oluşan bir veri yapısıdır.
             *
             * 3)
             * Birden fazla otorite senin kim olduğuna dair bilgi sağlayabilir.
             * Örneğin; Grandma, Google, Facebook, E-Devlet, vs.
             *
             * 4)
             * Identity framework kullanıcıya bağlı CLAIM'leri veritabanında tutar
             * ve kullanıcı (Identity ile gelen) ilgili manager sınıfıyla çekildiğinde
             * otomatik olarak bunları kullanıcıya SET eder.
             *
             * Dolayısıyla burada yaptığımız şeyler low-level işlemler aslında.
             * Yine de neyin nasıl işlediğini bilmekte fayda olabilir.
             *
             * */
        }

        [Authorize]
        public IActionResult Secret()
        {
            return View();
        }
    }
}
