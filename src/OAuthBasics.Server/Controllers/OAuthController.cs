using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Primitives;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;

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
        }

        // https://tools.ietf.org/html/rfc6749#section-4.1.3 access token request (from the client)
        public async Task<IActionResult> Token(
            string grant_type, // flow of access_token request
            string code, // confirmation purposes
            string redirect_uri,
            string client_id
            )
        {
            // OAuth Kodu'nu burada doğrulamamız gerekir. Biz atladık o işi.
            //      Issue edilen Auth Kodu'nun 5 dakikalık bir ömrü var(mış)

            var claims = new List<Claim>()
            {
                new Claim(JwtRegisteredClaimNames.Sub, "some-id-1"),
                new Claim(JwtRegisteredClaimNames.Email, "ozan@ozten.com"),
            };

            SecurityKey securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Constants.SecretKey));
            string algorithm = SecurityAlgorithms.HmacSha256;
            SigningCredentials signingCredentials = new SigningCredentials(securityKey, algorithm);
            
            JwtSecurityToken jwtSecurityToken =
                new JwtSecurityToken(
                    Constants.Issuer,
                    Constants.Audience,
                    claims,
                    notBefore: DateTime.Now,
                    expires: DateTime.Now.AddHours(1),
                    signingCredentials: signingCredentials);

            // jwtSecurityToken değişkenini (JwtSecurityToken) öylece serialize edemeyiz,
            // çünkü içerisinde internal malzemeler var.
            var tokenJson = new JwtSecurityTokenHandler().WriteToken(jwtSecurityToken);
            
            // https://tools.ietf.org/html/rfc6749#section-4.1.4 access token response
            var tokenResponseObject = new
            {
                access_token = tokenJson,
                token_type = "Bearer",
                // expires_in = "", // omitted
                // refresh_token = "", // omitted
                my_name = "ozan"
            };

            string tokenResponseJson = JsonConvert.SerializeObject(tokenResponseObject);
            var responseBytes = Encoding.UTF8.GetBytes(tokenResponseJson);

            await Response.Body.WriteAsync(responseBytes, 0, responseBytes.Length);

            return Redirect(redirect_uri);
        }
    }
}
/*
 *  
 * If you are using KESTREL
 * 
 * Q:
 * Thanks for your video, when I start using iis, it works fine, but when using kestler, it always thrown an error said: error while copying content to stream. Would you please help?
 * 
 * A from RawCoding:
 * var responseJson = JsonConvert.SerializeObject(responseObject);
 * var responseBytes = Encoding.UTF8.GetBytes(responseJson);
 * await Response.Body.WriteAsync(responseBytes, 0, responseBytes.Length);
 * return  Redirect(redirect_uri);
 * 
 * Remove this, and just return responseObjectn
 * 
 * You'll have to change the method return type from async Task<Iaction.... to object
 * 
 */
