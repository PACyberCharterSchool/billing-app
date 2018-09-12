using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

using Newtonsoft.Json;

using api.Dtos;
using models;
using Microsoft.AspNetCore.Http;
using System.IO;
using CsvHelper;
using Aspose.Cells;
using System.Text.RegularExpressions;

namespace api.Controllers
{
	[Route("/api/[controller]")]
	public class PaymentsController : Controller
	{
		private readonly PacBillContext _context;
		private readonly IPaymentRepository _payments;
		private readonly ISchoolDistrictRepository _districts;
		private readonly ILogger<PaymentsController> _logger;

		public PaymentsController(
			PacBillContext context,
			IPaymentRepository payments,
			ISchoolDistrictRepository districts,
			ILogger<PaymentsController> logger)
		{
			_context = context;
			_payments = payments;
			_districts = districts;
			_logger = logger;
		}

		public struct CreateUpdatePayment
		{
			[Required]
			[MinLength(1)]
			public string ExternalId { get; set; }

			[Required]
			[JsonConverter(typeof(PaymentTypeJsonConverter))]
			public PaymentType Type { get; set; }

			[BindRequired]
			[Range(100000000, 999999999)]
			public int SchoolDistrictAun { get; set; }

			public struct Split
			{
				[Required]
				public DateTime Date { get; set; }

				[Required]
				[Range(0, double.PositiveInfinity)]
				public decimal Amount { get; set; }

				[Required]
				[MinLength(1)]
				public string SchoolYear { get; set; }
			}

			[Required]
			public IList<Split> Splits { get; set; }
		}

		public struct PaymentsResponse
		{
			public IList<PaymentDto> Payments { get; set; }
		}

		[HttpPost]
		[Authorize(Policy = "PAY+")]
		[ProducesResponseType(typeof(PaymentsResponse), 201)]
		[ProducesResponseType(typeof(ErrorsResponse), 400)]
		[ProducesResponseType(409)]
		public async Task<IActionResult> Create([FromBody]CreateUpdatePayment create)
		{
			if (!ModelState.IsValid)
				return new BadRequestObjectResult(new ErrorsResponse(ModelState));

			var payments = new List<Payment>();
			for (var i = 0; i < create.Splits.Count; i++)
			{
				var split = create.Splits[i];

				payments.Add(new Payment
				{
					Date = split.Date,
					ExternalId = create.ExternalId,
					Type = create.Type,
					SchoolDistrict = _districts.GetByAun(create.SchoolDistrictAun),
					Split = i + 1,
					Amount = split.Amount,
					SchoolYear = split.SchoolYear,
				});
			}

			try
			{
				payments = await Task.Run(() => _context.SaveChanges(() => _payments.CreateMany(payments)).ToList());
			}
			catch (DbUpdateException) // check the inner exception?
			{
				return StatusCode(409);
			}

			return Created($"/api/payments/{payments[0].PaymentId}", new PaymentsResponse
			{
				Payments = payments.Select(p => new PaymentDto(p)).ToList(),
			});
		}

		[HttpGet]
		[Authorize(Policy = "PAY+")]
		[ProducesResponseType(typeof(PaymentsResponse), 200)]
		public async Task<IActionResult> GetMany()
		{
			var payments = await Task.Run(() => _payments.GetMany());
			if (payments == null)
				payments = new List<Payment>();

			return new ObjectResult(new PaymentsResponse
			{
				Payments = payments.Select(p => new PaymentDto(p)).ToList(),
			});
		}

		[HttpGet("{id}")]
		[Authorize(Policy = "PAY+")]
		[ProducesResponseType(typeof(PaymentsResponse), 200)]
		public async Task<IActionResult> GetManyById(string id)
		{
			var payments = await Task.Run(() => _payments.GetMany(id));
			if (payments == null)
				payments = new List<Payment>();

			return new ObjectResult(new PaymentsResponse
			{
				Payments = payments.Select(p => new PaymentDto(p)).ToList(),
			});
		}

