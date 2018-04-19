using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

// this nasty mess saves us some screen real estate below, where it matters
using OpDictionary = System.Collections.Generic.Dictionary<
	string,
	System.Func<
		System.Linq.Expressions.MemberExpression,
		object,
		System.Type,
		System.Linq.Expressions.Expression
	>
>;

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


		private static OpDictionary OpExpressions = new OpDictionary
		{
			{"eq", (s, v, t) => Expression.Equal(s, Expression.Constant(v, t))},
			{"ne", (s, v, t) => Expression.NotEqual(s, Expression.Constant(v, t))},
			{"gt", (s, v, t) => Expression.GreaterThan(s, Expression.Constant(v, t))},
			{"ge", (s, v, t) => Expression.GreaterThanOrEqual(s, Expression.Constant(v, t))},
			{"lt", (s, v, t) => Expression.LessThan(s, Expression.Constant(v, t))},
			{"le", (s, v, t) => Expression.LessThanOrEqual(s, Expression.Constant(v, t))},
			{"has", (s, v, t) => Expression.Call(s, typeof(string).GetMethod("Contains"), Expression.Constant(v, t))},
			{"bgn", (s, v, t) => Expression.Call(s, typeof(string).GetMethod("StartsWith", new[]{typeof(string)}), Expression.Constant(v, t))},
			{"end", (s, v, t) => Expression.Call(s, typeof(string).GetMethod("EndsWith", new[]{typeof(string)}), Expression.Constant(v, t))},
		};

		// Dynamically build SQL where clause.
		// See: https://stackoverflow.com/a/39183597
		public static IQueryable<T> Filter<T>(this IQueryable<T> source, string field, string op, object value)
		{
			var param = Expression.Parameter(typeof(T), "x");
			var selector = Expression.PropertyOrField(param, field);
			var type = selector.Type;
			if (value != null && value.GetType() != type)
				value = Convert.ChangeType(value, type);

			Expression method;
			if (!OpExpressions.ContainsKey(op))
				throw new ArgumentException($"Invalid WhereFilter operation '{op}'.");

			method = OpExpressions[op](selector, value, type);
			var pred = Expression.Lambda<Func<T, bool>>(method, param);

			Console.WriteLine($"pred: {pred}");
			return source.Where(pred);
		}
	}
}
