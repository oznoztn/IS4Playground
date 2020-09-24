using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Raw.IdentityServer.Models
{
    public class LoginViewModel
    {
        public string Username { get; set; }
        public string Password { get; set; }

        /// <summary>
        /// URL to redirect after successful login
        /// </summary>
        public string ReturnUrl { get; set; }
    }
}
