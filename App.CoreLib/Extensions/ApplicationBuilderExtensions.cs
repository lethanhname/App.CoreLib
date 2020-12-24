using System.ComponentModel.DataAnnotations;
using System.Net.Http;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using App.CoreLib.Module;

namespace App.CoreLib.Extensions
{
    public static class ApplicationBuilderExtensions
    {
        public static void UseAppModules(this IApplicationBuilder applicationBuilder, IWebHostEnvironment env)
        {
            ILogger logger = applicationBuilder.ApplicationServices.GetService<ILoggerFactory>().CreateLogger("CoreLib");

            var httpContextAccessor = applicationBuilder.ApplicationServices.GetRequiredService<IHttpContextAccessor>();
            Globals.SetHttpContext(httpContextAccessor.HttpContext);
            var moduleInitializers = applicationBuilder.ApplicationServices.GetServices<IModuleInitializer>();
            foreach (var moduleInitializer in moduleInitializers)
            {
                var type = moduleInitializer.GetType();
                logger.LogInformation("Executing Configure action '{0}'", type.FullName);
                moduleInitializer.Configure(applicationBuilder, env);
            }
        }
    }
}