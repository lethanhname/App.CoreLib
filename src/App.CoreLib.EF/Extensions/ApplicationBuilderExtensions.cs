using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;
using App.CoreLib.EF.Context;

namespace App.CoreLib.EF.Extensions
{
    public static class ApplicationBuilderExtensions
    {
        public static void DatabaseMigrate(this IApplicationBuilder applicationBuilder, IWebHostEnvironment env)
        {
            using (var scope = applicationBuilder.ApplicationServices.CreateScope())
            using (var context = scope.ServiceProvider.GetService<AppDbContext>())
                context.Database.Migrate();
        }
    }
}
