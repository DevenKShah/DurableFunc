using System.IO;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

[assembly: FunctionsStartup(typeof(DurableFunc.Startup))]

namespace DurableFunc
{
    public class Startup : FunctionsStartup
    {
        public override void Configure(IFunctionsHostBuilder builder)
        {
            var config = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("local.settings.json", optional: true, reloadOnChange: true)
                .AddEnvironmentVariables()
                .Build();
            string connectionString = config["SqlConnectionString"];
            builder.Services.AddDbContext<DurableFuncDbContext>(
                options => options.UseSqlServer(connectionString));
                //options => SqlServerDbContextOptionsExtensions.UseSqlServer(options, connectionString));

        }
    }
}