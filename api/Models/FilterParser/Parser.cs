using System;
using System.Collections.Generic;
using System.Linq.Expressions;

// this nasty mess saves us some screen real estate below, where it matters
using BooleanOpDictionary = System.Collections.Generic.Dictionary<
	string,
	System.Func<
		System.Linq.Expressions.MemberExpression,
		object,
		System.Type,
		System.Linq.Expressions.Expression
	>
>;

using CompoundOpDictionary = System.Collections.Generic.Dictionary<
	string,
	System.Func<
		System.Linq.Expressions.Expression,
		System.Linq.Expressions.Expression,
		System.Linq.Expressions.Expression
	>
>;

namespace api.Models.FilterParser
{
	public interface IParser
	{
		LambdaExpression Parse();
	}

	public class Parser<T> : IParser
	{
		private const char OPEN = '(';
		private const char CLOSE = ')';

		private readonly ParameterExpression _param;
		private readonly string _filter;

		private int _pos = 0;

		public Parser(string param, string filter)
		{
			_param = Expression.Parameter(typeof(T), param);
			_filter = filter;
		}

		public Parser(string filter) : this("x", filter) { }

		private char Next()
		{
			_pos++;
			if (_pos >= _filter.Length)
				throw new ArgumentException("Filter was not properly closed.");

			return _filter[_pos];
		}

		private void Backup()
		{
			_pos--;
		}

		private void IgnoreWhitespace()
		{
			while (char.IsWhiteSpace(Next()))
			{
				// advance _pos
			}
			Backup();
		}

		private MemberExpression ParseIdent()
		{
			var ident = string.Empty;

			while (true)
			{
				var c = Next();
				if (char.IsWhiteSpace(c))
				{
					Backup();
					return Expression.PropertyOrField(_param, ident);
				}

				ident += c;
			}
		}

		private static BooleanOpDictionary BooleanOpExpressions = new BooleanOpDictionary
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

		private Func<MemberExpression, object, Type, Expression> ParseBooleanOp()
		{
			var op = string.Empty;

			while (true)
			{
				var c = Next();
				if (char.IsWhiteSpace(c))
				{
					Backup();
					if (!BooleanOpExpressions.ContainsKey(op))
						throw new ArgumentException($"Invalid boolean operation '{op}' ending at [{_pos}].");

					return BooleanOpExpressions[op];
				}

				op += c;
			}
		}

		private object ParseValue(Type type)
		{
			var value = string.Empty;

			while (true)
			{
				var c = Next();
				if (char.IsWhiteSpace(c) || c == CLOSE)
				{
					Backup();
					return Convert.ChangeType(value, type);
				}

				value += c;
			}
		}

		private LambdaExpression ParseBooleanClause()
		{
			IgnoreWhitespace();

			var ident = ParseIdent();
			IgnoreWhitespace();

			var op = ParseBooleanOp();
			IgnoreWhitespace();

			var value = ParseValue(ident.Type);
			IgnoreWhitespace();

			var method = op(ident, value, ident.Type);
			return Expression.Lambda<Func<T, bool>>(method, _param);
		}

		private static CompoundOpDictionary CompoundOpExpressions = new CompoundOpDictionary
		{
			{"and", (left, right) => Expression.AndAlso(left, right)},
			{"or", (left, right) => Expression.OrElse(left, right)},
		};

		private Func<Expression, Expression, Expression> ParseCompoundOp()
		{
			var op = string.Empty;

			while (true)
			{
				var c = Next();
				if (char.IsWhiteSpace(c))
				{
					Backup();
					if (!CompoundOpExpressions.ContainsKey(op))
						throw new ArgumentException($"Invalid compound operation '{op}' ending at [{_pos}].");

					return CompoundOpExpressions[op];
				}

				op += c;
			}
		}

		private LambdaExpression ParseCompoundClause()
		{
			IgnoreWhitespace();

			var left = ParseClause();
			IgnoreWhitespace();

			var op = ParseCompoundOp();
			IgnoreWhitespace();

			var right = ParseClause();
			IgnoreWhitespace();

			var method = op(left.Body, right.Body);
			return Expression.Lambda<Func<T, bool>>(method, _param);
		}

		private LambdaExpression ParseClause()
		{
			var c = Next();
			if (c != OPEN)
				throw new ArgumentException($"Clause must begin with '{OPEN}'; found '{c}' at [{_pos}].");

			IgnoreWhitespace();

			LambdaExpression exp = null;

			c = Next();
			if (c == OPEN)
			{
				Backup();
				exp = ParseCompoundClause();
			}
			else // isValidIdent?
			{
				Backup();
				exp = ParseBooleanClause();
			}

			IgnoreWhitespace();

			c = Next();
			if (c != CLOSE)
				throw new ArgumentException($"Clause must end with '{CLOSE}'; found '{c}' at [{_pos}].");

			return exp;
		}

		public LambdaExpression Parse()
		{
			_pos = -1;
			return ParseClause();
		}
	}
}
