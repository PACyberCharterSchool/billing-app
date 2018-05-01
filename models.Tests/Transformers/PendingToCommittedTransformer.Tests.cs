using System.Collections.Generic;
using System.Linq;

using Moq;
using NUnit.Framework;

using models.Transformers;

namespace models.Tests.Transformers
{
	[TestFixture]
	public class PendingToCommittedTransformerTests
	{
		private Mock<ICommittedStudentStatusRecordRepository> _committed;

		private PendingToCommittedTransformer _uut;

		[SetUp]
		public void SetUp()
		{
			_committed = new Mock<ICommittedStudentStatusRecordRepository>();

			_uut = new PendingToCommittedTransformer(_committed.Object);
		}

		[Test]
		public void TransformTransforms()
		{
			var studentId = 3;
			var pending = new PendingStudentStatusRecord
			{
				StudentId = studentId,
			};

			var actual = (_uut.Transform(new List<PendingStudentStatusRecord> { pending }) as
				IEnumerable<CommittedStudentStatusRecord>).ToList();
			Assert.That(actual, Has.Count.EqualTo(1));
			Assert.That(actual[0].StudentId, Is.EqualTo(studentId));

			_committed.Verify(c => c.Create(It.Is<CommittedStudentStatusRecord>(r => r.StudentId == studentId)), Times.Once);
		}
	}
}
