using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.Storage;
using App.CoreLib.EF.Data.Entity;
using App.CoreLib.EF.Messages;

namespace App.CoreLib.EF.Data
{
    public interface IRepository<T> 
        where T : IEntity
    {
        IQueryable<T> Query();

        void Add(T entity);
        void Edit(T entity);

        void AddRange(IEnumerable<T> entity);

        IDbContextTransaction BeginTransaction();

        Task<EntityResult> SaveChangesAsync(CancellationToken cancellationToken = default(CancellationToken));

        void Remove(params object[] keyValues);
        T Find(params object[] keyValues);
        void Remove(T entity);
    }
}