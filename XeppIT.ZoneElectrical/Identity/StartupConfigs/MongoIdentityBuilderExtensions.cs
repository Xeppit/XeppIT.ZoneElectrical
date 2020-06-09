using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Driver;
using XeppIT.ZoneElectrical.Identity.Services;

namespace XeppIT.ZoneElectrical.Identity.StartupConfigs
{
    public static class MongoIdentityBuilderExtensions
    {
        public static IdentityBuilder RegisterMongoStores<TUser, TRole>(this IServiceCollection services, string connectionString)
            where TUser : class
            where TRole : class
        {
            var client = new MongoClient(connectionString);

            var database = client.GetDatabase("Identity");

            var applicationUserCollection = database.GetCollection<ApplicationUser>("ApplicationUsers");
            var applicationRoleCollection = database.GetCollection<ApplicationRole>("ApplicationRoles");

            services.AddSingleton(provider => applicationUserCollection);
            services.AddSingleton(provider => applicationRoleCollection);

            services.AddScoped<AdminIdentityService>();

            services.AddTransient<IUserStore<ApplicationUser>, CustomUserStore>();
            services.AddTransient<IRoleStore<ApplicationRole>, CustomRoleStore>();

            return new IdentityBuilder(typeof(TUser), typeof(TRole), services);
        }
    }
}
