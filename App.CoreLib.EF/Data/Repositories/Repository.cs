using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using App.CoreLib.EF.Data.Entity;
using App.CoreLib.EF.Messages;

namespace App.CoreLib.EF.Data.Repositories
{
    public class Repository<TEntity> : IRepository<TEntity>
        where TEntity : class, IEntity
    {
        public Repository(IStorage context)
        {
            Storage = context;
            StorageContext = context.StorageContext as DbContext;
            DbSet = StorageContext.Set<TEntity>();
        }

        protected IStorage Storage { get; }

        protected DbContext StorageContext { get; }

        protected DbSet<TEntity> DbSet { get; }

        public TEntity Find(params object[] keyValues)
        {
            return DbSet.Find(keyValues);
        }

        public void Add(TEntity entity)
        {
            DbSet.Add(entity);
        }
        public void Edit(TEntity entity)
        {
            StorageContext.Entry(entity).State = EntityState.Modified;
        }
        public void AddRange(IEnumerable<TEntity> entity)
        {
            DbSet.AddRange(entity);
        }

        public IDbContextTransaction BeginTransaction()
        {
            return StorageContext.Database.BeginTransaction();
        }

        public Task<EntityResult> SaveChangesAsync(CancellationToken cancellationToken = default(CancellationToken))
        {
            return Storage.SaveChangesAsync(cancellationToken);
        }

        public IQueryable<TEntity> Query()
        {
            return DbSet.AsNoTracking();
        }

        public void Remove(params object[] keyValues)
        {
            this.Remove(this.Find(keyValues));
        }

        public void Remove(TEntity entity)
        {
            DbSet.Remove(entity);
        }
    }
}