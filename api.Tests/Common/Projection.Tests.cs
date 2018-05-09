using NUnit.Framework;

using static api.Common.Projection;

namespace api.Tests.Common
{
	[TestFixture]
	public class ProjectionTests
	{
		private class TestClass
		{
			public int A { get; set; }
			public int B { get; set; }
		}

		private class NestedClass
		{
			public int C { get; set; }
			public TestClass TestClass { get; set; }
		}

		private class NestedNestedClass
		{
			public int D { get; set; }
			public NestedClass NestedClass { get; set; }
		}

		[Test]
		public void ProjectReturnsFull()
		{
			var obj = new TestClass
			{
				A = 1,
				B = 2,
			};

			var result = Project(obj);
			Assert.That(result.GetType().GetField("A").GetValue(result), Is.EqualTo(obj.A));
			Assert.That(result.GetType().GetField("B").GetValue(result), Is.EqualTo(obj.B));
		}

		[Test]
		public void ProjectReturnsIncluded()
		{
			var obj = new TestClass
			{
				A = 1,
				B = 2,
			};

			var result = Project(obj, includes: new[] { nameof(TestClass.A) });
			Assert.That(result.GetType().GetField("A").GetValue(result), Is.EqualTo(obj.A));
			Assert.That(result.GetType().GetField("B"), Is.Null);
		}

		[Test]
		public void ProjectReturnsNotExcluded()
		{
			var obj = new TestClass
			{
				A = 1,
				B = 2,
			};

			var result = Project(obj, excludes: new[] { nameof(TestClass.A) });
			Assert.That(result.GetType().GetField("A"), Is.Null);
			Assert.That(result.GetType().GetField("B").GetValue(result), Is.EqualTo(obj.B));
		}

		[Test]
		public void ProjectReturnsNestedClassIncluded()
		{
			var obj = new NestedClass
			{
				TestClass = new TestClass
				{
					A = 1,
					B = 2,
				},
				C = 3,
			};

			var result = Project(obj, includes: new[] { "TestClass.A" });
			var nested = result.GetType().GetField("TestClass").GetValue(result);
			var type = nested.GetType();
			Assert.That(type.GetField("A").GetValue(nested), Is.EqualTo(obj.TestClass.A));
			Assert.That(type.GetField("B"), Is.Null);
			Assert.That(result.GetType().GetField("C"), Is.Null);
		}

		[Test]
		public void ProjectReturnsNestedClassNotExcluded()
		{
			var obj = new NestedClass
			{
				TestClass = new TestClass
				{
					A = 1,
					B = 2,
				},
				C = 3,
			};

			var result = Project(obj, excludes: new[] { "TestClass.A" });
			var nested = result.GetType().GetField("TestClass").GetValue(result);
			var type = nested.GetType();
			Assert.That(type.GetField("A"), Is.Null);
			Assert.That(type.GetField("B").GetValue(nested), Is.EqualTo(obj.TestClass.B));
			Assert.That(result.GetType().GetField("C").GetValue(result), Is.EqualTo(obj.C));
		}

		[Test]
		public void ProjectReturnsNestedNestedClassIncluded()
		{
			var obj = new NestedNestedClass
			{
				NestedClass = new NestedClass
				{
					TestClass = new TestClass
					{
						A = 1,
						B = 2,
					},
					C = 3,
				},
				D = 4,
			};

			var result = Project(obj, includes: new[] { "NestedClass.TestClass.A" });
			var nested = result.GetType().GetField("NestedClass").GetValue(result);
			var test = nested.GetType().GetField("TestClass").GetValue(nested);
			var type = test.GetType();
			Assert.That(type.GetField("A").GetValue(test), Is.EqualTo(obj.NestedClass.TestClass.A));
			Assert.That(type.GetField("B"), Is.Null);
			Assert.That(nested.GetType().GetField("C"), Is.Null);
			Assert.That(result.GetType().GetField("D"), Is.Null);
		}

		[Test]
		public void ProjectReturnsNestedNestedClassNotExcluded()
		{
			var obj = new NestedNestedClass
			{
				NestedClass = new NestedClass
				{
					TestClass = new TestClass
					{
						A = 1,
						B = 2,
					},
					C = 3,
				},
				D = 4,
			};

			var result = Project(obj, excludes: new[] { "NestedClass.TestClass.A" });
			var nested = result.GetType().GetField("NestedClass").GetValue(result);
			var test = nested.GetType().GetField("TestClass").GetValue(nested);
			var type = test.GetType();
			Assert.That(type.GetField("A"), Is.Null);
			Assert.That(type.GetField("B").GetValue(test), Is.EqualTo(obj.NestedClass.TestClass.B));
			Assert.That(nested.GetType().GetField("C").GetValue(nested), Is.EqualTo(obj.NestedClass.C));
			Assert.That(result.GetType().GetField("D").GetValue(result), Is.EqualTo(obj.D));
		}
	}
}
