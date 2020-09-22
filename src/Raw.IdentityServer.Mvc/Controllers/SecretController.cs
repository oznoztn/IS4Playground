using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Raw.IdentityServer.Mvc.Controllers
{
    [Authorize]
    public class SecretController : Controller
    {
        public async Task<IActionResult> Index()
        {
            return View();
        }
    }
}
