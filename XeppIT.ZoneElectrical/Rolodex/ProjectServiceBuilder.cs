using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Driver;
using XeppIT.ZoneElectrical.Rolodex.Addresses.Config;
using XeppIT.ZoneElectrical.Rolodex.Addresses.Model;
using XeppIT.ZoneElectrical.Rolodex.Companies.Config;
using XeppIT.ZoneElectrical.Rolodex.Companies.Model;
using XeppIT.ZoneElectrical.Rolodex.Contacts.Config;
using XeppIT.ZoneElectrical.Rolodex.Contacts.Model;

namespace XeppIT.ZoneElectrical.Rolodex
{
    public static class ProjectServiceBuilder
    {
        public static async Task RegisterRolodexServices(
            this IServiceCollection services, 
            string connectionString,
            string databaseName = "Rolodex"
        )
        {
            var client = new MongoClient(connectionString);

            var database = client.GetDatabase(databaseName);

            var addressCollection = database.GetCollection<Address>("Addresses");
            var companyCollection = database.GetCollection<Company>("Companies");
            var contactCollection = database.GetCollection<Contact>("Contacts");

            services.AddScoped(provider => companyCollection);
            services.AddScoped(provider => addressCollection);
            services.AddScoped(provider => contactCollection);
            services.AddScoped<RolodexService>();

            // Set indexes on db
            services.AddHostedService<SetIndexOnCompanyNameAsync>();
            services.AddHostedService<SetIndexOnContactEmail>();

            // Seed Data
            if (await addressCollection.CountDocumentsAsync(Builders<Address>.Filter.Empty) < 10)
            {
                services.AddHostedService<SeedAddresses>();
            }

            if (await companyCollection.CountDocumentsAsync(Builders<Company>.Filter.Empty) < 10)
            {
                services.AddHostedService<SeedCompanies>();
            }

            if (await contactCollection.CountDocumentsAsync(Builders<Contact>.Filter.Empty) < 10)
            {
                services.AddHostedService<SeedContacts>();
            }
        }
    }
}
