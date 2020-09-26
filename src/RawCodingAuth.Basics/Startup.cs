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
             * Authorization, bir veya birden fazla Authorization Requirement'�n sa�lanmas�yla olu�an bir olgudur.
             * AuthorizationRequirement'lar AuthorizationHandler s�n�flar� taraf�ndan i�lenir.
             * 
             * T�m bunlar Authorization Policy kavram�n� olu�tururlar.
             *
             * .NET TARAFINDAK� KAR�ILIKLAR
             * Authorization Requirement => An implementation of IAuthorizationRequirement interface
             * Authorization Handler     => AuthorizationHandler of that IAuthorizationRequirement implementation
             */

            services.AddAuthorization(config =>
            {
                config.AddPolicy("admin", configurePolicy =>
                {
                    configurePolicy
                        .RequireAuthenticatedUser()
                        // Bu Authorization poli�esi belirli bir CLAIM tipini zorunlu k�l�yor
                        //      Bu arada, "Role" Microsoft'un tan�mlad��� CUSTOM bir CLAIM.
                        //          Claim'in namespace'inden de g�rebilirsin.
                        //      �rne�in name, email gibi standart claim'lerden biri de�il.
                        .RequireClaim(ClaimTypes.Role, "admin");
                });

                config.AddPolicy("secret-garden", configurePolicy =>
                {
                    configurePolicy
                        .RequireClaim("secretGarden:level")
                        .RequireClaim("secretGarden:xp")
                        .RequireClaim("secretGarden:mastery")
                        .RequireClaim("secretGarden:level")
                        // (�stteki) RequireClaim taklidi olan custom Requirement'i kullan�yorum.
                        // UserPrincipal.Claims i�erisindeki "secretGarden:path" claiminin varl���n� kontrol ediyor
                        .AddRequirements(new CustomRequireClaimRequirement("secretGarden:path"))
                        // Bir di�er custom requirement tan�mlamas�:
                        .AddRequirements(new ExperiencePointsRequirement(12));
                });

                // DEFAULT POLICY
                // A��klamada yazana g�re varsay�lan poli�e authenticated user ister, ba�ka bir �ey de�il. 
                // Varsay�lan poli�eye config.DefaultPolicy prop'u ile eri�ebilirsin.

                // Biz �imdi default policy'yi kendimiz olu�turup config.DefaultPolicy'ye set edece�iz.

                var authorizationPolicyBuilder = new AuthorizationPolicyBuilder();
                var authorizationPolicy = 
                    authorizationPolicyBuilder
                        // ��te varsay�lan policy'nin gerektirdi�i tek �ey bu
                        // Default Policy'yi default policy yapan tek Requirement.
                        // Bunu da eklemezsen "en az bir tane auth requirement'� eklemen gerek" diye hata al�rs�n.
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
