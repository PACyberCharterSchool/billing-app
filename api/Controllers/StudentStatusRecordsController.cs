using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;

using models;

namespace api.Controllers
{
	[Route("api/[controller]")]
	public class StudentStatusRecordsController : Controller
	{
		private readonly IPendingStudentStatusRecordRepository _pending;
		private readonly ICommittedStudentStatusRecordRepository _committed;
		private readonly ILogger<StudentStatusRecordsController> _logger;

		public StudentStatusRecordsController(
			IPendingStudentStatusRecordRepository pending,
			ICommittedStudentStatusRecordRepository committed,
			ILogger<StudentStatusRecordsController> logger)
		{
			_pending = pending;
			_committed = committed;
			_logger = logger;
		}

		public struct PendingStudentStatusRecordsResponse
		{
			public IList<PendingStudentStatusRecord> StudentStatusRecords { get; set; }
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
		[ProducesResponseType(typeof(PendingStudentStatusRecordsResponse), 200)]
		public async Task<IActionResult> GetManyPending([FromQuery]GetManyArgs args)
		{
			if (!ModelState.IsValid)
				return new BadRequestObjectResult(new ErrorsResponse(ModelState));

			var records = await Task.Run(() => _pending.GetMany(args.Skip, args.Take));
			if (records == null)
				records = new List<PendingStudentStatusRecord>();

			return new ObjectResult(new PendingStudentStatusRecordsResponse { StudentStatusRecords = records });
		}

		[HttpPost("pending/commit")]
		[Authorize(Policy = "PAY+")]
		public async Task<IActionResult> Commit()
		{
			var pending = _pending.GetMany();
			if (pending == null || pending.Count == 0)
				return Ok();

			var committed = new List<CommittedStudentStatusRecord>();
			foreach (var p in pending)
				committed.Add(new CommittedStudentStatusRecord
				{
					SchoolDistrictId = p.SchoolDistrictId,
					SchoolDistrictName = p.SchoolDistrictName,
					StudentId = p.StudentId,
					StudentFirstName = p.StudentFirstName,
					StudentMiddleInitial = p.StudentMiddleInitial,
					StudentLastName = p.StudentLastName,
					StudentGradeLevel = p.StudentGradeLevel,
					StudentDateOfBirth = p.StudentDateOfBirth,
					StudentStreet1 = p.StudentStreet1,
					StudentStreet2 = p.StudentStreet2,
					StudentCity = p.StudentCity,
					StudentState = p.StudentState,
					StudentZipCode = p.StudentZipCode,
					ActivitySchoolYear = p.ActivitySchoolYear,
					StudentEnrollmentDate = p.StudentEnrollmentDate,
					StudentWithdrawalDate = p.StudentWithdrawalDate,
					StudentIsSpecialEducation = p.StudentIsSpecialEducation,
					StudentCurrentIep = p.StudentCurrentIep,
					StudentFormerIep = p.StudentFormerIep,
					StudentNorep = p.StudentNorep,
					StudentPaSecuredId = p.StudentPaSecuredId,
					BatchTime = p.BatchTime,
					BatchFilename = p.BatchFilename,
					BatchHash = p.BatchHash,
				});

			await Task.Run(() => _committed.CreateMany(committed));
			await Task.Run(() => _pending.Truncate());

			return Ok();
		}
	}
}
