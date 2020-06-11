using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using MongoDB.Driver;
using XeppIT.ZoneElectrical.Rolodex.Models;

namespace XeppIT.ZoneElectrical.Rolodex.Config
{
    public class SeedAddresses : IHostedService
    {
        private readonly IServiceProvider _serviceProvider;

        public SeedAddresses(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            // Create a new scope to retrieve scoped services
            using var scope = _serviceProvider.CreateScope();

            // Get the userManager instance
            //var contactCollection = scope.ServiceProvider.GetRequiredService<IMongoCollection<Contact>>();
            var rolodexService = scope.ServiceProvider.GetRequiredService<RolodexService>();
            var x = await File.ReadAllTextAsync(@"Rolodex\Config\Addresses.json");
            var y = JsonSerializer.Deserialize<List<Address>>(x);
            foreach (var z in y)
            {
                await rolodexService.CreateAddressAsync(z);
            }
        }

        public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
    }
}
