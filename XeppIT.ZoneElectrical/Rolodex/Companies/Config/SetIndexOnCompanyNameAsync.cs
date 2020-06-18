using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using MongoDB.Driver;
using XeppIT.ZoneElectrical.Rolodex.Companies.Model;

namespace XeppIT.ZoneElectrical.Rolodex.Companies.Config
{
    public class SetIndexOnCompanyNameAsync : IHostedService
    {
        private readonly IServiceProvider _serviceProvider;

        public SetIndexOnCompanyNameAsync(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            // Create a new scope to retrieve scoped services
            using var scope = _serviceProvider.CreateScope();

            // Get the userManager instance
            var companyCollection = scope.ServiceProvider.GetRequiredService<IMongoCollection<Company>>();

            var projectBuilder = Builders<Company>.IndexKeys;
            var keys = new CreateIndexModel<Company>(projectBuilder.Ascending(x => x.Name), new CreateIndexOptions(){Unique = true});
            await companyCollection.Indexes.CreateOneAsync(keys, cancellationToken: cancellationToken).ConfigureAwait(false);
        }

        public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
    }
}
