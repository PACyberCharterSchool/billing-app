using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Linq.Expressions;

using Dapper;

namespace models.Reporters.Generators
{
	public delegate dynamic GeneratorFunc(Dictionary<string, dynamic> input, Dictionary<string, dynamic> state = null);

	public static class Generator
	{
		public static GeneratorFunc Object(IDictionary<string, GeneratorFunc> properties) => (input, state) =>
		{
			var next = new Dictionary<string, dynamic>();
			foreach (var property in properties)
				next.Add(property.Key, property.Value(input, state));
			return next;
		};

		public static GeneratorFunc Constant<T>(T constant) => (_, __) => constant;

		public static GeneratorFunc Input(Func<IReadOnlyDictionary<string, dynamic>, dynamic> select) =>
			(input, _) => select(input);

		public static GeneratorFunc Reference(Func<IReadOnlyDictionary<string, dynamic>, dynamic> select) =>
			(_, state) => select(state);

		private static Func<Delegate, dynamic[], dynamic>[] _actions = new Func<Delegate, dynamic[], dynamic>[] {
			(del, _) => del.DynamicInvoke(),
			(del, values) => del.DynamicInvoke(values[0]),
			(del, values) => del.DynamicInvoke(values[0], values[1]),
		};

		private static GeneratorFunc Lambda(LambdaExpression lambda, IList<GeneratorFunc> generators = null) =>
			(input, state) =>
			{
				var count = lambda.Parameters.Count;
				dynamic[] values = null;
				if (generators != null)
					values = generators.Select(g => g(input, state)).ToArray();

				return _actions[count](lambda.Compile(), values);
			};

		public static GeneratorFunc Lambda<R>(Expression<Func<R>> lambda, IList<GeneratorFunc> generators = null) =>
			Lambda(lambda as LambdaExpression, generators);

		public static GeneratorFunc Lambda<T, R>(Expression<Func<T, R>> lambda, IList<GeneratorFunc> generators = null) =>
			Lambda(lambda as LambdaExpression, generators);

		public static GeneratorFunc Lambda<T1, T2, R>(Expression<Func<T1, T2, R>> lambda, IList<GeneratorFunc> generators = null) =>
			Lambda(lambda as LambdaExpression, generators);

		public static GeneratorFunc Sql(IDbConnection db, string query, GeneratorFunc generator = null) => (input, state) =>
		{
			if (generator == null)
				return db.Query<dynamic>(query);

			var args = generator(input, state);
			if (!(args is Dictionary<string, dynamic>))
				throw new ArgumentException("Sql param must be Object", nameof(generator));

			return db.Query<dynamic>(query, args as Dictionary<string, dynamic>);
		};
	}
}
