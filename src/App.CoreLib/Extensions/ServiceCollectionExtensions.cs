using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Loader;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using App.CoreLib.Module;

namespace App.CoreLib.Extensions
{
    public static class ServiceCollectionExtensions
    {

        public static void AddModules(this IServiceCollection services)
        {
            services.AddExtensions(false, new AssemblyProvider(services.BuildServiceProvider()));
        }

        public static void AddExtensions(this IServiceCollection services, bool includingSubpaths)
        {
            services.AddExtensions(includingSubpaths, new AssemblyProvider(services.BuildServiceProvider()));
        }

        public static void AddExtensions(this IServiceCollection services, IAssemblyProvider assemblyProvider)
        {
            services.AddExtensions(false, assemblyProvider);
        }

        public static void AddExtensions(this IServiceCollection services, bool includingSubpaths, IAssemblyProvider assemblyProvider)
        {
            services.AddHttpContextAccessor();
            ServiceCollectionExtensions.DiscoverAssemblies(assemblyProvider, Globals.ExtensionsPath, includingSubpaths);

            IServiceProvider serviceProvider = services.BuildServiceProvider();
            ILogger logger = serviceProvider.GetService<ILoggerFactory>().CreateLogger("CoreLib.WebApplication");

            foreach (IModuleInitializer action in ExtensionManager.GetInstances<IModuleInitializer>())
            {
                logger.LogInformation("Executing ConfigureServices action '{0}'", action.GetType().FullName);
                services.AddSingleton(typeof(IModuleInitializer), action);
                action.ConfigureServices(services);
            }
        }

        private static void DiscoverAssemblies(IAssemblyProvider assemblyProvider, string extensionsPath, bool includingSubpaths)
        {
            ExtensionManager.SetAssemblies(assemblyProvider.GetAssemblies(extensionsPath, includingSubpaths));
        }
    }
}