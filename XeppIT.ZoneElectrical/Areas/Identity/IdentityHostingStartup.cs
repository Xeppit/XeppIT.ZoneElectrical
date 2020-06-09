using Microsoft.AspNetCore.Hosting;

[assembly: HostingStartup(typeof(XeppIT.ZoneElectrical.Areas.Identity.IdentityHostingStartup))]
namespace XeppIT.ZoneElectrical.Areas.Identity
{
    public class IdentityHostingStartup : IHostingStartup
    {
        public void Configure(IWebHostBuilder builder)
        {
            builder.ConfigureServices((context, services) => {
            });
        }
    }
}