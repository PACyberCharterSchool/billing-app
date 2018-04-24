using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Threading.Tasks;

using Moq;
using NUnit.Framework;

using api.Controllers;
using api.Tests.Util;
using models;

namespace api.Tests.Controllers
{
	[TestFixture]
	public class StudentStatusRecordsControllerTests
	{
		private Mock<IPendingStudentStatusRecordRepository> _pending;
		private Mock<ICommittedStudentStatusRecordRepository> _committed;
		private ILogger<StudentStatusRecordsController> _logger;

		private StudentStatusRecordsController _uut;

		[SetUp]
		public void SetUp()
		{
			_pending = new Mock<IPendingStudentStatusRecordRepository>();
			_committed = new Mock<ICommittedStudentStatusRecordRepository>();
			_logger = new TestLogger<StudentStatusRecordsController>();

			_uut = new StudentStatusRecordsController(_pending.Object, _committed.Object, _logger);
		}

		[Test]
		public async Task GetManyNoArgsReturnsList()
		{
			var records = new[] {
				new PendingStudentStatusRecord{Id = 1},
				new PendingStudentStatusRecord{Id = 2},
				new PendingStudentStatusRecord{Id = 3},
			};
			_pending.Setup(r => r.GetMany(0, 0)).Returns(records);

			var result = await _uut.GetManyPending(new StudentStatusRecordsController.GetManyArgs());
			Assert.That(result, Is.TypeOf<ObjectResult>());

			var value = ((ObjectResult)result).Value;
			Assert.That(value, Is.TypeOf<StudentStatusRecordsController.PendingStudentStatusRecordsResponse>());

			var actual = ((StudentStatusRecordsController.PendingStudentStatusRecordsResponse)value).StudentStatusRecords;
			Assert.That(actual, Is.EqualTo(records));
		}

		[Test]
		public async Task GetManyWithArgsReturnsList()
		{
			var skip = 1;
			var take = 1;
			var records = new[] {
				new PendingStudentStatusRecord{Id = 1},
				new PendingStudentStatusRecord{Id = 2},
				new PendingStudentStatusRecord{Id = 3},
			};
			_pending.Setup(r => r.GetMany(skip, take)).Returns(records);

			var result = await _uut.GetManyPending(new StudentStatusRecordsController.GetManyArgs
			{
				Skip = skip,
				Take = take,
			});
			Assert.That(result, Is.TypeOf<ObjectResult>());

			var value = ((ObjectResult)result).Value;
			Assert.That(value, Is.TypeOf<StudentStatusRecordsController.PendingStudentStatusRecordsResponse>());

			var actual = ((StudentStatusRecordsController.PendingStudentStatusRecordsResponse)value).StudentStatusRecords;
			Assert.That(actual, Is.EqualTo(records));
		}

		[Test]
		public async Task GetManyReturnsEmptyListWhenEmpty()
		{
			_pending.Setup(r => r.GetMany(0, 0)).Returns(new List<PendingStudentStatusRecord>());

			var result = await _uut.GetManyPending(new StudentStatusRecordsController.GetManyArgs());
			Assert.That(result, Is.TypeOf<ObjectResult>());

			var value = ((ObjectResult)result).Value;
			Assert.That(value, Is.TypeOf<StudentStatusRecordsController.PendingStudentStatusRecordsResponse>());

			var actual = ((StudentStatusRecordsController.PendingStudentStatusRecordsResponse)value).StudentStatusRecords;
			Assert.That(actual, Has.Count.EqualTo(0));
		}

		[Test]
		public async Task GetManyReturnsEmptyListWhenNull()
		{
			_pending.Setup(r => r.GetMany(0, 0)).Returns((IList<PendingStudentStatusRecord>)null);

			var result = await _uut.GetManyPending(new StudentStatusRecordsController.GetManyArgs());
			Assert.That(result, Is.TypeOf<ObjectResult>());

			var value = ((ObjectResult)result).Value;
			Assert.That(value, Is.TypeOf<StudentStatusRecordsController.PendingStudentStatusRecordsResponse>());

			var actual = ((StudentStatusRecordsController.PendingStudentStatusRecordsResponse)value).StudentStatusRecords;
			Assert.That(actual, Has.Count.EqualTo(0));
		}

		[Test]
		public async Task GetManyBadRequestWhenModelStateInvalid()
		{
			var key = "err";
			var msg = "msg";
			_uut.ModelState.AddModelError(key, msg);

			var result = await _uut.GetManyPending(new StudentStatusRecordsController.GetManyArgs());
			Assert.That(result, Is.TypeOf<BadRequestObjectResult>());

			var value = ((BadRequestObjectResult)result).Value;
			Assert.That(value, Is.TypeOf<ErrorsResponse>());

			var actual = ((ErrorsResponse)value).Errors;
			Assert.That(actual, Has.Count.EqualTo(1));
			Assert.That(actual[0], Is.EqualTo(msg));
		}

		[Test]
		public async Task CommitCommits()
		{
			var pending = new[] {
				new PendingStudentStatusRecord{Id = 1},
				new PendingStudentStatusRecord{Id = 2},
				new PendingStudentStatusRecord{Id = 3},
			};
			_pending.Setup(p => p.GetMany()).Returns(pending);

			var result = await _uut.Commit();
			Assert.That(result, Is.TypeOf<OkResult>());

			_committed.Verify(c => c.CreateMany(It.IsAny<List<CommittedStudentStatusRecord>>()), Times.Once);
			_pending.Verify(c => c.Truncate(), Times.Once);
		}

		[Test]
		public async Task CommitOkWhenNoPending()
		{
			_pending.Setup(p => p.GetMany()).Returns(new List<PendingStudentStatusRecord>());

			var result = await _uut.Commit();
			Assert.That(result, Is.TypeOf<OkResult>());
		}

		[Test]
		public async Task CommitOkWhenPendingNull()
		{
			_pending.Setup(p => p.GetMany()).Returns((List<PendingStudentStatusRecord>)null);

			var result = await _uut.Commit();
			Assert.That(result, Is.TypeOf<OkResult>());
		}
	}
}
