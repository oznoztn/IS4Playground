using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;
using JwtRegisteredClaimNames = Microsoft.IdentityModel.JsonWebTokens.JwtRegisteredClaimNames;

namespace OAuthBasics.Server.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        public async Task<IActionResult> Authenticate()
        {
            var claims = new List<Claim>()
            {
                new Claim(JwtRegisteredClaimNames.Sub, "some-id-1"),
                new Claim(JwtRegisteredClaimNames.Email, "ozan@ozten.com"),
            };

            #region SigningCredentials

            /*
             * Standartlara göre JWT jetonunu şifrelemenin iki yolu vardı:
             * Bunlardan birisi sertifika kullanmak diğeri ise secret key. Dokümantasyonu hatırla.
             *
             * Bu gerçek kendisini SigningCredentials sınıfının ctor'unda tezahür ettiriyor.
             */

            SecurityKey securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Constants.SecretKey));
            string algorithm = SecurityAlgorithms.Sha256;

            SigningCredentials signingCredentials = new SigningCredentials(securityKey, algorithm );

            #endregion
            
            JwtSecurityToken jwtSecurityToken = 
                new JwtSecurityToken(
                    Constants.Issuer,
                    Constants.Audience, 
                    claims, 
                    notBefore: DateTime.Now, 
                    expires: DateTime.Now.AddHours(1),
                    signingCredentials: signingCredentials);

            var claimsIdentity = new ClaimsIdentity(claims);

            var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);


            // jwtSecurityToken değişkenini (JwtSecurityToken) öylece serialize edemeyiz,
            // çünkü içerisinde internal malzemeler var.
            var tokenJson = new JwtSecurityTokenHandler().WriteToken(jwtSecurityToken);

            return Ok(new
            {
                access_token = tokenJson
            });
        }

        [Authorize]
        public async Task<IActionResult> Secret()
        {
            return Content("OK!");
        }
    }
}
