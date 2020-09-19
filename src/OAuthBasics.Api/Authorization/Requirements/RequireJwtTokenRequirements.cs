using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;

namespace OAuthBasics.Api.Authorization.Requirements
{
    public class RequireJwtTokenRequirement : IAuthorizationRequirement
    {
        
    }

    public class RequireJwtTokenRequirementHandler : AuthorizationHandler<RequireJwtTokenRequirement>
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly HttpClient _httpClient;

        // Buradan API call yapmam lazım. Dolayısıyla HttpClient'a (veya başka bir istemciye) ihtiyacım var.
        //      Inject IHttpClientFactory
        // API Call'dan önce access_token'a erişmem gerekiyor. access_token ise request header'ında...
        //      Inject IHttpContextAccessor
        public RequireJwtTokenRequirementHandler(
            IHttpClientFactory httpClientFactory,
            IHttpContextAccessor httpContextAccessor)
        {
            _httpClientFactory = httpClientFactory;
            _httpContextAccessor = httpContextAccessor;
            _httpClient = httpClientFactory.CreateClient();
        }

        protected override async Task HandleRequirementAsync(
            AuthorizationHandlerContext context, 
            RequireJwtTokenRequirement requirement)
        {
            context.Succeed(requirement);
            
            if (_httpContextAccessor.HttpContext.Request.Headers.TryGetValue("Authorization", out StringValues authzHeaderValue) == false)
            {
                context.Fail();
                return;
            }

            if (context.HasSucceeded && string.IsNullOrWhiteSpace(authzHeaderValue))
            {
                context.Fail();
                return;
            }

            var accessToken = authzHeaderValue.ToString().Split(' ')[1] ?? ""; // "Bearer access_tokens_content"

            if (context.HasSucceeded && string.IsNullOrWhiteSpace(accessToken))
            {
                context.Fail();
                return;
            }

            _httpClient.DefaultRequestHeaders.Authorization = 
                new AuthenticationHeaderValue("Bearer", accessToken);

            // validate token
            
            var result = await _httpClient
                .GetAsync($"https://localhost:44324/oauth/validate?access_token={accessToken}");

            if (result.StatusCode == HttpStatusCode.OK)
            {
                context.Succeed(requirement);
            }
        }
    }
}
