using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using MongoDB.Bson;
using MongoDB.Driver;

namespace XeppIT.ZoneElectrical.Identity.StartupConfigs
{
    public class MongoSetIndexesAsync : IHostedService
    {
        private readonly IServiceProvider _serviceProvider;

        public MongoSetIndexesAsync(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }
        public async Task StartAsync(CancellationToken cancellationToken)
        {
            // Create a new scope to retrieve scoped services
            using (var scope = _serviceProvider.CreateScope())
            {
                // Get the userManager instance
                var applicationUserCollection = scope.ServiceProvider.GetRequiredService<IMongoCollection<ApplicationUser>>();

                var applicationUserBuilder = Builders<ApplicationUser>.IndexKeys;
                var keys = new CreateIndexModel<ApplicationUser>(applicationUserBuilder.Ascending(x => x.UserName));
                await applicationUserCollection.Indexes.CreateOneAsync(keys, cancellationToken: cancellationToken).ConfigureAwait(false);
            }
        }

        public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
    }
}
