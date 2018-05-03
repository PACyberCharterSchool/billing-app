using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;

using Swashbuckle.AspNetCore.SwaggerGen;

using models;

namespace api.Controllers
{
	[Route("api/[controller]")]
	public class StudentActivityRecordsController : Controller
	{
		private readonly IStudentActivityRecordRepository _activities;

		public StudentActivityRecordsController(IStudentActivityRecordRepository activities)
		{
			_activities = activities;
		}

		public class GetManyArgs
		{
			[EnumerableValidation(typeof(StudentActivity))]
			public string Activity { get; set; }

			[Range(0, int.MaxValue)]
			public int Skip { get; set; }

			[Range(0, int.MaxValue)]
			public int Take { get; set; }
		}

		public struct StudentActivityRecordsResponse
		{
			public IList<StudentActivityRecord> StudentActivityRecords { get; set; }
		}

		[HttpGet()]
		[Authorize(Policy = "STD+")]
		[ProducesResponseType(typeof(StudentActivityRecordsResponse), 200)]
		[ProducesResponseType(typeof(ErrorsResponse), 400)]
		public async Task<IActionResult> GetMany([FromQuery]GetManyArgs args)
		{
			if (!ModelState.IsValid)
				return new BadRequestObjectResult(new ErrorsResponse(ModelState));

			var activities = await Task.Run(() => _activities.GetMany(
				activity: args.Activity,
				skip: args.Skip,
				take: args.Take
			));
			if (activities == null)
				activities = new List<StudentActivityRecord>();

			return new ObjectResult(new StudentActivityRecordsResponse { StudentActivityRecords = activities });
		}

		[HttpGet("{id}")]
		[Authorize(Policy = "STD+")]
		[ProducesResponseType(typeof(StudentActivityRecordsResponse), 200)]
		[ProducesResponseType(typeof(ErrorsResponse), 400)]
		public async Task<IActionResult> GetMany(string id, [FromQuery]GetManyArgs args)
		{
			if (!ModelState.IsValid)
				return new BadRequestObjectResult(new ErrorsResponse(ModelState));

			var activities = await Task.Run(() => _activities.GetMany(
				id: id,
				activity: args.Activity,
				skip: args.Skip,
				take: args.Take
			));
			if (activities == null)
				activities = new List<StudentActivityRecord>();

			return new ObjectResult(new StudentActivityRecordsResponse { StudentActivityRecords = activities });
		}
	}
}
