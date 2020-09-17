using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;

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
            List<Claim> roleClaims = new List<Claim>()
            {
                new Claim(ClaimTypes.Role, "root"),
                new Claim(ClaimTypes.Role, "admin")
            };

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
                new Claim("secretGarden:level", "master"),
                new Claim("secretGarden:xp", "12"),
                new Claim("secretGarden:mastery", "archery"),
                new Claim("secretGarden:path", "tao")
            };

            // Bu claim'leri baz alarak bir kimlik oluşturuyoruz
            ClaimsIdentity grandmaIdentity = new ClaimsIdentity(grandmaClaims, authenticationType: "Grandma Identity");

            ClaimsIdentity secretGardenIdentity = new ClaimsIdentity(secretGardenClaims, "Secret Garden Identity");

            ClaimsIdentity rolesIdentity = new ClaimsIdentity(roleClaims, "RawCodingAuth Identity");

            ClaimsPrincipal userPrincipal = new ClaimsPrincipal(new[]
            {
                // Birden fazla otorite senin kim olduğuna dair bilgi verebileceğine göre,
                // ClaimsIdentity'ye sahip olabilirsin.
                grandmaIdentity,
                secretGardenIdentity,
                rolesIdentity
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
             *
             * 4)
             * Identity framework kullanıcıya bağlı CLAIM'leri veritabanında tutar ve
             * kullanıcı (identity'deki) ilgili manager sınıfıyla çekildiğinde
             * zaten otomatik olarak kullanıcıya SET eder.
             *
             * Buradaki implementasyon bir nevi low-level implementasyon.
             * Yine de neyin nasıl set edildiğini bilmekte fayda var.
             *
             * */
        }

        [Authorize]
        public IActionResult Secret()
        {
            return View();
        }

        // User.Claims.Any(t => t.Type == ClaimTypes.Role) == true
        //   olan ClaimsPrinciap için...
        //   Kısacası kullanıcının 'role' claimine bakıyor, belirtilen rol var mı yok mu diye.
        [Authorize(Roles = "god-mode")]
        public IActionResult GodMode()
        {
            return Content("God mode");
        }

        [Authorize(Policy = "secret-garden")]
        public IActionResult SecretGardenPolicy()
        {
            return Content("Secret Garden Policy");
        }

        [Authorize(Policy = "admin")]
        public IActionResult CustomPolicy()
        {
            return Content("Custom Policy");
        }

        // Episode 4 - IAuthorizationService kullanımı

        public async Task<IActionResult> AuthorizationService(
            [FromServices]IAuthorizationService authorizationService)
        {
            // Poliçe oluşturuyorum. Normalde bu iş Startup içerisinde yapılır.
            var policyBuilder = new AuthorizationPolicyBuilder();
            var policy = 
                policyBuilder
                    .RequireClaim(ClaimTypes.Role, "admin")
                    .Build();

            ClaimsPrincipal loggedInUser = HttpContext.User;

            var authzResult = await authorizationService.AuthorizeAsync(loggedInUser, null, policy);
            if (authzResult.Succeeded)
            {
                // kullanıcı admin rolünde, bir şeyler yap.
                return Content("You are the administrator!");
            }
            else
            {
                // kullanıcı admin rolünde değil, bir şeyler yap.
                return Content("You are not the administrator!");
            }
        }

        [Authorize]
        public async Task<IActionResult> AuthorizationService2(
            [FromServices]IAuthorizationService authorizationService)
        {
            // the policy name to check against
            string policyName = "admin";
; 
            var result =
                await authorizationService
                    .AuthorizeAsync(HttpContext.User, null, policyName);

            if (result.Succeeded)
            {
                // kullanıcı admin rolünde, bir şeyler yap.

                return Content("You are the administrator!");
            }
            else
            {
                // kullanıcı admin rolünde değil, bir şeyler yap.

                return Content("You are not the administrator!");
            }
        }
    }
}
