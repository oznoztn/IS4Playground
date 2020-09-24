using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using Raw.IdentityServer.Constants;

namespace Raw.IdentityServer.Api2
{
    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services
                .AddAuthentication("Bearer")
                .AddJwtBearer("Bearer", config =>
                {
                    config.Authority = RawApplicationUrl.IdentityServer;
                    config.Audience = RawClientName.Api2;

                    // Art�k access_token'daki 'aud' claim de�erini de valide ediyoruz.
                    // B�ylece gelen token'in bu API i�in olu�turulmu� oldu�unu do�rulam�� oluyoruz.

                    // Gereksiz asl�nda --�stte set etti�imizden. Yine de silmedim.
                    config.TokenValidationParameters = new TokenValidationParameters()
                    {
                        ValidateAudience = true
                    };
                });

            services.AddHttpClient();

            services.AddControllers();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}