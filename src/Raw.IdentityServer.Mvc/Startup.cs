using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.ApplicationParts;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Raw.IdentityServer.Mvc
{
    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services
                .AddAuthentication(options =>
                {
                    options.DefaultScheme = "Raw.IdentityServer.Mvc.Cookie";
                    options.DefaultSignInScheme = "Raw.IdentityServer.Mvc.Cookie";
                    options.DefaultChallengeScheme = "oidc";
                })
                .AddCookie("Raw.IdentityServer.Mvc.Cookie", options =>
                {
                    options.LoginPath = "";
                })
                .AddOpenIdConnect("oidc", config =>
                {
                    config.Authority = "https://localhost:44348";

                    config.ClientId = "Raw.IdentityServer.Mvc.ClientId";
                    config.ClientSecret = "Raw.IdentityServer.Mvc.ClientSecret";

                    config.ResponseType = "code";

                    config.SaveTokens = true;
                    
                    // NOT 1.
                });

            services.AddControllersWithViews();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();
            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapDefaultControllerRoute();
            });
        }
    }
}

/*
 *      NOT 1) 
 * Önceki basit bir OAuth implementasyon örneði olan,
 * OAuthBasics.Client'deki AddOAuth'da belirttiðimiz endpoint'leri burada belirtmemize gerek yok.
 * AddOpenIdConnect servisi o bilgilere Discovery Dokümaný ile vakýf zaten.
 *
 */
