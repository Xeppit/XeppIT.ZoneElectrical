using Microsoft.Extensions.DependencyInjection;
using MongoDB.Driver;
using XeppIT.ZoneElectrical.Project;
using XeppIT.ZoneElectrical.Rolodex.Models;

namespace XeppIT.ZoneElectrical.Rolodex.Config
{
    public static class ProjectServiceBuilder
    {
        public static void RegisterRolodexServices(
            this IServiceCollection services, 
            string connectionString,
            string databaseName = "Rolodex"
        )
        {
            var client = new MongoClient(connectionString);

            var database = client.GetDatabase(databaseName);

            var companyCollection = database.GetCollection<Company>("Companies");
            var addressCollection = database.GetCollection<Address>("Addresses");
            
            services.AddScoped(provider => companyCollection);
            services.AddScoped(provider => addressCollection);
            services.AddScoped<RolodexService>();

            // Set indexes on db
            services.AddHostedService<SetIndexOnCompanyNameAsync>();
            services.AddHostedService<SeedAddresses>();
        }
    }
}
