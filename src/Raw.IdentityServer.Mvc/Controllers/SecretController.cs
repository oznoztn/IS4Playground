using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Raw.IdentityServer.Mvc.Models;

namespace Raw.IdentityServer.Mvc.Controllers
{
    [Authorize]
    public class SecretController : Controller
    {
        public async Task<IActionResult> Index()
        {
            string access_token = await HttpContext.GetTokenAsync("access_token");
            string id_token = await HttpContext.GetTokenAsync("id_token");

            JwtSecurityToken accessToken = new JwtSecurityTokenHandler().ReadJwtToken(access_token);
            JwtSecurityToken idToken = new JwtSecurityTokenHandler().ReadJwtToken(id_token);

            var x = this.User.Claims.ToList();
            return View(new SecretViewModel()
            {
                AccessToken = accessToken,
                IdToken = idToken,
            });
        }
    }
}
