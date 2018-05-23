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
	public interface IGenerator
	{
		dynamic Generate(Dictionary<string, dynamic> input, Dictionary<string, dynamic> state = null);
	}

	public sealed class ObjectGenerator : IGenerator
	{
		private readonly IDictionary<string, IGenerator> _properties;

		internal ObjectGenerator(IDictionary<string, IGenerator> properties) => _properties = properties;

		public dynamic Generate(Dictionary<string, dynamic> input, Dictionary<string, dynamic> state = null)
		{
			var next = new Dictionary<string, dynamic>();
			foreach (var property in _properties)
				next.Add(property.Key, property.Value.Generate(input, state));

			return next;
		}
	}

	public sealed class ConstantGenerator<T> : IGenerator
	{
		private readonly T _constant;

		internal ConstantGenerator(T constant) => _constant = constant;

		public dynamic Generate(Dictionary<string, dynamic> input, Dictionary<string, dynamic> state = null) => _constant;
	}

	public sealed class InputGenerator : IGenerator
	{
		private readonly Func<IReadOnlyDictionary<string, dynamic>, dynamic> _select;

		internal InputGenerator(Func<IReadOnlyDictionary<string, dynamic>, dynamic> select) => _select = select;

		public dynamic Generate(Dictionary<string, dynamic> input, Dictionary<string, dynamic> state = null) =>
			_select(input);
	}

	public sealed class ReferenceGenerator : IGenerator
	{
		private readonly Func<IReadOnlyDictionary<string, dynamic>, dynamic> _select;

		internal ReferenceGenerator(Func<IReadOnlyDictionary<string, dynamic>, dynamic> select) => _select = select;

		public dynamic Generate(Dictionary<string, dynamic> input, Dictionary<string, dynamic> state = null) =>
			_select(state);
	}

	public sealed class LambdaGenerator : IGenerator
	{
		private readonly LambdaExpression _lambda;
		private readonly Delegate _delegate;
		private readonly IList<IGenerator> _values;

		internal LambdaGenerator(LambdaExpression lambda, IList<IGenerator> values = null)
		{
			_lambda = lambda;
			_delegate = lambda.Compile();
			_values = values;
		}

		private static Func<Delegate, dynamic[], dynamic>[] _actions = new Func<Delegate, dynamic[], dynamic>[] {
			(del, _) => del.DynamicInvoke(),
			(del, values) => del.DynamicInvoke(values[0]),
			(del, values) => del.DynamicInvoke(values[0], values[1]),
		};

		public dynamic Generate(Dictionary<string, dynamic> input, Dictionary<string, dynamic> state = null)
		{
			var count = _lambda.Parameters.Count;
			dynamic[] values = null;
			if (_values != null)
				values = _values.Select(v => v.Generate(input, state)).ToArray();

			return _actions[count](_delegate, values);
		}
	}

	public sealed class SqlGenerator : IGenerator
	{
		private readonly IDbConnection _db;
		private readonly string _query;
		private readonly ObjectGenerator _args;

		internal SqlGenerator(IDbConnection db, string query, ObjectGenerator args = null)
		{
			_db = db;
			_query = query;
			_args = args;
		}

		public dynamic Generate(Dictionary<string, dynamic> input, Dictionary<string, dynamic> state = null)
		{
			if (_args == null)
				return _db.Query<dynamic>(_query);

			return _db.Query<dynamic>(_query, _args.Generate(input, state) as Dictionary<string, dynamic>);
		}
	}
}
