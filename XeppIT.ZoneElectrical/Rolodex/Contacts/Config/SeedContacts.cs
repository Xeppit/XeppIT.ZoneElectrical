using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using XeppIT.ZoneElectrical.Rolodex.Contacts.Model;

namespace XeppIT.ZoneElectrical.Rolodex.Contacts.Config
{
    public class SeedContacts : IHostedService
    {
        private readonly IServiceProvider _serviceProvider;

        public SeedContacts(IServiceProvider serviceProvider)
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
            var x = await File.ReadAllTextAsync(@"Rolodex\Config\Contacts.json");
            var y = JsonSerializer.Deserialize<List<Contact>>(x);
            foreach (var z in y)
            {
                try
                {
                    await rolodexService.CreateContactAsync(z);
                }
                catch (Exception)
                {
                    //Fail Silent
                }
            }
        }

        public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
    }
}
