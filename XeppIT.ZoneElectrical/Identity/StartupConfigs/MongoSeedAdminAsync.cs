using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using MongoDB.Bson;
using MongoDB.Driver;

namespace XeppIT.ZoneElectrical.Identity.StartupConfigs
{
    public class MongoSeedAdminAsync : IHostedService
    {
        private readonly IServiceProvider _serviceProvider;

        public MongoSeedAdminAsync(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            // Create a new scope to retrieve scoped services
            using (var scope = _serviceProvider.CreateScope())
            {
                // Get the userManager instance
                var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();

                var usersInRole = await userManager.GetUsersInRoleAsync("Admin");
                //Do the migration asynchronously
                if (usersInRole.Count == 0)
                {
                    var newAdminUser = new ApplicationUser()
                    {
                        Id = ObjectId.GenerateNewId().ToString(),
                        UserName = "Admin",
                        NormalizedUserName = "ADMIN",
                        LockoutEnabled = false,
                        EmailConfirmed = true,
                        SecurityStamp = Guid.NewGuid().ToString()
                    };

                    await userManager.AddToRoleAsync(newAdminUser,"Admin");

                    await userManager.CreateAsync(newAdminUser, "admin");
                }
            }
        }
        public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
    }
}
