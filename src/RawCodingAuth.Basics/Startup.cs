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
            // Altta UseAuthorization dedik fakat Authz i�in hangi scheme'i kullanmas� gerekti�ini belirtmedik.
            // Yani varsay�lan bir AuthenticationScheme tan�mlamas� yapmam�z gerekiyor.
            // Aksi halde exception: No authenticationScheme was specified, and there was no DefaultChallengeScheme found
            services
                .AddAuthentication("CookieAuth")
                .AddCookie("CookieAuth", config =>
                {
                    // Authentication i�lemi i�in bakaca�� cookie'nin ismi:
                    //      Bu isimde bir cookie bulamazsa authentication i�lemi ba�ar�s�z addedilecektir.
                    config.Cookie.Name = "Grandmas.Cookie"; 

                    // Authentication i�lemi ba�ar�s�z ise kullan�c�n�n LOGIN i�in y�nlendirilece�i adres:
                    config.LoginPath = "/Home/Index";
                });


            // MapDefaultControllerRoute() ifadesinin kullanaca�� gerekli servisleri enjeksiyon mekanizmas�na eklemen gerekir.
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

            // Her bir request ile beraber �al��acak Authorization check i�lemleri i�in gerekli middleware
            // [Authorize] attribute'unu kullanmak i�in de gereklidir.
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                // Standart routing: Controller=Home, Action=Index
                endpoints.MapDefaultControllerRoute();
            });
        }
    }
}
