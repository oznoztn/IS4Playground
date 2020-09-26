using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net.Http;
using System.Security.Claims;
using System.Threading.Tasks;
using IdentityModel.Client;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Raw.IdentityServer.Constants;
using Raw.IdentityServer.Mvc.Models;

namespace Raw.IdentityServer.Mvc.Controllers
{
    [Authorize]
    public class SecretController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public SecretController(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        public async Task<IActionResult> Index()
        {
            string access_token = await HttpContext.GetTokenAsync("access_token");
            string id_token = await HttpContext.GetTokenAsync("id_token");

            JwtSecurityToken accessToken = new JwtSecurityTokenHandler().ReadJwtToken(access_token);
            JwtSecurityToken idToken = new JwtSecurityTokenHandler().ReadJwtToken(id_token);

            return View(new SecretViewModel()
            {
                AccessToken = accessToken,
                IdToken = idToken,
                UserClaims = await GetUserClaims(access_token)
            });
        }
        
        public async Task<List<Claim>> GetUserClaims(string accessToken)
        {
            var client = _httpClientFactory.CreateClient();
            var discoveryDocument = 
                await client.GetDiscoveryDocumentAsync(RawApplicationUrl.IdentityServer);

            var response =
                await client.GetUserInfoAsync(new UserInfoRequest()
                {
                    Token = accessToken,
                    Address = discoveryDocument.UserInfoEndpoint,
                    ClientId = RawClientId.Mvc,
                    ClientSecret = RawClientSecret.Mvc
                });

            return response.Claims.ToList();
        }
    }
}
