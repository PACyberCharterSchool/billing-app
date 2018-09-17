using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.Extensions.Logging;
using models;
using static models.Common.PropertyMerger;

namespace api.Controllers
{
	[Route("/api/[Controller]")]
	public class StudentRecordsController : Controller
	{
		private readonly PacBillContext _context;
		private readonly IStudentRecordRepository _records;
		private readonly IAuditRecordRepository _audits;
		private readonly ILogger<StudentRecordsController> _logger;

		public StudentRecordsController(
			PacBillContext context,
			 IStudentRecordRepository records,
			 IAuditRecordRepository audits,
			 ILogger<StudentRecordsController> logger)
		{
			_context = context;
			_records = records;
			_audits = audits;
			_logger = logger;
		}

		public class GetScopesArgs
		{
			public bool? Locked { get; set; }
		}

		public struct ScopesResponse
		{
			public IList<string> Scopes { get; set; }
		}

		[HttpGet("scopes")]
		[Authorize(Policy = "STD+")]
		[ProducesResponseType(200)]
		public async Task<IActionResult> GetScopes([FromQuery]GetScopesArgs args)
		{
			var scopes = await Task.Run(() => _records.GetScopes(
				locked: args.Locked
			));
			return new ObjectResult(new ScopesResponse
			{
				Scopes = scopes,
			});
		}

		public class GetHeaderArgs
		{
			public string Filter { get; set; }

			[Range(0, int.MaxValue)]
			public int Skip { get; set; }

			[Range(0, int.MaxValue)]
			public int Take { get; set; }
		}

		public struct StudentRecordsHeaderResponse
		{
			public StudentRecordsHeader Header { get; set; }
		}

		[HttpGet("header/{scope}")]
		[Authorize(Policy = "STD+")]
		[ProducesResponseType(typeof(StudentRecordsHeaderResponse), 200)]
		[ProducesResponseType(404)]
		public async Task<IActionResult> GetHeader(string scope, [FromQuery]GetHeaderArgs args)
		{
			var header = await Task.Run(() => _records.Get(
				scope: scope,
				filter: args.Filter,
				skip: args.Skip,
				take: args.Take
			));
			if (header == null)
				return NotFound();

			return new ObjectResult(new StudentRecordsHeaderResponse
			{
				Header = header,
			});
		}

		[HttpPost("header/{scope}/lock")]
		[Authorize(Policy = "PAY+")]
		[ProducesResponseType(200)]
		[ProducesResponseType(404)]
		public async Task<IActionResult> LockHeader(string scope)
		{
			await Task.Run(() => _context.SaveChanges(() => _records.Lock(scope)));
			return Ok();
		}

		public class StudentRecordUpdate
		{
			[Required]
			[MinLength(1)]
			public string StudentFirstName { get; set; }

			[Required]
			[MinLength(1)]
			public string StudentMiddleInitial { get; set; }

			[Required]
			[MinLength(1)]
			public string StudentLastName { get; set; }

			[Required]
			[MinLength(1)]
			public string StudentGradeLevel { get; set; }

			[Required]
			public DateTime StudentDateOfBirth { get; set; }

			[Required]
			[MinLength(1)]
			public string StudentStreet1 { get; set; }

			[MinLength(0)]
			public string StudentStreet2 { get; set; }

			[Required]
			[MinLength(1)]
			public string StudentCity { get; set; }

			[Required]
			[MinLength(1)]
			public string StudentState { get; set; }

			[Required]
			[MinLength(1)]
			public string StudentZipCode { get; set; }

			[Required]
			public DateTime StudentEnrollmentDate { get; set; }

			public DateTime? StudentWithdrawalDate { get; set; }

			[BindRequired]
			public bool StudentIsSpecialEducation { get; set; }

			public DateTime? StudentCurrentIep { get; set; }

			public DateTime? StudentFormerIep { get; set; }

			public DateTime? StudentNorep { get; set; }
		}

		[HttpPut("{scope}/{id}")]
		[Authorize(Policy = "STD+")]
		[ProducesResponseType(200)]
		[ProducesResponseType(typeof(ErrorResponse), 400)]
		[ProducesResponseType(422)]
		public async Task<IActionResult> Update(string scope, int id, [FromBody]StudentRecordUpdate update)
		{
			if (!ModelState.IsValid)
				return new BadRequestObjectResult(new ErrorsResponse(ModelState));

			if (!_records.IsLocked(scope))
				return StatusCode(422);

			var record = new StudentRecord
			{
				Id = id,
				StudentFirstName = update.StudentFirstName,
				StudentMiddleInitial = update.StudentMiddleInitial,
				StudentLastName = update.StudentLastName,
				StudentGradeLevel = update.StudentGradeLevel,
				StudentDateOfBirth = update.StudentDateOfBirth,
				StudentStreet1 = update.StudentStreet1,
				StudentStreet2 = update.StudentStreet2,
				StudentCity = update.StudentCity,
				StudentState = update.StudentState,
				StudentZipCode = update.StudentZipCode,
				StudentEnrollmentDate = update.StudentEnrollmentDate,
				StudentWithdrawalDate = update.StudentWithdrawalDate,
				StudentIsSpecialEducation = update.StudentIsSpecialEducation,
				StudentCurrentIep = update.StudentCurrentIep,
				StudentFormerIep = update.StudentFormerIep,
				StudentNorep = update.StudentNorep,
			};

			var current = _records.Get(record.Id);
			var delta = MergeProperties(current, record, new[] {
				 nameof(StudentRecord.Id),
				 nameof(StudentRecord.StudentId),
				 nameof(StudentRecord.SchoolDistrictId),
				 nameof(StudentRecord.SchoolDistrictName),
				 nameof(StudentRecord.Header),
				 nameof(StudentRecord.LastUpdated),
				 nameof(StudentRecord.ActivitySchoolYear),
				 nameof(StudentRecord.StudentPaSecuredId),
			});

			var username = User.FindFirst(c => c.Type == JwtRegisteredClaimNames.Sub).Value;
			using (var tx = _context.Database.BeginTransaction())
			{
				try
				{
					_records.Update(current);
					_audits.CreateMany(delta.Select(d => new AuditRecord
					{
						Username = username,
						Activity = AuditRecordActivity.EDIT_STUDENT_RECORD,
						Timestamp = DateTime.Now,
						Identifier = current.StudentId,
						Field = d.Key,
						Next = d.Value.Next,
						Previous = d.Value.Previous,
					}).ToList());
					_context.SaveChanges();

					tx.Commit();
				}
				catch (Exception)
				{
					tx.Rollback();
					throw;
				}
			}

			return Ok();
		}
	}
}
