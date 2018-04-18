using System;
using System.Linq;
using System.Linq.Expressions;

namespace api.Models
{
	// Dynamically build SQL sort clause.
	// See: https://stackoverflow.com/a/36303246
	public static class QueryExtensions
	{
		public static IQueryable<T> SortBy<T>(this IQueryable<T> source, string field, string dir)
		{
			var exp = source.Expression;

			var param = Expression.Parameter(typeof(T), "x");
			var selector = Expression.PropertyOrField(param, field);

			var method = dir == "asc" ? "OrderBy" : "OrderByDescending";
			exp = Expression.Call(typeof(Queryable), method,
				new Type[] { source.ElementType, selector.Type },
				exp, Expression.Quote(Expression.Lambda(selector, param)));

			return source.Provider.CreateQuery<T>(exp);
		}
	}
}
