using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Driver;
using XeppIT.ZoneElectrical.Rolodex.Addresses.Config;
using XeppIT.ZoneElectrical.Rolodex.Addresses.Model;
using XeppIT.ZoneElectrical.Rolodex.Addresses.States;
using XeppIT.ZoneElectrical.Rolodex.Companies.Config;
using XeppIT.ZoneElectrical.Rolodex.Companies.Model;
using XeppIT.ZoneElectrical.Rolodex.Companies.States;
using XeppIT.ZoneElectrical.Rolodex.Contacts.Config;
using XeppIT.ZoneElectrical.Rolodex.Contacts.Model;
using XeppIT.ZoneElectrical.Rolodex.Contacts.States;

namespace XeppIT.ZoneElectrical.Rolodex
{
    public static class RolodexServiceBuilder
    {
        public static void RegisterRolodexServices(
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
            services.AddScoped<AddressEditState>();
            services.AddScoped<CompanyEditState>();
            services.AddScoped<ContactEditState>();
            services.AddScoped<RolodexService>();

            // Set indexes on db
            services.AddHostedService<SetIndexOnCompanyNameAsync>();
            services.AddHostedService<SetIndexOnContactEmail>();

            // Todo move this to an admin page with option to upload see data
            // Seed Data
            //if (await addressCollection.CountDocumentsAsync(Builders<Address>.Filter.Empty) < 10)
            //{
            //    services.AddHostedService<SeedAddresses>();
            //}

            //if (await companyCollection.CountDocumentsAsync(Builders<Company>.Filter.Empty) < 10)
            //{
            //    services.AddHostedService<SeedCompanies>();
            //}

            //if (await contactCollection.CountDocumentsAsync(Builders<Contact>.Filter.Empty) < 10)
            //{
            //    services.AddHostedService<SeedContacts>();
            //}
        }
    }
}
