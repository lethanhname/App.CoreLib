using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;
using App.CoreLib.EF.Context;
using App.CoreLib.EF.Data;
using App.CoreLib.EF.Data.Repositories;
using App.CoreLib.EF.Messages;
using App.CoreLib.EF.Events;

namespace App.CoreLib.EF.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddSqlServer(this IServiceCollection services, StorageContextOptions contextOptions)
        {
            services.AddDbContextPool<AppDbContext>(options =>
                options.UseSqlServer(contextOptions.ConnectionString, b => b.MigrationsAssembly(contextOptions.MigrationsAssembly))
                .UseLoggerFactory(logerFactory));

            AddStorageServices(services);

            return services;
        }
        public static IServiceCollection AddPostgres(this IServiceCollection services, StorageContextOptions contextOptions)
        {
            services.AddDbContextPool<AppDbContext>(options =>
                options.UseNpgsql(contextOptions.ConnectionString, b => b.MigrationsAssembly(contextOptions.MigrationsAssembly))
                .UseLoggerFactory(logerFactory));

            AddStorageServices(services);

            return services;
        }
        public static readonly ILoggerFactory logerFactory = LoggerFactory.Create(builder => { builder.AddConsole(); });

        private static void AddStorageServices(IServiceCollection services)
        {
            services.TryAddScoped<EventHandlerContainer>();
            services.TryAddScoped<GenericErrorDescriber>();
            services.TryAddScoped(typeof(IStorage), typeof(Storage));
            services.TryAddScoped(typeof(IRepository<>), typeof(Repository<>));
        }
    }
}