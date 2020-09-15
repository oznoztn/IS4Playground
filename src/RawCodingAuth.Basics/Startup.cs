using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
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
            // Altta UseAuthorization dedik fakat Authz için hangi scheme'i kullanmasý gerektiðini belirtmedik.
            // Yani varsayýlan bir AuthenticationScheme tanýmlamasý yapmamýz gerekiyor.
            // Aksi halde exception: No authenticationScheme was specified, and there was no DefaultChallengeScheme found
            services
                .AddAuthentication("CookieAuth")
                .AddCookie("CookieAuth", config =>
                {
                    // Authentication iþlemi için bakacaðý cookie'nin ismi:
                    //      Bu isimde bir cookie bulamazsa authentication iþlemi baþarýsýz addedilecektir.
                    config.Cookie.Name = "Grandmas.Cookie"; 

                    // Authentication iþlemi baþarýsýz ise kullanýcýnýn LOGIN için yönlendirileceði adres:
                    config.LoginPath = "/Home/Index";
                });


            // MapDefaultControllerRoute() ifadesinin kullanacaðý gerekli servisleri enjeksiyon mekanizmasýna eklemen gerekir.
            services.AddControllersWithViews();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseRouting();

            // Her bir request ile beraber çalýþacak Authorization check iþlemleri için gerekli middleware
            // [Authorize] attribute'unu kullanmak için de gereklidir.
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                // Standart routing: Controller=Home, Action=Index
                endpoints.MapDefaultControllerRoute();
            });
        }
    }
}
