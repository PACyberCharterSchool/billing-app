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
		public static GeneratorFunc Object(params (string Key, GeneratorFunc Generate)[] properties) => (input, state) =>
		{
			var next = new Dictionary<string, dynamic>();
			foreach (var property in properties)
				next.Add(property.Key, property.Generate(input, next));
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

		private static GeneratorFunc Lambda(LambdaExpression lambda, params GeneratorFunc[] generators) => (input, state) =>
		{
			var count = lambda.Parameters.Count;
			dynamic[] values = null;
			if (generators != null && generators.Length > 0)
				values = generators.Select(g => g(input, state)).ToArray();

			return _actions[count](lambda.Compile(), values);
		};

		public static GeneratorFunc Lambda<R>(Expression<Func<R>> lambda, params GeneratorFunc[] generators) =>
			Lambda(lambda as LambdaExpression, generators);

		public static GeneratorFunc Lambda<T, R>(Expression<Func<T, R>> lambda, params GeneratorFunc[] generators) =>
			Lambda(lambda as LambdaExpression, generators);

		public static GeneratorFunc Lambda<T1, T2, R>(Expression<Func<T1, T2, R>> lambda, params GeneratorFunc[] generators) =>
			Lambda(lambda as LambdaExpression, generators);

		public static GeneratorFunc Sql(IDbConnection db, string query, params (string Key, GeneratorFunc Generator)[] properties)
		 => (input, state) =>
		{
			var args = Object(properties)(input, state);
			return db.Query<dynamic>(query, args as Dictionary<string, dynamic>);
		};
	}
}
