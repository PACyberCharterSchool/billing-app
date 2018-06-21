using System;

using NUnit.Framework;

using models;

namespace models.Tests
{
	public class NestedClass
	{
		public int A { get; set; }
		public int Z { get; set; }
	}

	public class TestClass
	{
		public int A { get; set; }
		public int B { get; set; }
		public int C { get; set; }
		public string D { get; set; }
		public int? E { get; set; }
		public NestedClass F { get; set; }
	}

	[TestFixture]
	public class FilterParserTests
	{
		[Test]
		[TestCase("(a eq 1)", "x => (x.A == 1)")]
		[TestCase("(a ne 1)", "x => (x.A != 1)")]
		[TestCase("(a gt 1)", "x => (x.A > 1)")]
		[TestCase("(a ge 1)", "x => (x.A >= 1)")]
		[TestCase("(a lt 1)", "x => (x.A < 1)")]
		[TestCase("(a le 1)", "x => (x.A <= 1)")]
		/* [TestCase("(d has z)", "x => x.D.Contains(\"z\")")] */
		[TestCase("(d bgn z)", "x => x.D.StartsWith(\"z\")")]
		[TestCase("(d end z)", "x => x.D.EndsWith(\"z\")")]
		[TestCase("((a eq 1) and (b eq 2))", "x => ((x.A == 1) AndAlso (x.B == 2))")]
		[TestCase("((a eq 1) or (b eq 2))", "x => ((x.A == 1) OrElse (x.B == 2))")]
		[TestCase("(((a eq 1) and (b eq 2)) or (c eq 3))", "x => (((x.A == 1) AndAlso (x.B == 2)) OrElse (x.C == 3))")]
		[TestCase("(e eq 1)", "x => (x.E == 1)")] // nullables
		[TestCase("(f.a eq 1)", "x => (x.F.A == 1)")]
		[TestCase("(f.z eq 1)", "x => (x.F.Z == 1)")]
		public void ParseProducesLambda(string clause, string lambda)
		{
			var actual = new FilterParser().Parse<TestClass>(clause);

			Assert.That(actual.ToString(), Is.EqualTo(lambda));
		}

		[Test]
		[TestCase("(a eq 1", "Filter was not properly closed.")]
		[TestCase("a eq 1)", "Clause must begin with '('; found 'a' at [0].")]
		[TestCase("(a eq 1 ;", "Clause must end with ')'; found ';' at [8].")]
		[TestCase("(a bob 1)", "Invalid boolean operation 'bob' ending at [5].")]
		[TestCase("((a eq 1) bob (b eq 2))", "Invalid compound operation 'bob' ending at [12].")]
		public void ParseThrowsArgumentException(string clause, string message)
		{
			Assert.That(() => { new FilterParser().Parse<TestClass>(clause); },
				Throws.TypeOf<ArgumentException>().With.
				Message.EqualTo(message));
		}
	}
}
