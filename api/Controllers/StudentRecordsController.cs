using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.Extensions.Logging;
using models;

namespace api.Controllers
{
	[Route("/api/[Controller]")]
	public class StudentRecordsController : Controller
	{
		private readonly PacBillContext _context;
		private readonly IStudentRecordRepository _records;
		private readonly ILogger<StudentRecordsController> _logger;

		public StudentRecordsController(
			PacBillContext context,
			 IStudentRecordRepository records,
			 ILogger<StudentRecordsController> logger)
		{
			_context = context;
			_records = records;
			_logger = logger;
		}

		public class GetHeaderArgs
		{
			[Range(0, int.MaxValue)]
			public int Skip { get; set; }

			[Range(0, int.MaxValue)]
			public int Take { get; set; }
		}

		public struct ScopesResponse
		{
			public IList<string> Scopes { get; set; }
		}

		[HttpGet("scopes")]
		[Authorize(Policy = "STD+")]
		[ProducesResponseType(200)]
		public async Task<IActionResult> GetScopes()
		{
			var scopes = await Task.Run(() => _records.GetScopes());

			return new ObjectResult(new ScopesResponse
			{
				Scopes = scopes,
			});
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
		[ProducesResponseType(423)]
		public async Task<IActionResult> Update(string scope, int id, [FromBody]StudentRecordUpdate update)
		{
			if (!ModelState.IsValid)
				return new BadRequestObjectResult(new ErrorsResponse(ModelState));

			if (_records.IsLocked(scope))
				return StatusCode(423);

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
			await Task.Run(() => _context.SaveChanges(() => _records.Update(record)));
			return Ok();
		}
	}
}
