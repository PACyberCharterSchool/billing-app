using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IdentityModel.Tokens.Jwt;
using System.Threading.Tasks;

using models;

namespace api.Controllers
{
	[Route("api/[controller]")]
	public class StudentStatusRecordsController : Controller
	{
		private readonly PacBillContext _context;
		private readonly IPendingStudentStatusRecordRepository _pending;
		private readonly ICommittedStudentStatusRecordRepository _committed;
		private readonly IAuditRecordRepository _audits;
		private readonly ILogger<StudentStatusRecordsController> _logger;

		public StudentStatusRecordsController(
			PacBillContext context,
			IPendingStudentStatusRecordRepository pending,
			ICommittedStudentStatusRecordRepository committed,
			IAuditRecordRepository audits,
			ILogger<StudentStatusRecordsController> logger)
		{
			_context = context;
			_pending = pending;
			_committed = committed;
			_audits = audits;
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
			var pending = await Task.Run(() => _pending.GetMany());
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

			using (var tx = _context.Database.BeginTransaction())
			{
				_committed.CreateMany(committed);
				_pending.Truncate();

				var username = User.FindFirst(c => c.Type == JwtRegisteredClaimNames.Sub).Value;
				_audits.Create(new AuditRecord
				{
					Username = username,
					Activity = AuditRecordActivity.COMMIT_GENIUS,
				});

				tx.Commit();
			}

			return Ok();
		}
	}
}
