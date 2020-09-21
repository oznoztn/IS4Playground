using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace Raw.IdentityServer.Api1.Controllers
{
    [Route("[controller]")]
    public class HomeController : ControllerBase
    {
        public async Task<IActionResult> Get()
        {
            var okResult = Ok("Raw.IdentityServer.Api1 says Hello!");
            
            return await Task.FromResult(okResult);
        }
    }
}
