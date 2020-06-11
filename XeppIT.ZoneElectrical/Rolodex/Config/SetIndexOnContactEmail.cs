using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using MongoDB.Driver;
using XeppIT.ZoneElectrical.Rolodex.Models;

namespace XeppIT.ZoneElectrical.Rolodex.Config
{
    public class SetIndexOnContactEmail : IHostedService
    {
        private readonly IServiceProvider _serviceProvider;

        public SetIndexOnContactEmail(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            // Create a new scope to retrieve scoped services
            using var scope = _serviceProvider.CreateScope();

            // Get the userManager instance
            var contactCollection = scope.ServiceProvider.GetRequiredService<IMongoCollection<Contact>>();

            var projectBuilder = Builders<Contact>.IndexKeys;
            var keys = new CreateIndexModel<Contact>(projectBuilder.Ascending(x => x.Email), new CreateIndexOptions(){Unique = true});
            await contactCollection.Indexes.CreateOneAsync(keys, cancellationToken: cancellationToken).ConfigureAwait(false);
        }

        public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
    }
}
