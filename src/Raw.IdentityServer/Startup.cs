using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Logging;
using Raw.IdentityServer.Data;

namespace Raw.IdentityServer
{
    public class Startup
    { 
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDbContext<ApplicationDbContext>(options =>
            {
                options.UseInMemoryDatabase("memdb");
            });

            IdentityBuilder identityBuilder = services.AddIdentity<IdentityUser, IdentityRole>(options =>
            {
                options.Password.RequiredLength = 3;
                options.Password.RequireDigit = false;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequireLowercase = false;
                options.Password.RequireUppercase = false;
            });

            identityBuilder
                .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddDefaultTokenProviders();

            services.ConfigureApplicationCookie(options =>
            {
                options.Cookie.Name = "Raw.IdentityServer.Cookie";
                options.LoginPath = "/auth/login";
            });

            var migrationsAssembly = typeof(Startup).GetTypeInfo().Assembly.GetName().Name;
            const string connectionString = @"Data Source=(LocalDb)\MSSQLLocalDB;database=Raw.IdentityServer;trusted_connection=yes;";

            // He says AddIdentityServer instruction adds authentication and authorizations services
            IIdentityServerBuilder identityServerBuilder = services.AddIdentityServer();

            identityServerBuilder
                .AddAspNetIdentity<IdentityUser>() // Solves "InvalidOperationException: sub claim is missing"
                .AddConfigurationStore(options =>
                {
                    options.ConfigureDbContext = b => 
                        b.UseSqlServer(connectionString, sql => sql.MigrationsAssembly(migrationsAssembly));
                })
                .AddOperationalStore(options =>
                {
                    options.ConfigureDbContext = b => 
                        b.UseSqlServer(connectionString, sql => sql.MigrationsAssembly(migrationsAssembly));
                });

            // not recommended for production - you need to store your key material somewhere secure
            identityServerBuilder.AddDeveloperSigningCredential();

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

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseIdentityServer();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapDefaultControllerRoute();
            });
        }
    }
}
