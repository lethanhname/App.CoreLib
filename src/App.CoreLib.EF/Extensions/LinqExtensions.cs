using System;
using System.Linq;
using System.Linq.Expressions;

namespace App.CoreLib.EF.Extensions
{
    public static class LinqExtensions
    {
        public static IOrderedQueryable<T> OrderBy<T>(this IQueryable<T> source, string propertyName, string direction)
        {
            var parameter = Expression.Parameter(typeof(T), "x");
            var selector = Expression.PropertyOrField(parameter, propertyName);
            var method = string.Equals(direction, "desc", StringComparison.OrdinalIgnoreCase) ? "OrderByDescending" : "OrderBy";
            var expression = Expression.Call(
              typeof(Queryable),
              method,
              new Type[] { source.ElementType, selector.Type },
              source.Expression,
              Expression.Quote(Expression.Lambda(selector, parameter))
            );

            return source.Provider.CreateQuery<T>(expression) as IOrderedQueryable<T>;
        }

        public static IOrderedQueryable<T> ThenBy<T>(this IQueryable<T> source, string propertyName, string direction)
        {
            var parameter = Expression.Parameter(typeof(T), "x");
            var selector = Expression.PropertyOrField(parameter, propertyName);
            var method = string.Equals(direction, "desc", StringComparison.OrdinalIgnoreCase) ? "ThenByDescending" : "ThenBy";
            var expression = Expression.Call(
              typeof(Queryable),
              method,
              new Type[] { source.ElementType, selector.Type },
              source.Expression,
              Expression.Quote(Expression.Lambda(selector, parameter))
            );

            return source.Provider.CreateQuery<T>(expression) as IOrderedQueryable<T>;
        }
    }
}