using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.IdentityModel.Tokens.Jwt;

using CsvHelper;
using Aspose.Cells;

using api.Common;
using models;

namespace api.Controllers
{
	[Route("/api/[controller]")]
	public class CalendarsController : Controller
	{
		private readonly PacBillContext _context;
		private readonly ICalendarRepository _calendars;
		private readonly IAuditRecordRepository _audits;
		private readonly ILogger<CalendarsController> _logger;

		public CalendarsController(
			PacBillContext context,
			ICalendarRepository calendars,
			IAuditRecordRepository audits,
			ILogger<CalendarsController> logger)
		{
			_context = context;
			_calendars = calendars;
			_audits = audits;
			_logger = logger;
		}

		public struct CalendarResponse
		{
			public Calendar Calendar { get; set; }
		}

		public struct CalendarYearsResponse
		{
			public IList<string> Years { get; set; }
		}

		[HttpGet("years")]
		[Authorize(Policy = "ADM=")]
		[ProducesResponseType(typeof(CalendarYearsResponse), 200)]
		[ProducesResponseType(404)]
		public async Task<IActionResult> GetYears()
		{
			try
			{
				var years = await Task.Run(() => _calendars.GetYears());

				return new ObjectResult(new CalendarYearsResponse
				{
					Years = years.ToList()
				});
			}
			catch (NotFoundException)
			{
				return NotFound();
			}
		}

		[HttpGet("{year}")]
		[Authorize(Policy = "ADM=")]
		[ProducesResponseType(typeof(CalendarResponse), 200)]
		[ProducesResponseType(404)]
		public async Task<IActionResult> Get(string year)
		{
			try
			{
				var calendar = await Task.Run(() => _calendars.Get(year));
				return new ObjectResult(new CalendarResponse
				{
					Calendar = calendar,
				});
			}
			catch (NotFoundException)
			{
				return NotFound();
			}
		}

		private static Calendar CsvToCalendar(string year, Stream stream)
		{
			var calendar = new Calendar
			{
				SchoolYear = year,
				Days = new List<CalendarDay>(),
			};
			using (var reader = new CsvReader(new StreamReader(stream)))
			{
				reader.Configuration.RegisterClassMap<CalendarDayClassMap>();

				foreach (var record in reader.GetRecords<CalendarDay>())
					calendar.Days.Add(record);
			}

			return calendar;
		}

		private static Calendar XlsxToCalendar(string year, Stream stream)
		{
			var wb = new Workbook(stream);
			var sheet = wb.Worksheets[0];

			FindOptions fopts = new FindOptions();
			fopts.LookInType = LookInType.Values;
			fopts.LookAtType = LookAtType.EntireContent;

			// var header = sheet.GetRow(0);
			// var dayIndex = header.Cells.FindIndex(c => c.StringCellValue == "DAY");
			var dayIndex = sheet.Cells.Find("DAY", null, fopts).Column;
			// var dateIndex = header.Cells.FindIndex(c => c.StringCellValue == "DATE");
			var dateIndex = sheet.Cells.Find("DATE", null, fopts).Column;
			// var schoolDayIndex = header.Cells.FindIndex(c => c.StringCellValue == "SCHOOL DAY");
			var schoolDayIndex = sheet.Cells.Find("SCHOOL DAY", null, fopts).Column;
			// var membershipIndex = header.Cells.FindIndex(c => c.StringCellValue == "MEMBERSHIP");
			var membershipIndex = sheet.Cells.Find("MEMBERSHIP", null, fopts).Column;

			var calendar = new Calendar
			{
				SchoolYear = year,
				Days = new List<CalendarDay>(),
			};
			for (var i = 1; i <= sheet.Cells.MaxDataRow; i++)
			{
				var row = sheet.Cells.Rows[i];
				if (row == null || row.IsBlank)
					continue;

				calendar.Days.Add(new CalendarDay
				{
					DayOfWeek = row.GetCellByIndex(dayIndex).StringValue,
					Date = DateTime.FromOADate(row.GetCellByIndex(dateIndex).IntValue), // days since epoch
					SchoolDay = (byte)row.GetCellByIndex(schoolDayIndex).IntValue,
					Membership = (byte)row.GetCellByIndex(membershipIndex).IntValue,
				});
			}

			return calendar;
		}

		private static Dictionary<string, Func<string, Stream, Calendar>> _parsers = new Dictionary<string, Func<string, Stream, Calendar>>
		{
			{"text/csv", CsvToCalendar},
			{ContentTypes.XLSX, XlsxToCalendar},
		};

		[HttpPut("{year}")]
		[Authorize(Policy = "ADM=")]
		[ProducesResponseType(typeof(CalendarResponse), 201)]
		[ProducesResponseType(typeof(ErrorResponse), 400)]
		public async Task<IActionResult> Upload(string year, IFormFile file)
		{
			if (file == null)
				return new BadRequestObjectResult(
					new ErrorResponse($"Could not find parameter named '{nameof(file)}'."));

			if (!_parsers.ContainsKey(file.ContentType))
				return new BadRequestObjectResult(
					new ErrorResponse($"Invalid file Content-Type '{file.ContentType}'."));

			var parse = _parsers[file.ContentType]; var calendar = parse(year, file.OpenReadStream());

			calendar = await Task.Run(() => _context.SaveChanges(() => _calendars.CreateOrUpdate(calendar)));

			var username = User.FindFirst(c => c.Type == JwtRegisteredClaimNames.Sub).Value;
			using (var tx = _context.Database.BeginTransaction())
			{
				try
				{
					_audits.Create(new AuditRecord
					{
						Username = username,
						Activity = AuditRecordActivity.UPDATE_SCHOOL_CALENDAR,
						Timestamp = DateTime.Now,
						Identifier = calendar.SchoolYear,
						Field = null,
						Next = null,
						Previous = null,
					});
					_context.SaveChanges();

					tx.Commit();
				}
				catch (Exception)
				{
					tx.Rollback();
					throw;
				}
			}

			return new CreatedResult($"/api/calendars/{year}", new CalendarResponse
			{
				Calendar = calendar,
			});
		}
	}
}
