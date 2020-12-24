using System;
using System.IO;
using System.Reflection;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using App.CoreLib.EF.Context;
using App.CoreLib.EF.Extensions;
using App.CoreLib;
using App.CoreLib.Extensions;

namespace App.CoreLib.IntegrationTest
{

    public class IntegrationTestFactory : DesignTimeStorageContextFactoryBase
    {
        public override void AddDatabase(IServiceCollection services, IConfiguration configuration)
        {
            services.AddPostgres(new StorageContextOptions
            {
                ConnectionString = configuration.GetConnectionString("DefaultConnection"),
                MigrationsAssembly = typeof(IntegrationTestFactory).GetTypeInfo().Assembly.FullName
            });
        }
    }
}