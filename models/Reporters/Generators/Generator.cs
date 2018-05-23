using System;
using System.Collections.Generic;
using System.Data;
using System.Linq.Expressions;

namespace models.Reporters.Generators
{
	public static class Generator
	{
		public static ObjectGenerator Object(IDictionary<string, IGenerator> properties) =>
			new ObjectGenerator(properties);

		public static ConstantGenerator<T> Constant<T>(T constant) => new ConstantGenerator<T>(constant);

		public static InputGenerator Input(Func<IReadOnlyDictionary<string, dynamic>, dynamic> select) =>
			new InputGenerator(select);

		public static ReferenceGenerator Reference(Func<IReadOnlyDictionary<string, dynamic>, dynamic> select) =>
			new ReferenceGenerator(select);

		public static LambdaGenerator Lambda<R>(Expression<Func<R>> lambda) => new LambdaGenerator(lambda);

		public static LambdaGenerator Lambda<T, R>(Expression<Func<T, R>> lambda, IList<IGenerator> values = null) =>
			new LambdaGenerator(lambda, values);

		public static LambdaGenerator Lambda<T1, T2, R>(Expression<Func<T1, T2, R>> lambda, IList<IGenerator> values = null) =>
			new LambdaGenerator(lambda, values);

		public static SqlGenerator Sql(IDbConnection conn, string query, ObjectGenerator args = null) =>
			new SqlGenerator(conn, query, args);
	}
}
