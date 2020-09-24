using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace RawCodingAuth.Basics.Controllers
{
    public class PolicyController : Controller
    {
        // User.Claims.Any(t => t.Type == ClaimTypes.Role) == true
        //      olan ClaimsPrincipal buraya erişebilir.
        //  Kullanıcının 'role' claimine bakıyor, belirtilen rol var mı yok mu diye.
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
            [FromServices] IAuthorizationService authorizationService)
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
            [FromServices] IAuthorizationService authorizationService)
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
