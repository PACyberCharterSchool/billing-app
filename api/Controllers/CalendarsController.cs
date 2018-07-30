using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

using CsvHelper;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;

using api.Common;
using models;

namespace api.Controllers
{
	[Route("/api/[controller]")]
	public class CalendarsController : Controller
	{
		private readonly PacBillContext _context;
		private readonly ICalendarRepository _calendars;
		private readonly ILogger<CalendarsController> _logger;

		public CalendarsController(
			PacBillContext context,
			ICalendarRepository calendars,
			ILogger<CalendarsController> logger)
		{
			_context = context;
			_calendars = calendars;
			_logger = logger;
		}

		public struct CalendarResponse
		{
			public Calendar Calendar { get; set; }
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
			var wb = new XSSFWorkbook(stream);
			var sheet = wb.GetSheetAt(0);

			var header = sheet.GetRow(0);
			var dayIndex = header.Cells.FindIndex(c => c.StringCellValue == "DAY");
			var dateIndex = header.Cells.FindIndex(c => c.StringCellValue == "DATE");
			var schoolDayIndex = header.Cells.FindIndex(c => c.StringCellValue == "SCHOOL DAY");
			var membershipIndex = header.Cells.FindIndex(c => c.StringCellValue == "MEMBERSHIP");

			var calendar = new Calendar
			{
				SchoolYear = year,
				Days = new List<CalendarDay>(),
			};
			for (var i = 1; i <= sheet.LastRowNum; i++)
			{
				var row = sheet.GetRow(i);
				if (row == null || row.Cells.All(c => c.CellType == CellType.Blank))
					continue;

				calendar.Days.Add(new CalendarDay
				{
					DayOfWeek = row.GetCell(dayIndex).StringCellValue,
					Date = DateTime.FromOADate(row.GetCell(dateIndex).NumericCellValue), // days since epoch
					SchoolDay = (byte)row.GetCell(schoolDayIndex).NumericCellValue,
					Membership = (byte)row.GetCell(membershipIndex).NumericCellValue,
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

			var parse = _parsers[file.ContentType];
			var calendar = parse(year, file.OpenReadStream());

			calendar = await Task.Run(() => _context.SaveChanges(() => _calendars.CreateOrUpdate(calendar)));

			return new CreatedResult($"/api/calendars/{year}", new CalendarResponse
			{
				Calendar = calendar,
			});
		}
	}
}
