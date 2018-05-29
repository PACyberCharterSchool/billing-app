using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

using Dapper;

namespace models.Reporters.Generators
{
	public class State : Dictionary<string, dynamic> { }

	public delegate dynamic GeneratorFunc(dynamic input = null, State state = null);

	public static class Generator
	{
		public static GeneratorFunc Object(params (string Key, GeneratorFunc Generate)[] properties) => (input, state) =>
		{
			var next = new State();
			foreach (var property in properties)
				next.Add(property.Key, property.Generate);

			void Expand(State s)
			{
				var keys = s.Keys.ToArray();
				for (var i = 0; i < keys.Length; i++)
				{
					var key = keys[i];
					if (s[key] is GeneratorFunc)
						s[key] = s[key](input, next); // pass down top-level state

					if (s[key] is State)
						Expand(s[key]);
				}
			}

			if (state == null) // this is the top-level object
				Expand(next);

			return next;
		};

		public static GeneratorFunc Constant<T>(T constant) => (_, __) => constant;

		public static GeneratorFunc Input(Func<dynamic, dynamic> select) => (input, _) => select(input);

		public static GeneratorFunc Input<T>(Func<T, dynamic> select) => (input, _) => select(input);

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

		public static GeneratorFunc Lambda<T1, T2, T3, R>(Func<T1, T2, T3, R> lambda, params GeneratorFunc[] generators) =>
			Lambda(lambda as Delegate, generators);

		private static Dictionary<string, dynamic> SqlArgs(
			dynamic input, State state,
			params (string Key, GeneratorFunc Generator)[] properties)
		{
			var args = new Dictionary<string, dynamic>();
			foreach (var property in properties)
				args.Add(property.Key, property.Generator(input, state));
			return args;
		}

		public static GeneratorFunc SqlList<T>(
			IDbConnection db, string query,
			params (string Key, GeneratorFunc Generator)[] properties) => (input, state) =>
				db.Query<T>(query, SqlArgs(input, state, properties) as Dictionary<string, dynamic>);

		public static GeneratorFunc SqlList(
			IDbConnection db, string query,
			params (string Key, GeneratorFunc Generator)[] properties) => SqlList<dynamic>(db, query, properties);

		public static GeneratorFunc SqlObject<T>(
			IDbConnection db, string query,
			params (string Key, GeneratorFunc Generator)[] properties) => (input, state) =>
				db.Query<T>(query, SqlArgs(input, state, properties) as Dictionary<string, dynamic>).SingleOrDefault();

		public static GeneratorFunc SqlObject(
			IDbConnection db, string query,
			params (string Key, GeneratorFunc Generator)[] properties) => SqlObject<dynamic>(db, query, properties);
	}
}
