using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

using Moq;
using NUnit.Framework;

using api.Controllers;
using models;

namespace api.Tests.Controllers
{
	[TestFixture]
	public class StudentActivityRecordsControllerTests
	{
		private Mock<IStudentActivityRecordRepository> _activities;

		private StudentActivityRecordsController _uut;

		[SetUp]
		public void SetUp()
		{
			_activities = new Mock<IStudentActivityRecordRepository>();

			_uut = new StudentActivityRecordsController(_activities.Object);
		}

		[Test]
		public async Task GetManyWithNoArgsReturnsRecords()
		{
			var records = new[]{
				new StudentActivityRecord(),
				new StudentActivityRecord(),
			};
			_activities.Setup(a => a.GetMany(null, null, 0, 0)).Returns(records);

			var result = await _uut.GetMany(new StudentActivityRecordsController.GetManyArgs());
			Assert.That(result, Is.TypeOf<ObjectResult>());

			var value = ((ObjectResult)result).Value;
			Assert.That(value, Is.TypeOf<StudentActivityRecordsController.StudentActivityRecordsResponse>());

			var actual = ((StudentActivityRecordsController.StudentActivityRecordsResponse)value).StudentActivityRecords;
			Assert.That(actual, Is.EqualTo(records));
		}

		[Test]
		public async Task GetManyWithAllArgsReturnsRecords()
		{
			var records = new[] {
				new StudentActivityRecord(),
				new StudentActivityRecord(),
			};

			var activity = StudentActivity.NewStudent.Value;
			var skip = 1;
			var take = 1;
			_activities.Setup(a => a.GetMany(null, activity, skip, take)).Returns(records);

			var result = await _uut.GetMany(new StudentActivityRecordsController.GetManyArgs
			{
				Activity = activity,
				Skip = skip,
				Take = take,
			});
			Assert.That(result, Is.TypeOf<ObjectResult>());

			var value = ((ObjectResult)result).Value;
			Assert.That(value, Is.TypeOf<StudentActivityRecordsController.StudentActivityRecordsResponse>());

			var actual = ((StudentActivityRecordsController.StudentActivityRecordsResponse)value).StudentActivityRecords;
			Assert.That(actual, Is.EqualTo(records));
		}

		[Test]
		public async Task GetManyReturnsEmptyListWhenNull()
		{
			_activities.Setup(a => a.GetMany(null, null, 0, 0)).Returns((List<StudentActivityRecord>)null);

			var result = await _uut.GetMany(new StudentActivityRecordsController.GetManyArgs());
			Assert.That(result, Is.TypeOf<ObjectResult>());

			var value = ((ObjectResult)result).Value;
			Assert.That(value, Is.TypeOf<StudentActivityRecordsController.StudentActivityRecordsResponse>());

			var actual = ((StudentActivityRecordsController.StudentActivityRecordsResponse)value).StudentActivityRecords;
			Assert.That(actual, Is.Empty);
		}

		[Test]
		public async Task GetManyReturnsBadRequestWhenModelStateIsInvalid()
		{
			var key = "error key";
			var msg = "error msg";
			_uut.ModelState.AddModelError(key, msg);

			var result = await _uut.GetMany(new StudentActivityRecordsController.GetManyArgs());
			Assert.That(result, Is.TypeOf<BadRequestObjectResult>());
			var value = ((BadRequestObjectResult)result).Value;

			Assert.That(value, Is.TypeOf<ErrorsResponse>());
			var errors = ((ErrorsResponse)value).Errors;
			Assert.That(errors, Has.Count.EqualTo(1));
			Assert.That(errors[0], Is.EqualTo(msg));
		}

		[Test]
		public async Task GetManyByIdWithNoArgsReturnsRecords()
		{
			var id = "3";
			var records = new[]{
				new StudentActivityRecord(),
				new StudentActivityRecord(),
			};
			_activities.Setup(a => a.GetMany(id, null, 0, 0)).Returns(records);

			var result = await _uut.GetMany(id, new StudentActivityRecordsController.GetManyArgs());
			Assert.That(result, Is.TypeOf<ObjectResult>());

			var value = ((ObjectResult)result).Value;
			Assert.That(value, Is.TypeOf<StudentActivityRecordsController.StudentActivityRecordsResponse>());

			var actual = ((StudentActivityRecordsController.StudentActivityRecordsResponse)value).StudentActivityRecords;
			Assert.That(actual, Is.EqualTo(records));
		}

		[Test]
		public async Task GetManyByIdWithAllArgsReturnsRecords()
		{
			var id = "3";
			var records = new[] {
				new StudentActivityRecord(),
				new StudentActivityRecord(),
			};

			var activity = StudentActivity.NewStudent.Value;
			var skip = 1;
			var take = 1;
			_activities.Setup(a => a.GetMany(id, activity, skip, take)).Returns(records);

			var result = await _uut.GetMany(id, new StudentActivityRecordsController.GetManyArgs
			{
				Activity = activity,
				Skip = skip,
				Take = take,
			});
			Assert.That(result, Is.TypeOf<ObjectResult>());

			var value = ((ObjectResult)result).Value;
			Assert.That(value, Is.TypeOf<StudentActivityRecordsController.StudentActivityRecordsResponse>());

			var actual = ((StudentActivityRecordsController.StudentActivityRecordsResponse)value).StudentActivityRecords;
			Assert.That(actual, Is.EqualTo(records));
		}

		[Test]
		public async Task GetManyByIdReturnsEmptyListWhenNull()
		{
			var id = "3";
			_activities.Setup(a => a.GetMany(id, null, 0, 0)).Returns((List<StudentActivityRecord>)null);

			var result = await _uut.GetMany(new StudentActivityRecordsController.GetManyArgs());
			Assert.That(result, Is.TypeOf<ObjectResult>());

			var value = ((ObjectResult)result).Value;
			Assert.That(value, Is.TypeOf<StudentActivityRecordsController.StudentActivityRecordsResponse>());

			var actual = ((StudentActivityRecordsController.StudentActivityRecordsResponse)value).StudentActivityRecords;
			Assert.That(actual, Is.Empty);
		}

		[Test]
		public async Task GetManyByIdReturnsBadRequestWhenModelStateIsInvalid()
		{
			var id = "3";
			var key = "error key";
			var msg = "error msg";
			_uut.ModelState.AddModelError(key, msg);

			var result = await _uut.GetMany(id, new StudentActivityRecordsController.GetManyArgs());
			Assert.That(result, Is.TypeOf<BadRequestObjectResult>());
			var value = ((BadRequestObjectResult)result).Value;

			Assert.That(value, Is.TypeOf<ErrorsResponse>());
			var errors = ((ErrorsResponse)value).Errors;
			Assert.That(errors, Has.Count.EqualTo(1));
			Assert.That(errors[0], Is.EqualTo(msg));
		}
	}
}
