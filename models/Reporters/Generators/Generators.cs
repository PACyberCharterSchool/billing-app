using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

using Dapper;

namespace models.Reporters.Generators
{
	public delegate dynamic GeneratorFunc(Dictionary<string, dynamic> input = null, Dictionary<string, dynamic> state = null);

	public static class Generator
	{
		public static GeneratorFunc Object(params (string Key, GeneratorFunc Generate)[] properties) => (input, state) =>
		{
			var next = new Dictionary<string, dynamic>();
			foreach (var property in properties)
				next.Add(property.Key, property.Generate);

			void Expand(Dictionary<string, dynamic> d)
			{
				var keys = d.Keys.ToArray();
				for (var i = 0; i < keys.Length; i++)
				{
					var key = keys[i];
					if (d[key] is GeneratorFunc)
						d[key] = d[key](input, next); // pass down top-level state

					if (d[key] is Dictionary<string, dynamic>)
						Expand(d[key]);
				}
			}

			if (state == null) // this is the top-level object
				Expand(next);

			return next;
		};

		public static GeneratorFunc Constant<T>(T constant) => (_, __) => constant;

		public static GeneratorFunc Input(Func<IReadOnlyDictionary<string, dynamic>, dynamic> select) =>
			(input, _) => select(input);

		public static GeneratorFunc Reference(Func<IReadOnlyDictionary<string, dynamic>, dynamic> select) =>
			(_, state) => select(state);

		private static GeneratorFunc Lambda(Delegate lambda, params GeneratorFunc[] generators) => (input, state) =>
		{
			dynamic[] values = null;
			if (generators != null && generators.Length > 0)
				values = generators.Select(g => g(input, state)).ToArray();

			return lambda.DynamicInvoke(values);
		};

		public static GeneratorFunc Lambda<R>(Func<R> lambda, params GeneratorFunc[] generators) =>
			Lambda(lambda as Delegate, generators);

		public static GeneratorFunc Lambda<T, R>(Func<T, R> lambda, params GeneratorFunc[] generators) =>
			Lambda(lambda as Delegate, generators);

		public static GeneratorFunc Lambda<T1, T2, R>(Func<T1, T2, R> lambda, params GeneratorFunc[] generators) =>
			Lambda(lambda as Delegate, generators);

		public static GeneratorFunc Sql(IDbConnection db, string query, params (string Key, GeneratorFunc Generator)[] properties)
		 => (input, state) =>
		{
			var args = new Dictionary<string, dynamic>();
			foreach (var property in properties)
				args.Add(property.Key, property.Generator(input, state));
			return db.Query<dynamic>(query, args);
		};
	}
}