using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace Raw.IdentityServer.Mvc.Models
{
    public class SecretViewModel
    {
        public JwtSecurityToken AccessToken { get; set; }
        public JwtSecurityToken IdToken { get; set; }
        
        /// <summary>
        /// via /userinfo endpoint
        /// </summary>
        public List<Claim> UserClaims { get; set; }
    }
}