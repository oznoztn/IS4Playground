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
             * Authorization, Authorization Requirement'lar�n sa�lanmas�yla olu�an bir olgudur.
             * AuthorizationRequirement'lar AuthorizationHandler s�n�flar� taraf�ndan i�lenir edilir.
             *
             * T�m bunlar Authorization Policy kavram�n� olu�tururlar.
             */

            services.AddAuthorization(config =>
            {
                // A��klamada yazana g�re varsay�lan poli�e authenticated user ister, ba�ka bir �ey de�il. 
                // Varsay�lan poli�eye config.DefaultPolicy prop'u ile eri�ebilirsin.

                // Biz �imdi default policy'yi kendimiz olu�turup config.DefaultPolicy'ye set edece�iz.

                var authorizationPolicyBuilder = new AuthorizationPolicyBuilder();
                var authorizationPolicy = 
                    authorizationPolicyBuilder
                            // ��te varsay�lan policy'nin gerektirdi�i tek �ey bu
                            // Default Policy'yi default policy yapan tek gereksinim.
                            // Bunu da eklemezsen "en az bir tane auth requirement'� eklemen gerek" diye hata al�rs�n.
                        .RequireAuthenticatedUser() 
                        .Build();

                // kendi olu�turdu�umuz �eyi set ediyoruz:
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
