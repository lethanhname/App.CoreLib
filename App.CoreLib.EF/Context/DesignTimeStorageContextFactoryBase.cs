using System;
using System.IO;
using System.Reflection;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using App.CoreLib.EF.Extensions;
using App.CoreLib;
using App.CoreLib.Extensions;

namespace App.CoreLib.EF.Context
{
    public abstract class DesignTimeStorageContextFactoryBase : IDesignTimeDbContextFactory<AppDbContext>
    {
        public IServiceCollection Services { get; private set; }
        public AppDbContext CreateDbContext(string[] args)
        {
            var environmentName = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");

            var basePath = Directory.GetCurrentDirectory();

            var builder = new ConfigurationBuilder()
                .SetBasePath(basePath)
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddJsonFile($"appsettings.{environmentName}.json", true);

            builder.AddEnvironmentVariables();
            var configuration = builder.Build();

            //setup DI
            Services = new ServiceCollection();
            Services.AddLogging();

            Globals.SetRoot(basePath, configuration["Extensions:Path"]);
            //services.AddPostgres(new StorageContextOptions
            //{
            //    ConnectionString = configuration.GetConnectionString(DefaultConnectionName),
            //    MigrationsAssembly = MigrationsAssembly
            //});
            AddDatabase(Services, configuration);
            Services.AddModules();
            var serviceProvider = Services.BuildServiceProvider();

            return serviceProvider.GetRequiredService<AppDbContext>();
        }

        public abstract void AddDatabase(IServiceCollection services, IConfiguration configuration);

    }
}