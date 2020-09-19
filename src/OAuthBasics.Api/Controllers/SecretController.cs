using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace OAuthBasics.Api.Controllers
{
    [Authorize]
    public class SecretController : ControllerBase
    {
        [HttpGet]
        public async Task<IActionResult> Get()
        {
            return Ok("Your secret code is 'DAWN'");
        }
    }
}
