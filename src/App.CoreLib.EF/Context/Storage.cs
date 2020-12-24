using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using App.CoreLib.EF.Data.Entity;
using App.CoreLib.EF.Messages;
using App.CoreLib;
using Microsoft.Data.SqlClient;
using System.Threading;
using App.CoreLib.EF.Events;

namespace App.CoreLib.EF.Context
{
    public class Storage : IStorage
    {
        public GenericErrorDescriber ErrorDescriber { get; set; }
        public IStorageContext StorageContext { get; private set; }
        public EventHandlerContainer eventHandler { get; set; }

        public Storage(AppDbContext storageContext, EventHandlerContainer eventHandlerContainer, GenericErrorDescriber describer = null)
        {
            if (!(storageContext is DbContext))
                throw new ArgumentException("The storageContext object must be an instance of the Microsoft.EntityFrameworkCore.DbContext class.");

            this.StorageContext = storageContext;
            ErrorDescriber = describer ?? new GenericErrorDescriber();
            this.eventHandler = eventHandlerContainer;
        }

        public async Task<EntityResult> SaveChangesAsync(CancellationToken cancellationToken = default(CancellationToken))
        {
            var messages = new List<EntityError>();
            try
            {
                var dbContext = (this.StorageContext as DbContext);
                var eventEntities = new List<EventEntity>();
                await OnBeforeSaveChangesAsync(dbContext, eventEntities, messages);
                if (messages.Count <= 0)
                {
                    await dbContext.SaveChangesAsync(cancellationToken);
                    await OnAfterSaveChangesAsync(dbContext, eventEntities, messages);
                }
            }
            catch (Exception ex)
            {
                messages.Add(ErrorDescriber.DefaultError(ex.InnerException.Message));
            }
            if (messages.Count > 0)
            {
                return EntityResult.Failed(messages.ToArray());
            }
            return EntityResult.Success;
        }

        private async Task OnBeforeSaveChangesAsync(DbContext dbContext, List<EventEntity> entities, List<EntityError> messages)
        {
            var events = new BeforeSaveEvent();
            foreach (var dbEntityEntry in dbContext.ChangeTracker.Entries().Where(x => x.State == EntityState.Added
                                                                                    || x.State == EntityState.Modified
                                                                                    || x.State == EntityState.Deleted))
            {
                IEntity entity = dbEntityEntry.Entity as IEntity;
                if (entity != null)
                {
                    if (dbEntityEntry.State == EntityState.Added)
                    {
                        entity.RowVersion = 1;
                        entities.Add(new EventEntity
                        {
                            EntityType = entity.GetType().ToString(),
                            State = EntityState.Added,
                            EntityData = entity
                        });
                    }
                    else if (dbEntityEntry.State == EntityState.Modified)
                    {
                        var valueBefore = dbEntityEntry.OriginalValues.GetValue<int>("RowVersion");

                        var value = entity.RowVersion;
                        if (value == Int32.MaxValue)
                            value = 1;
                        else value++;

                        var rowversion = dbEntityEntry.Property("RowVersion");
                        rowversion.CurrentValue = value;
                        rowversion.OriginalValue = valueBefore;

                        entities.Add(new EventEntity
                        {
                            EntityType = entity.GetType().ToString(),
                            State = EntityState.Modified,
                            EntityData = entity
                        });
                    }
                    else if (dbEntityEntry.State == EntityState.Deleted)
                    {
                        entities.Add(new EventEntity
                        {
                            EntityType = entity.GetType().ToString(),
                            State = EntityState.Deleted,
                            EntityData = entity
                        });
                    }
                }
                IEntity auditEntity = dbEntityEntry.Entity as IAuditEntity;
                if (auditEntity != null)
                {
                    DateTime currentDateTime = DateTime.Now;
                    var currentUserId = Globals.CurrentUser;

                    if (dbEntityEntry.State == EntityState.Added)
                    {
                        var createdById = dbEntityEntry.Property("CreatedById");
                        createdById.CurrentValue = currentUserId;

                        var createdDateTime = dbEntityEntry.Property("CreatedDateTime");
                        createdDateTime.CurrentValue = currentDateTime;
                    }
                    else if (dbEntityEntry.State == EntityState.Modified)
                    {
                        var changedById = dbEntityEntry.Property("ChangedById");
                        changedById.CurrentValue = currentUserId;

                        var changedDateTime = dbEntityEntry.Property("ChangedDateTime");
                        changedDateTime.CurrentValue = currentDateTime;
                    }
                }
            }
            events.Items = entities;
            await eventHandler.PublishAsync(events);
            foreach (var item in events.Items)
            {
                messages.AddRange(item.Messages);
            }
        }
        private async Task OnAfterSaveChangesAsync(DbContext dbContext, List<EventEntity> entities, List<EntityError> messages)
        {
            var events = new AfterSaveEvent();
            events.Items = entities;
            await eventHandler.PublishAsync(events);
            foreach (var item in events.Items)
            {
                messages.AddRange(item.Messages);
            }
        }
    }
}