		[HttpPut("{id}")]
		[Authorize(Policy = "PAY+")]
		[ProducesResponseType(200)]
		[ProducesResponseType(typeof(ErrorsResponse), 400)]
		[ProducesResponseType(404)]
		public async Task<IActionResult> Update(string id, [FromBody]CreateUpdatePayment update)
		{
			if (!ModelState.IsValid)
				return new BadRequestObjectResult(new ErrorsResponse(ModelState));

			var payments = new List<Payment>();
			for (var i = 0; i < update.Splits.Count; i++)
			{
				var split = update.Splits[i];

				payments.Add(new Payment
				{
					PaymentId = id,
					Date = split.Date,
					ExternalId = update.ExternalId,
					Type = update.Type,
					SchoolDistrict = _districts.GetByAun(update.SchoolDistrictAun),
					Split = i + 1,
					Amount = split.Amount,
					SchoolYear = split.SchoolYear,
				});
			}

			try
			{
				await Task.Run(() => _context.SaveChanges(() => _payments.UpdateMany(payments)));
			}
			catch (NotFoundException)
			{
				return NotFound();
			}

			return Ok();
		}

		private static Dictionary<string, (int Number, bool First)> _months = new Dictionary<string, (int Number, bool First)>
		{
			{"July", (7, true)},
			{"August", (8, true)},
			{"September", (9, true)},
			{"October", (10, true)},
			{"November", (11, true)},
			{"December", (12, true)},
			{"January", (1, false)},
			{"February", (2, false)},
			{"March", (3, false)},
			{"April", (4, false)},
			{"May", (5, false)},
			{"June", (6, false)},
		};

		private static IList<Payment> XlsxToPayments(ISchoolDistrictRepository districts, Stream stream)
		{
			var wb = new Workbook(stream);
			var sheet = wb.Worksheets[0];

			const int aunIndex = 0;
			const int amountIndex = 3;

			var amountHeader = sheet.Cells[0, amountIndex].StringValue.Trim();
			const string pattern = @"^(\w+) (\d{4}).*$";
			var matches = Regex.Matches(amountHeader, pattern);
			if (matches.Count != 1)
				throw new ArgumentException($"Column {amountIndex + 1} header has invalid format. Expected '{pattern}'.");

			var groups = matches[0].Groups;
			var month = _months[groups[1].Value];
			var year = int.Parse(groups[2].Value);
			string schoolYear;
			if (month.First)
				schoolYear = $"{year}-{year + 1}";
			else
				schoolYear = $"{year - 1}-{year}";

			var payments = new List<Payment>();
			for (var i = 1; i < sheet.Cells.MaxDataRow; i++)
			{
				var row = sheet.Cells.GetRow(i);
				if (row == null || row.IsBlank)
					continue;

				payments.Add(new Payment
				{
					Split = 1,
					Date = new DateTime(year, month.Number, 1),
					ExternalId = "PDE",
					Type = PaymentType.UniPay,
					Amount = (decimal)sheet.Cells[i, amountIndex].DoubleValue,
					SchoolYear = schoolYear,
					SchoolDistrict = districts.GetByAun(sheet.Cells[i, aunIndex].IntValue),
				});
			}

			return payments;
		}

		private static Dictionary<string, Func<ISchoolDistrictRepository, Stream, IList<Payment>>> _parsers =
			new Dictionary<string, Func<ISchoolDistrictRepository, Stream, IList<Payment>>>
			{
				{"application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", XlsxToPayments},
			};

		[HttpPut]
		[Authorize(Policy = "PAY+")]
		[ProducesResponseType(typeof(PaymentsResponse), 201)]
		[ProducesResponseType(typeof(ErrorResponse), 400)]
		[ProducesResponseType(typeof(ErrorResponse), 409)]
		public async Task<IActionResult> Upload(IFormFile file)
		{
			if (file == null)
				return new BadRequestObjectResult(
					new ErrorResponse($"Could not find parameter named '{nameof(file)}'."));

			if (!_parsers.ContainsKey(file.ContentType))
				return new BadRequestObjectResult(
					new ErrorResponse($"Invalid file Content-Type '{file.ContentType}'."));

			var parse = _parsers[file.ContentType];
			IList<Payment> payments;
			try
			{
				payments = parse(_districts, file.OpenReadStream());
			}
			catch (ArgumentException e)
			{
				return new BadRequestObjectResult(new ErrorResponse(e));
			}

			using (var tx = _context.Database.BeginTransaction())
			{
				try
				{
					payments = await Task.Run(() => _context.SaveChanges(() => _payments.CreateMany(payments)));
					tx.Commit();
				}
				catch (DbUpdateException)
				{
					tx.Rollback();
					return new StatusCodeResult(409);
				}
				catch (Exception)
				{
					tx.Rollback();
					throw;
				}
			}

			return new CreatedResult($"/api/schooldistricts", new PaymentsResponse
			{
				Payments = payments.Select(p => new PaymentDto(p)).ToList(),
			});
		}
	}
}
