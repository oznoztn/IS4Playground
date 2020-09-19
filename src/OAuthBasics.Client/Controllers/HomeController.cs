using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace OAuthBasics.Client.Controllers
{
    public class HomeController : Controller
    {
        private readonly HttpClient _httpClient;

        public HomeController(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient();
        }

        public async Task<IActionResult> Index()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Authenticate()
        {
            return View();
        }

        public async Task<IActionResult> CallSecretApi()
        {
            var accessToken = await HttpContext.GetTokenAsync("access_token");

            if (accessToken != null)
            {
                string apiResourceUri = "https://localhost:44309/secret/get";

                // her seferinde set ediyorum
                // yoksa birden fazla Authorization header eklenmeye çalışılacaktır eninde sonunda
                // bu da hata verecektir.
                _httpClient.DefaultRequestHeaders.Authorization = 
                    new AuthenticationHeaderValue("Bearer", accessToken);
                
                HttpResponseMessage response = await _httpClient.GetAsync(apiResourceUri);
                string responseContent = await response.Content.ReadAsStringAsync();

                return Content(responseContent);
            }

            return BadRequest("bad request - maybe you are not authorized");
        }

        [Authorize]
        public async Task<IActionResult> Secret()
        {
            return View();
        }
    }
}
