using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Primitives;

namespace OAuthBasics.Server.Controllers
{
    public class RedirectionInfo
    {
        public string RedirectUri { get; set; }
        public string State { get; set; }
    }

    public class OAuthController : Controller
    {
        // https://tools.ietf.org/html/rfc6749#section-4.2.1 4.2.1.  Authorization Request
        // User authc için buraya yönlendirildiğiinde OAuthBasics.Client tarafı gönderiyor bunları
        [HttpGet]
        public async Task<IActionResult> Authorize(
            string response_type, // authorization flow type
            string client_id, 
            string redirect_uri, // url to redirect after successful authentication
            string scope, // what info I want
            string state // client generates it. https://pipedrive.readme.io/docs/marketplace-oauth-authorization-state-parameter
            )
        {
            return View(new RedirectionInfo
            {
                RedirectUri = redirect_uri,
                State = state
            });
        }

        
        [HttpPost]
        public async Task<IActionResult> Authorize(
            string username, 
            string password, 
            string state, 
            string redirect_uri)
        {
            // we assume the user info is valid.
            
            const string code = "auth code"; // we assume we generated the code

            // // https://tools.ietf.org/html/rfc6749#section-4.1.2 Authorization Response

            QueryBuilder query = new QueryBuilder();
            query.Add(nameof(code), code);
            query.Add(nameof(state), state);

            string redirectUri = $"{redirect_uri}{query}";

            return Redirect(redirectUri);


            return null;
        }

        public async Task<IActionResult> Token()
        {
            return null;
        }
    }
}
