using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using IdentityModel.Client;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Raw.IdentityServer.Constants;

namespace Raw.IdentityServer.Api1.Controllers
{
    [Route("[controller]")]
    public class HomeController : ControllerBase
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public HomeController(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        public async Task<IActionResult> Get()
        {
            //retrieve access token
            var serverClient = _httpClientFactory.CreateClient();

            var discoveryDocument = await serverClient.GetDiscoveryDocumentAsync(RawApplicationUrl.IdentityServer);

            var tokenResponse = await serverClient.RequestClientCredentialsTokenAsync(
                new ClientCredentialsTokenRequest
                {
                    Address = discoveryDocument.TokenEndpoint,

                    ClientId = RawClientId.Api1,
                    ClientSecret = RawClientSecret.Api1,

                    Scope = "Raw.IdentityServer.Api2",
                });

            //retrieve secret data
            var apiClient = _httpClientFactory.CreateClient();
            apiClient.SetBearerToken(tokenResponse.AccessToken);

            var response = await apiClient.GetAsync($"{RawApplicationUrl.Api2}secret");
            var content = await response.Content.ReadAsStringAsync();

            return Ok(new
            {
                access_token = tokenResponse.AccessToken,
                status = response.StatusCode,
                secret_content = content,
            });
        }
    }
}
