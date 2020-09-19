using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace OAuthBasics.Api.Controllers
{
    [Authorize]
    public class SecretController : ControllerBase
    {
        [HttpGet]
        public async Task<IActionResult> Get()
        {
            return Ok(new
            {
                code = "dawn",
                level = "12",
                confidential = "true",
                rank = "supreme"
            });
        }
    }
}
