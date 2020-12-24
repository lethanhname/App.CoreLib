using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Console;
using Microsoft.Extensions.Options;
using App.CoreLib.EF.Data;
using App.CoreLib.EF.Data.Entity;
using App.CoreLib.EF.Extensions;
using App.CoreLib.EF.Messages;
using App.CoreLib;
using App.CoreLib.EF.Data.Identity;

namespace App.CoreLib.EF.Context
{


    public class AppDbContext : IdentityDbContext<
        AppUser, AppRole, string,
        AppUserClaim, AppUserRole, AppUserLogin,
        AppRoleClaim, AppUserToken>, IStorageContext
    {

        public AppDbContext(DbContextOptions options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<AppRole>(etb =>
            {
                etb.Property(e => e.Product).HasMaxLength(64);
                etb.Property(e => e.RowVersion).IsConcurrencyToken();
            });
            modelBuilder.Entity<AppUser>(etb =>
            {
                etb.Property(e => e.GivenName).IsRequired().HasMaxLength(64);
                etb.Property(e => e.FamilyName).IsRequired().HasMaxLength(64);

                etb.Property(e => e.RowVersion).IsConcurrencyToken();
            });

            modelBuilder.RegisterEntities();

            modelBuilder.RegisterConvention();

            base.OnModelCreating(modelBuilder);

            if (Database.ProviderName == "Microsoft.EntityFrameworkCore.Sqlite")
            {
                // SQLite does not have proper support for DateTimeOffset via Entity Framework Core, see the limitations
                // here: https://docs.microsoft.com/en-us/ef/core/providers/sqlite/limitations#query-limitations
                // To work around this, when the Sqlite database provider is used, all model properties of type DateTimeOffset

                foreach (var entityType in modelBuilder.Model.GetEntityTypes())
                {
                    var properties = entityType.ClrType.GetProperties().Where(p => p.PropertyType == typeof(DateTimeOffset) || p.PropertyType == typeof(DateTimeOffset?));
                    foreach (var property in properties)
                    {
                        modelBuilder
                            .Entity(entityType.Name)
                            .Property(property.Name)
                            .HasConversion(new DateTimeOffsetToBinaryConverter());
                    }

                    var decimalProperties = entityType.ClrType.GetProperties().Where(p => p.PropertyType == typeof(decimal) || p.PropertyType == typeof(decimal?));
                    foreach (var property in decimalProperties)
                    {
                        modelBuilder
                            .Entity(entityType.Name)
                            .Property(property.Name)
                            .HasConversion<double>();
                    }
                }
            }
        }


    }

}