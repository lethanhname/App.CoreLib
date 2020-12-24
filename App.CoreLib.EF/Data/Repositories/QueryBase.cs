using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using App.CoreLib.EF.Extensions;
namespace App.CoreLib.EF.Data.Repositories
{

    public abstract class QueryBase<TResponse> : IQueryBase<TResponse>
        where TResponse : class, new()
    {
        public QueryBase(IStorage context)
        {
            storage = context;
            storageContext = context.StorageContext as DbContext;
        }

        protected IStorage storage { get; }

        protected DbContext storageContext { get; }

        public async Task<QueryResponse<TResponse>> ReadDataAsync(IQueryRequest request)
        {
            var response = new QueryResponse<TResponse>();
            if (string.IsNullOrEmpty(request.OrderColumn))
            {
                DefaultSort(request);
            }
            var dbSet = Define(request).OrderBy(request.OrderColumn, request.SortDirection);

            response.RecordsTotal = await dbSet.CountAsync();
            //Paging   
            response.Results = await dbSet.Skip(request.Skip).Take(request.Take).ToListAsync();
            return response;
        }

        protected abstract IQueryable<TResponse> Define(IQueryRequest request);

        protected abstract void DefaultSort(IQueryRequest request);

    }
    
}