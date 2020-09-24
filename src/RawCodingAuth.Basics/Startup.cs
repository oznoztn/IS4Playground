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

            services.AddAuthorization(config =>
            {
                config.AddPolicy("admin", configurePolicy =>
                {
                    configurePolicy
                        .RequireAuthenticatedUser()
                        .RequireClaim(ClaimTypes.Role, "admin");
                });

                config.AddPolicy("secret-garden", configurePolicy =>
                {
                    configurePolicy
                        .RequireClaim("secretGarden:level")
                        .RequireClaim("secretGarden:xp")
                        .RequireClaim("secretGarden:mastery")
                        .RequireClaim("secretGarden:level")
                        .RequireClaim("secretGarden:path")
                        .AddRequirements(new ExperiencePointsRequirement(12));
                });

                // DEFAULT POLICY
                var authorizationPolicyBuilder = new AuthorizationPolicyBuilder();
                var authorizationPolicy = 
                    authorizationPolicyBuilder
                        .RequireAuthenticatedUser()
                        .Build();

                config.DefaultPolicy = authorizationPolicy;
            });

            // custom handler registeration:
            services.AddScoped<IAuthorizationHandler, CustomRequireClaimRequirementHandler>();
            services.AddScoped<IAuthorizationHandler, RequireExperiencePointClaimHandler>();

            services.AddControllersWithViews();
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
