using System.Collections.Generic;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using MongoDB.Bson;
using MongoDB.Driver;
using Radzen;
using XeppIT.ZoneElectrical.Areas.Identity;
using XeppIT.ZoneElectrical.Data;
using XeppIT.ZoneElectrical.Identity;
using XeppIT.ZoneElectrical.Identity.StartupConfigs;
using XeppIT.ZoneElectrical.Project.Configs;
using XeppIT.ZoneElectrical.Rolodex.Config;

namespace XeppIT.ZoneElectrical
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            // Add identity types
            services.AddIdentity<ApplicationUser, ApplicationRole>(options =>
                {
                    options.Password.RequireDigit = false;
                    options.Password.RequireLowercase = false;
                    options.Password.RequireNonAlphanumeric = false;
                    options.Password.RequireUppercase = false;
                    options.Password.RequiredLength = 3;
                    options.Password.RequiredUniqueChars = 0;
                })
                .AddDefaultTokenProviders();

            services.RegisterMongoStores<ApplicationUser, ApplicationRole>("mongodb://root:admin@192.168.0.13:27017");
            services.RegisterProjectServices("mongodb://root:admin@192.168.0.13:27017");
            services.RegisterRolodexServices("mongodb://root:admin@192.168.0.13:27017");
            services.AddRazorPages();
            services.AddHostedService<MongoSetIndexesAsync>();
            services.AddHostedService<MongoSeedAdminAsync>();
            services.AddServerSideBlazor();
            services.AddScoped<AuthenticationStateProvider, RevalidatingIdentityAuthenticationStateProvider<ApplicationUser>>();
            services.AddSingleton<WeatherForecastService>();

            services.AddScoped<DialogService>();
            services.AddScoped<NotificationService>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                //app.UseDatabaseErrorPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapBlazorHub();
                endpoints.MapFallbackToPage("/_Host");
            });
        }
    }
}
