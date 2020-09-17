using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RawCodingAuth.Basics.Auth.CustomAuthorizationRequirements;

namespace RawCodingAuth.Basics.Controllers
{
    public class RequirementsController : Controller
    {
        private readonly IAuthorizationService _authorizationService;
        public RequirementsController(IAuthorizationService authorizationService)
        {
            _authorizationService = authorizationService;
        }

        [Authorize]
        public async Task<IActionResult> Index()
        {
            // IAuthorizationRequirement implementasyonlarını IAuthorizationService implementasyonuna verip
            // mevcut kullanıcının belirtilen kriterleri sağlayıp sağlamadı da kontrol edilebilir.
            // Normalde bu IAuthorizationRequirement'lar Policy bazında belirlenirdi.
            var authzRequirements = new[]
            {
                new ExperiencePointsRequirement(12),
            };

            var result = await _authorizationService.AuthorizeAsync(User, resource: null, authzRequirements);

            if (result.Succeeded)
            {
                return Content("SUCCESS!");
            }
            else
            {
                return Content("FAILURE! Your level is below 12");
            }
        }
    }
}
