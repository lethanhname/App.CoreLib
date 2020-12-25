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

        public static void AddModules(this IServiceCollection services, bool isBundledWithHost = true, bool includingSubpaths = false)
        {
            services.AddHttpContextAccessor();

            if (isBundledWithHost)
            {
                DiscoverAssemblies();
            }
            else
            {
                DiscoverAssemblies(new AssemblyProvider(services.BuildServiceProvider()), Globals.ExtensionsPath, includingSubpaths);
            }
            services.AddExtensions();
        }

        private static void AddExtensions(this IServiceCollection services)
        {
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

        private static void DiscoverAssemblies()
        {
            List<Assembly> assemblies = new List<Assembly>();
            var modules = ModuleConfigurationManager.GetModules();
            foreach (var module in modules)
            {
                var assembly = Assembly.Load(new AssemblyName(module.Id));
                assemblies.Add(assembly);
            }
            ExtensionManager.SetAssemblies(assemblies);
        }
    }
}