using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OAuthBasics.Server
{
    public static class Constants
    {
        public const string Issuer = "https://localhost:44324/";
        public const string Audience = "https://localhost:44324/";
        public const string SecretKey = "you shouldnt set a too short key. otherwise it will give you an error";
    }
}
