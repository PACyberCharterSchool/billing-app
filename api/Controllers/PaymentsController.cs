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
			public DateTime Date { get; set; }

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
					Date = create.Date,
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
					Date = update.Date,
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
	}
}
