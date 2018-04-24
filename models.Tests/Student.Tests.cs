using NUnit.Framework;

using models;

namespace models.Tests
{
	[TestFixture]
	public class StudentTests
	{
		[Test]
		public void IsValidFieldReturnFalseOnBadField()
		{
			Assert.False(Student.IsValidField("bad"));
		}

		[Test]
		public void IsValidFieldReturnTrueOnGoodField()
		{
			Assert.True(Student.IsValidField("FirstName"));
		}

		[Test]
		public void IsValidFieldReturnTrueOnGoodLowercaseField()
		{
			Assert.That(Student.IsValidField("FiRsTnAmE"));
		}
	}
}
