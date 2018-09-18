using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Threading.Tasks;
using System.IO;
using System.IO.Compression;
using System.Data;
using System.Linq;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.Extensions.Logging;

using Aspose.Cells;

using api.Common;

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

			public string ExcelFileName { get; set; }
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

		DataTable StudentRecordListToDataTable(IEnumerable<StudentRecord> students)
		{
			PropertyDescriptorCollection props = TypeDescriptor.GetProperties(typeof(StudentRecord));
			DataTable dt = new DataTable();

			for (int i = 0; i < props.Count; i++)
			{
				PropertyDescriptor pd = props[i];
				dt.Columns.Add(pd.Name, Nullable.GetUnderlyingType(pd.PropertyType) ?? pd.PropertyType);
			}

			object[] vals = new object[props.Count];
			foreach (StudentRecord sr in students)
			{
				for (int i = 0; i < vals.Length; i++)
				{
					vals[i] = props[i].GetValue(sr);
				}

				dt.Rows.Add(vals);
			}

			return dt;
		}	

		byte[] GenerateExcelData(DataTable dt)
		{
			byte[] result = null;

			var wb = new Workbook();
			var sheet = wb.Worksheets[0];

			var style = wb.CreateStyle();
			style.Number = 14;
			var styleFlag = new StyleFlag();
			styleFlag.NumberFormat = true;

			for (int i = 0; i < dt.Columns.Count; i++)
			{
				sheet.Cells[0, i].PutValue(dt.Columns[i]);

				if (new[] { "StudentDateOfBirth", "StudentEnrollmentDate", "StudentWithdrawalDate", "StudentCurrentIep", "StudentFormerIep", "StudentNorep" }.Contains(dt.Columns[i].ToString()))
					sheet.Cells.Columns[i].ApplyStyle(style, styleFlag);
			}

			var cells = sheet.Cells;
			for (int i = 1; i < dt.Rows.Count; i++)
			{
				var row = dt.Rows[i];
				var c = 0;
				cells[i, c++].PutValue(dt.Rows[i]["StudentDateOfBirth"]);
				cells[i, c++].PutValue(dt.Rows[i]["SchoolDistrictInformation"]);
				cells[i, c++].PutValue(dt.Rows[i]["StudentId"]);
				cells[i, c++].PutValue(dt.Rows[i]["StudentFirstName"]);
				cells[i, c++].PutValue(dt.Rows[i]["StudentMiddleInitial"]);
				cells[i, c++].PutValue(dt.Rows[i]["StudentLastName"]);
				cells[i, c++].PutValue(dt.Rows[i]["StudentGradeLevel"]);
				cells[i, c++].PutValue(dt.Rows[i]["StudentDateOfBirth"]);
				cells[i, c++].PutValue(dt.Rows[i]["StudentStreet1"]);
				cells[i, c++].PutValue(dt.Rows[i]["StudentStreet2"]);
				cells[i, c++].PutValue(dt.Rows[i]["StudentCity"]);
				cells[i, c++].PutValue(dt.Rows[i]["StudentState"]);
				cells[i, c++].PutValue(dt.Rows[i]["StudentZipCode"]);
				cells[i, c++].PutValue(dt.Rows[i]["ActivitySchoolYear"]);
				cells[i, c++].PutValue(dt.Rows[i]["StudentEnrollmentDate"]);
				cells[i, c++].PutValue(dt.Rows[i]["StudentWithdrawalDate"]);
				cells[i, c++].PutValue(dt.Rows[i]["StudentNorep"]);
				cells[i, c++].PutValue(dt.Rows[i]["LastUpdated"]);
			}

			using (var xlsxStream = new MemoryStream())
			{
				wb.Save(xlsxStream, new XlsSaveOptions(SaveFormat.Xlsx));
				return xlsxStream.ToArray();
			}
		}

		[HttpGet("header/{scope}/excel")]
		[Authorize(Policy = "STD+")]
		[Produces(ContentTypes.XLSX)]
		[ProducesResponseType(404)]
		[ProducesResponseType(406)]
		public async Task<IActionResult> GetData(string scope, [FromQuery]GetHeaderArgs args)
		{
			var header = await Task.Run(() => _records.Get(
				scope: scope,
				filter: args.Filter,
				skip: args.Skip,
				take: args.Take
			));
			if (header == null)
				return NotFound();

			var accept = Request.Headers["Accept"];

			var data = GenerateExcelData(StudentRecordListToDataTable(header.Records));
			MemoryStream stream = new MemoryStream(data);

			if (stream == null)
				return StatusCode(406);

			return new FileStreamResult(stream, accept) { FileDownloadName = args.ExcelFileName };	
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
