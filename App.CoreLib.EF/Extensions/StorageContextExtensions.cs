using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using App.CoreLib.EF.Context;
using App.CoreLib.EF.Data;
using App.CoreLib;

namespace App.CoreLib.EF.Extensions
{
    public static class StorageContextExtensions
    {
        public static void RegisterEntities(this ModelBuilder modelBuilder)
        {
            foreach (IEntityRegistrar entityRegistrar in ExtensionManager.GetInstances<IEntityRegistrar>())
                entityRegistrar.RegisterEntities(modelBuilder);
        }
        public static void RegisterConvention(this ModelBuilder modelBuilder)
        {
            foreach (var relationship in modelBuilder.Model.GetEntityTypes().SelectMany(e => e.GetForeignKeys()))
            {
                relationship.DeleteBehavior = DeleteBehavior.Restrict;
            }
        }
    }
}