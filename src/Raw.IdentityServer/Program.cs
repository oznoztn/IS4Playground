using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using IdentityModel;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Raw.IdentityServer
{
    public class Program
    {
        public static void Main(string[] args)
        {
            IHost host = CreateHostBuilder(args).Build();

            using (var scope = host.Services.CreateScope())
            {
                UserManager<IdentityUser> userManager = scope.ServiceProvider.GetRequiredService<UserManager<IdentityUser>>();

                var test = new IdentityUser("test");
                userManager.CreateAsync(test, "password").GetAwaiter().GetResult();

                var ozan = new IdentityUser("ozan");
                userManager.CreateAsync(ozan, "password");
                userManager.AddClaimsAsync(ozan, new List<Claim>()
                {
                    new Claim("secret.level", "master", ClaimValueTypes.String),
                    new Claim("secret.xp", "12", ClaimValueTypes.Integer),
                    new Claim("secret.mastery", "archery", ClaimValueTypes.String),
                    new Claim("secret.path", "tao", ClaimValueTypes.String)
                });
            }

            host.Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
    }
}
