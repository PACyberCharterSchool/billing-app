using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

using CsvHelper;

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

		[HttpPut("{year}")]
		[Authorize(Policy = "ADM=")]
		[ProducesResponseType(typeof(CalendarResponse), 201)]
		[ProducesResponseType(typeof(ErrorResponse), 400)]
		public async Task<IActionResult> Upload(string year, IFormFile file)
		{
			if (file.ContentType != "text/csv")
				return new BadRequestObjectResult(
					new ErrorResponse($"File Content-Type must be text/csv; was {file.ContentType}."));

			var calendar = CsvToCalendar(year, file.OpenReadStream());
			calendar = await Task.Run(() => _context.SaveChanges(() => _calendars.CreateOrUpdate(calendar)));

			return new CreatedResult($"/api/calendars/{year}", new CalendarResponse
			{
				Calendar = calendar,
			});
		}
	}
}
