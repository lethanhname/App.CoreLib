using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using App.CoreLib.EF.Data.Entity;

namespace App.CoreLib.EF.Data
{
    public interface IQueryBase<TResponse> where TResponse : class, new()
    {
        Task<QueryResponse<TResponse>> ReadDataAsync(IQueryRequest request);
    }
    public interface IQueryRequest
    {
        string Draw { get; set; }
        int Take { get; set; }
        int Skip { get; set; }
        string OrderColumn { get; set; }
        string SortDirection { get; set; }

        string SearchValue { get; set; }
    }

    public class QueryRequestBase : IQueryRequest
    {
        public QueryRequestBase()
        {
            Take = 0;
            Skip = 0;
            Draw = string.Empty;
            OrderColumn = string.Empty;
            SortDirection = string.Empty;
            SearchValue = string.Empty;
        }
        public string Draw { get; set; }
        public int Take { get; set; }
        public int Skip { get; set; }
        public string OrderColumn { get; set; }
        public string SortDirection { get; set; }
        public string SearchValue { get; set; }
    }
    public class QueryResponse<T> where T : class, new()
    {
        public int RecordsTotal { get; set; }
        public List<T> Results { get; set; }
    }
}
