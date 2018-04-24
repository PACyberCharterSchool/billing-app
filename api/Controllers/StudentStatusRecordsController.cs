using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;

using api.Models;

namespace api.Controllers
{
	[Route("api/[controller]")]
	public class StudentStatusRecordsController : Controller
	{
		private readonly IPendingStudentStatusRecordRepository _pending;
		private readonly ILogger<StudentStatusRecordsController> _logger;

		public StudentStatusRecordsController(
			IPendingStudentStatusRecordRepository pending,
			ILogger<StudentStatusRecordsController> logger)
		{
			_pending = pending;
			_logger = logger;
		}

		public struct StudentStatusRecordsResponse
		{
			public IList<StudentStatusRecord> StudentStatusRecords { get; set; }
		}

		public class GetManyArgs
		{
			[Range(0, int.MaxValue)]
			public int Skip { get; set; }

			[Range(0, int.MaxValue)]
			public int Take { get; set; }
		}

		[HttpGet("pending")]
		[Authorize(Policy = "STD+")]
		[ProducesResponseType(typeof(StudentStatusRecordsResponse), 200)]
		public async Task<IActionResult> GetManyPending([FromQuery]GetManyArgs args)
		{
			if (!ModelState.IsValid)
				return new BadRequestObjectResult(new ErrorsResponse(ModelState));

			var records = await Task.Run(() => _pending.GetMany(args.Skip, args.Take));
			if (records == null)
				records = new List<StudentStatusRecord>();

			return new ObjectResult(new StudentStatusRecordsResponse { StudentStatusRecords = records });
		}
	}
}
