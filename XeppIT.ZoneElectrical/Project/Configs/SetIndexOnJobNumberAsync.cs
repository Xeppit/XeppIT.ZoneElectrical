using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using MongoDB.Driver;
using XeppIT.ZoneElectrical.Identity;
using XeppIT.ZoneElectrical.Project.Models;

namespace XeppIT.ZoneElectrical.Project.Configs
{
    public class SetIndexOnJobNumberAsync : IHostedService
    {
        private readonly IServiceProvider _serviceProvider;

        public SetIndexOnJobNumberAsync(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            // Create a new scope to retrieve scoped services
            using var scope = _serviceProvider.CreateScope();

            // Get the userManager instance
            var projectCollection = scope.ServiceProvider.GetRequiredService<IMongoCollection<ProjectModel>>();

            var projectBuilder = Builders<ProjectModel>.IndexKeys;
            var keys = new CreateIndexModel<ProjectModel>(projectBuilder.Ascending(x => x.JobNo), new CreateIndexOptions(){Unique = true});
            await projectCollection.Indexes.CreateOneAsync(keys, cancellationToken: cancellationToken).ConfigureAwait(false);
        }

        public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
    }
}
