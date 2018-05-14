using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using api.Dtos;
using models;

namespace api.Controllers
{
	[Route("/api/[controller]")]
	public class PaymentsController : Controller
	{
		private readonly IPaymentRepository _payments;
		private readonly ILogger<PaymentsController> _logger;

		public PaymentsController(IPaymentRepository payments, ILogger<PaymentsController> logger)
		{
			_payments = payments;
			_logger = logger;
		}

		public struct CreatePayment
		{
			public DateTime Date { get; set; }
			public string ExternalId { get; set; }
			public PaymentType Type { get; set; }
			public int SchoolDistrictId { get; set; }

			public struct Split
			{
				public decimal Amount { get; set; }
				public string SchoolYear { get; set; }
			}

			public IList<Split> Splits { get; set; }
		}

		[HttpPost]
		[Authorize(Policy = "PAY+")]
		[ProducesResponseType(201)]
		[ProducesResponseType(400)]
		public async Task<IActionResult> Create([FromBody]CreatePayment create)
		{
			throw new NotImplementedException();
		}

		public struct PaymentsResponse
		{
			public IList<PaymentDto> Payments { get; set; }
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
	}
}
