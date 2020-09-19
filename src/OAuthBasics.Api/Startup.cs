using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using OAuthBasics.Api.Authorization.Requirements;

/*
 *      DECLARATION OF INTENT
 * Senaryo �u:
 *
 * Authorization Server taraf�ndan elde etti�imiz access_token ile
 * Web API'dan bir 'protected' resource istemek ('authorized' bir enpoint'e istekte bulunmak yani amk)
 *
 * Fakat bunu yapmadan �nce access_token'i valide etmek (tabii ki en basit haliyle)
 *
 * Bunun i�in authorization server taraf�nda bir endpoint olu�turmak ve bla bla....
 *
 */
namespace OAuthBasics.Api
{
    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddAuthentication();

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

            // to satify IHttpClientFactory dependency
            services.AddHttpClient();

            // to satify IHttpContextAccessor dependency
            services.AddHttpContextAccessor();

            services.AddControllers(); // since its an API
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
