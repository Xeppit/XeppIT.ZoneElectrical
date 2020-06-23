using Microsoft.Extensions.DependencyInjection;
using MongoDB.Driver;
using XeppIT.ZoneElectrical.Project.States;

namespace XeppIT.ZoneElectrical.Project.Configs
{
    public static class ProjectServiceBuilder
    {
        public static void RegisterProjectServices(
            this IServiceCollection services, 
            string connectionString,
            string databaseName = "ZoneElectrical"
        )
        {
            var client = new MongoClient(connectionString);

            var database = client.GetDatabase(databaseName);

            var projectCollection = database.GetCollection<Models.ProjectModel>("Projects");

            services.AddScoped(provider => projectCollection);
            services.AddScoped<ProjectEditState>();
            services.AddScoped<ProjectManager>();

            // Set indexes on db
            services.AddHostedService<SetIndexOnJobNumberAsync>();
        }
    }
}
