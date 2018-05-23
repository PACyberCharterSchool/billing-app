using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;

using NUnit.Framework;

using models.Reporters.Generators;
using static models.Reporters.Generators.Generator;

namespace models.Tests.Reporters.Generators
{
	[TestFixture]
	public class GeneratorsTests
	{
		[Test]
		public void InputGeneratorReturnsFromInput()
		{
			var key = "a";
			var value = 1;
			var input = new Dictionary<string, dynamic>
			{
				{key, value},
			};

			var actual = Input(i => i[key]).Generate(input);
			Assert.That(actual, Is.EqualTo(value));
		}

		[Test]
		public void PropertiesGeneratorSetsProperty()
		{
			var key = "a";
			var value = 1;
			var input = new Dictionary<string, dynamic>
			{
				{key, value},
			};

			var prop = "A";
			var actual = Properties(new Dictionary<string, IGenerator> {
				{prop, Input(i => i[key])},
			}).Generate(input);

			Assert.That(actual, Contains.Key(prop));
			Assert.That(actual[prop], Is.EqualTo(value));
		}

		[Test]
		public void ReferenceGeneratorReturnsFromState()
		{
			var value = 1;
			var state = new Dictionary<string, dynamic>
			{
				{"a", new Dictionary<string, dynamic>
				{
					{"b", value},
				}},
			};

			var actual = Reference(s => s["a"]["b"]).Generate(input: null, state: state);

			Assert.That(actual, Is.EqualTo(value));
		}

		[Test]
		public void LambdaGenerateWithNoParamsReturnsResults()
		{
			var value = 1;
			var actual = Lambda(() => value).Generate(null);
			Assert.That(actual, Is.EqualTo(value));
		}

		[Test]
		public void LambdaGenerateWithOneParamReturnsResults()
		{
			var value = 1;
			var actual = Lambda((int x) => x * x, new[] { Constant(value) }).Generate(null);
			Assert.That(actual, Is.EqualTo(value * value));
		}

		[Test]
		public void LambdaGenerateWithTwoParamsReturnsResults()
		{
			var value1 = 1;
			var value2 = 2;
			var actual = Lambda((int x, int y) => x + y, new[] { Constant(value1), Constant(value2) }).Generate(null);
			Assert.That(actual, Is.EqualTo(value1 + value2));
		}

		private static SqliteConnection _conn = new SqliteConnection("Data Source=:memory:");

		private static PacBillContext NewContext()
		{
			var ctx = new PacBillContext(new DbContextOptionsBuilder<PacBillContext>().
				UseSqlite(_conn).Options);
			ctx.Database.Migrate();

			return ctx;
		}

		[Test]
		public void SqlGeneratorReturnsQueryWithoutArgs()
		{
			using (_conn)
			{
				_conn.Open();
				var refund = new Refund
				{
					Amount = 10m,
				};
				using (var ctx = NewContext())
					ctx.SaveChanges(() => ctx.Add(refund));

				var query = @"
					select *
					from Refunds
				";

				var actual = Sql(NewContext().Database.GetDbConnection(), query).Generate(null);
				Assert.That(actual[0].Amount, Is.EqualTo(refund.Amount.ToString("0.0")));
			}
		}

		[Test]
		public void SqlGeneratorReturnsQueryWithArgs()
		{
			using (_conn)
			{
				_conn.Open();
				var refund = new Refund
				{
					Id = 3,
					Amount = 10m,
				};
				using (var ctx = NewContext())
					ctx.SaveChanges(() => ctx.Add(refund));

				var query = @"
					select *
					from Refunds
					where Id = @id
				";
				var args = Properties(new Dictionary<string, IGenerator> { { "id", Constant(3) } });
				var actual = Sql(NewContext().Database.GetDbConnection(), query, args).Generate(null);
				Assert.That(actual[0].Amount, Is.EqualTo(refund.Amount.ToString("0.0")));
			}
		}
	}
}
