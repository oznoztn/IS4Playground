using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Connections.Features;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace RawCodingAuth.Basics
{
    public class Startup
    {
        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services
                .AddAuthentication("CookieAuth")
                .AddCookie("CookieAuth", config =>
                {
                    config.Cookie.Name = "Grandmas.Cookie";
                    config.LoginPath = "/Home/Index";
                });

            /* AUTHORIZATION POLICY
             *
             * Authorization, Authorization Requirement'larýn saðlanmasýyla oluþan bir olgudur.
             * AuthorizationRequirement'lar AuthorizationHandler sýnýflarý tarafýndan iþlenir edilir.
             *
             * Tüm bunlar Authorization Policy kavramýný oluþtururlar.
             */

            services.AddAuthorization(config =>
            {
                // Açýklamada yazana göre varsayýlan poliçe authenticated user ister, baþka bir þey deðil. 
                // Varsayýlan poliçeye config.DefaultPolicy prop'u ile eriþebilirsin.

                // Biz þimdi default policy'yi kendimiz oluþturup config.DefaultPolicy'ye set edeceðiz.

                var authorizationPolicyBuilder = new AuthorizationPolicyBuilder();
                var authorizationPolicy = 
                    authorizationPolicyBuilder
                            // Ýþte varsayýlan policy'nin gerektirdiði tek þey bu
                            // Default Policy'yi default policy yapan tek gereksinim.
                            // Bunu da eklemezsen "en az bir tane auth requirement'ý eklemen gerek" diye hata alýrsýn.
                        .RequireAuthenticatedUser() 
                        .Build();

                // kendi oluþturduðumuz þeyi set ediyoruz:
                config.DefaultPolicy = authorizationPolicy;
            });

            services.AddControllersWithViews();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            
            app.UseAuthentication();

            app.UseRouting();
            
            app.UseAuthorization();
            
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapDefaultControllerRoute();
            });
        }
    }
}
