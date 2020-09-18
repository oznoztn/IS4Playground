using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace OAuthBasics.Client.Controllers
{
    public class HomeController : Controller
    {
        public async Task<IActionResult> Index()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Authenticate()
        {
            return View();
        }

        [Authorize]
        public async Task<IActionResult> Secret()
        {
            // SET oauthOptions.SaveTokens to TRUE. Otherwise it will be null.
            string accessToken = await HttpContext.GetTokenAsync("access_token");

            return View();
        }
    }
}
