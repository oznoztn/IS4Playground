using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Raw.IdentityServer.Api2.Controllers
{
    public class SecretController : ControllerBase
    {
        [Authorize]
        [HttpGet("/secret")]
        public async Task<string> Get()
        {
            return "secret Message from Api2";
        }
    }
}