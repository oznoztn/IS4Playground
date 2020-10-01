using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace Raw.IdentityServer.Mvc.Controllers
{
    public class AccountController : Controller
    {
        [HttpPost]
        public IActionResult Logout()
        {
            // we pass the authc schemes from which the user signs out:
            return SignOut("Raw.IdentityServer.Mvc.Cookie", "oidc");
        }

        [Route("/good-bye")]
        public async Task<IActionResult> GoodBye()
        {
            return View();
        }
    }
}
