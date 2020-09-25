using System.IdentityModel.Tokens.Jwt;

namespace Raw.IdentityServer.Mvc.Models
{
    public class SecretViewModel
    {
        public JwtSecurityToken AccessToken { get; set; }
        public JwtSecurityToken IdToken { get; set; }
    }
}