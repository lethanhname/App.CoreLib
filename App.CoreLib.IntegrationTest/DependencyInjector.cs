using System;
using Microsoft.Extensions.DependencyInjection;

namespace App.CoreLib.IntegrationTest
{
    internal static class DependencyInjector
    {
        public static IServiceProvider GetServiceProvider()
        {
            ServiceCollection services = new ServiceCollection();
            return services.BuildServiceProvider();
        }
    }
}