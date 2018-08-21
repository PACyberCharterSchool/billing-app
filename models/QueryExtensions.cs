using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace models
{
	// Dynamically build SQL sort clause.
	// See: https://stackoverflow.com/a/36303246
	public static class QueryExtensions
	{
		public static IQueryable<T> SortBy<T>(this IQueryable<T> source, string field, SortDirection dir)
		{
			var exp = source.Expression;

			var param = Expression.Parameter(typeof(T), "x");
			var selector = Expression.PropertyOrField(param, field);

			var method = dir == SortDirection.Ascending ? "OrderBy" : "OrderByDescending";
			exp = Expression.Call(typeof(Queryable), method,
				new Type[] { source.ElementType, selector.Type },
				exp, Expression.Quote(Expression.Lambda(selector, param)));

			return source.Provider.CreateQuery<T>(exp);
		}


		// Dynamically build SQL where clause.
		// See: https://stackoverflow.com/a/39183597
		public static IQueryable<T> Filter<T>(this IQueryable<T> source, IFilterParser parser, string filter)
		{
			var pred = parser.Parse<T>("x", filter);
			return source.Where(pred as Expression<Func<T, bool>>);
		}

		public static IEnumerable<T> Filter<T>(this IEnumerable<T> source, IFilterParser parser, string filter)
		{
			var pred = parser.Parse<T>("x", filter);
			return source.Where(pred.Compile() as Func<T, bool>);
		}
	}
}
