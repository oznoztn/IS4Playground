using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using OAuthBasics.Api.Authorization;
using OAuthBasics.Api.Authorization.Requirements;

/*
 *      DECLARATION OF INTENT
 * Senaryo þu:
 *
 * Authorization Server tarafýndan elde ettiðimiz access_token ile
 * Web API'dan bir 'protected' resource istemek ('authorized' bir enpoint'e istekte bulunmak yani amk)
 *
 * Fakat bunu yapmadan önce access_token'i valide etmek (tabii ki en basit haliyle)
 *
 * Bunun için authorization server tarafýnda bir endpoint oluþturmak ve bla bla....
 *
 */
namespace OAuthBasics.Api
{
    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            // using our custom authentication handler
            //      read the notes in OurCustomAuthenticationHandler.cs file
            services
                .AddAuthentication("DefaultAuthScheme")
                .AddScheme<AuthenticationSchemeOptions, OurCustomAuthenticationHandler>("DefaultAuthScheme", null);

            services
                .AddAuthorization(options =>
                {
                    AuthorizationPolicyBuilder policyBuilder = 
                        new AuthorizationPolicyBuilder();

                    policyBuilder.Requirements.Add(new RequireJwtTokenRequirement());
                    
                    options.DefaultPolicy = policyBuilder.Build();
                });


            // add authz requirements
            services.AddSingleton<IAuthorizationHandler, RequireJwtTokenRequirementHandler>();

            services.AddHttpClient();
            services.AddHttpContextAccessor();

            services.AddControllers(); // Controller only, since its an API.
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
