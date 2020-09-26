using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Connections.Features;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using RawCodingAuth.Basics.Auth.CustomAuthorizationRequirements;

namespace RawCodingAuth.Basics
{
    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services
                .AddAuthentication("CookieAuth")
                .AddCookie("CookieAuth", config =>
                {
                    config.Cookie.Name = "RawCodingAuth.Basics.Cookie";
                    config.LoginPath = "/Home/Index";
                });

            /* AUTHORIZATION POLICY
             *
             * Authorization, bir veya birden fazla Authorization Requirement'ýn saðlanmasýyla oluþan bir olgudur.
             * AuthorizationRequirement'lar AuthorizationHandler sýnýflarý tarafýndan iþlenir.
             * 
             * Tüm bunlar Authorization Policy kavramýný oluþtururlar.
             *
             * .NET TARAFINDAKÝ KARÞILIKLAR
             * Authorization Requirement => An implementation of IAuthorizationRequirement interface
             * Authorization Handler     => AuthorizationHandler of that IAuthorizationRequirement implementation
             */

            services.AddAuthorization(config =>
            {
                config.AddPolicy("admin", configurePolicy =>
                {
                    configurePolicy
                        .RequireAuthenticatedUser()
                        // Bu Authorization poliçesi belirli bir CLAIM tipini zorunlu kýlýyor
                        //      Bu arada, "Role" Microsoft'un tanýmladýðý CUSTOM bir CLAIM.
                        //          Claim'in namespace'inden de görebilirsin.
                        //      Örneðin name, email gibi standart claim'lerden biri deðil.
                        .RequireClaim(ClaimTypes.Role, "admin");
                });

                config.AddPolicy("secret-garden", configurePolicy =>
                {
                    configurePolicy
                        .RequireClaim("secretGarden:level")
                        .RequireClaim("secretGarden:xp")
                        .RequireClaim("secretGarden:mastery")
                        .RequireClaim("secretGarden:level")
                        // (Üstteki) RequireClaim taklidi olan custom Requirement'i kullanýyorum.
                        // UserPrincipal.Claims içerisindeki "secretGarden:path" claiminin varlýðýný kontrol ediyor
                        .AddRequirements(new CustomRequireClaimRequirement("secretGarden:path"))
                        // Bir diðer custom requirement tanýmlamasý:
                        .AddRequirements(new ExperiencePointsRequirement(12));
                });

                // DEFAULT POLICY
                // Açýklamada yazana göre varsayýlan poliçe authenticated user ister, baþka bir þey deðil. 
                // Varsayýlan poliçeye config.DefaultPolicy prop'u ile eriþebilirsin.

                // Biz þimdi default policy'yi kendimiz oluþturup config.DefaultPolicy'ye set edeceðiz.

                var authorizationPolicyBuilder = new AuthorizationPolicyBuilder();
                var authorizationPolicy = 
                    authorizationPolicyBuilder
                        // Ýþte varsayýlan policy'nin gerektirdiði tek þey bu
                        // Default Policy'yi default policy yapan tek Requirement.
                        // Bunu da eklemezsen "en az bir tane auth requirement'ý eklemen gerek" diye hata alýrsýn.
                        .RequireAuthenticatedUser()
                        .Build();

                config.DefaultPolicy = authorizationPolicy;
            });

            // custom handler registeration:
            services.AddScoped<IAuthorizationHandler, CustomRequireClaimRequirementHandler>();
            services.AddScoped<IAuthorizationHandler, RequireExperiencePointClaimHandler>();

            services
                .AddControllersWithViews()
                .AddRazorRuntimeCompilation();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

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
