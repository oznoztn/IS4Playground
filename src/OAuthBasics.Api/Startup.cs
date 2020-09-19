using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

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
            services.AddAuthorization();

            services.AddControllers(); // since its an API
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseRouting();
            
            // We dont need authentication functionality in this API
            
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapDefaultControllerRoute();
            });
        }
    }
}
