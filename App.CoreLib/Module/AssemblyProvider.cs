using Microsoft.Extensions.DependencyModel;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Loader;
using Microsoft.Extensions.DependencyInjection;

namespace App.CoreLib.Module
{
    public interface IAssemblyProvider
    {
        IEnumerable<Assembly> GetAssemblies(string path, bool includingSubpaths);
    }
    public class AssemblyProvider : IAssemblyProvider
    {
        protected ILogger logger;

        public Func<Assembly, bool> IsCandidateAssembly { get; set; }

        public Func<Library, bool> IsCandidateCompilationLibrary { get; set; }

        public AssemblyProvider(IServiceProvider serviceProvider)
        {
            this.logger = serviceProvider.GetService<ILoggerFactory>().CreateLogger("CoreLib");
            this.IsCandidateAssembly = assembly =>
              !assembly.FullName.StartsWith("System.", StringComparison.OrdinalIgnoreCase) &&
              !assembly.FullName.StartsWith("Microsoft.", StringComparison.OrdinalIgnoreCase);

            this.IsCandidateCompilationLibrary = library =>
              !library.Name.StartsWith("System.", StringComparison.OrdinalIgnoreCase) &&
              !library.Name.StartsWith("Microsoft.", StringComparison.OrdinalIgnoreCase) &&
              !library.Name.StartsWith("Newtonsoft.", StringComparison.OrdinalIgnoreCase) &&
              !library.Name.StartsWith("runtime.", StringComparison.OrdinalIgnoreCase) &&
              !library.Name.Equals("NETStandard.Library", StringComparison.OrdinalIgnoreCase) &&
              !library.Name.Equals("Libuv", StringComparison.OrdinalIgnoreCase) &&
              !library.Name.Equals("Remotion.Linq", StringComparison.OrdinalIgnoreCase) &&
              !library.Name.Equals("StackExchange.Redis.StrongName", StringComparison.OrdinalIgnoreCase) &&
              !library.Name.Equals("WindowsAzure.Storage", StringComparison.OrdinalIgnoreCase);
        }

        public IEnumerable<Assembly> GetAssemblies(string path, bool includingSubpaths)
        {
            List<Assembly> assemblies = new List<Assembly>();
            this.logger.LogInformation("Starting loading assemblies");
            this.GetAssembliesFromDependencyContext(assemblies);
            this.GetAssembliesFromPath(assemblies, path, includingSubpaths);
            return assemblies;
        }

        private void GetAssembliesFromPath(List<Assembly> assemblies, string path, bool includingSubpaths)
        {
            this.logger.LogInformation("Starting loading assemblies from path '{0}'", path);
            if (!string.IsNullOrEmpty(path) && Directory.Exists(path))
            {
                this.logger.LogInformation("Discovering and loading assemblies from path '{0}'", path);

                foreach (string extensionPath in Directory.EnumerateFiles(path, "*.dll"))
                {
                    Assembly assembly = null;

                    try
                    {
                        assembly = AssemblyLoadContext.Default.LoadFromAssemblyPath(extensionPath);

                        if (this.IsCandidateAssembly(assembly) && !assemblies.Any(a => string.Equals(a.FullName, assembly.FullName, StringComparison.OrdinalIgnoreCase)))
                        {
                            assemblies.Add(assembly);
                            this.logger.LogInformation("Assembly '{0}' is discovered and loaded", assembly.FullName);
                        }
                    }

                    catch (Exception e)
                    {
                        this.logger.LogWarning("Error loading assembly '{0}'", extensionPath);
                        this.logger.LogWarning(e.ToString());
                    }
                }

                if (includingSubpaths)
                    foreach (string subpath in Directory.GetDirectories(path))
                        this.GetAssembliesFromPath(assemblies, subpath, includingSubpaths);
            }

            else
            {
                if (string.IsNullOrEmpty(path))
                    this.logger.LogWarning("Discovering and loading assemblies from path skipped: path not provided", path);

                else this.logger.LogWarning("Discovering and loading assemblies from path '{0}' skipped: path not found", path);
            }
        }

        private void GetAssembliesFromDependencyContext(List<Assembly> assemblies)
        {
            this.logger.LogInformation("Discovering and loading assemblies from DependencyContext");

            foreach (CompilationLibrary compilationLibrary in DependencyContext.Default.CompileLibraries)
            {
                if (this.IsCandidateCompilationLibrary(compilationLibrary))
                {
                    Assembly assembly = null;

                    try
                    {
                        assembly = AssemblyLoadContext.Default.LoadFromAssemblyName(new AssemblyName(compilationLibrary.Name));

                        if (!assemblies.Any(a => string.Equals(a.FullName, assembly.FullName, StringComparison.OrdinalIgnoreCase)))
                        {
                            assemblies.Add(assembly);
                            this.logger.LogInformation("Assembly '{0}' is discovered and loaded", assembly.FullName);
                        }
                    }

                    catch (Exception e)
                    {
                        this.logger.LogWarning("Error loading assembly '{0}'", compilationLibrary.Name);
                        this.logger.LogWarning(e.ToString());
                    }
                }
            }
        }
    }
}
