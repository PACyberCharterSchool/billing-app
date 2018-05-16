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

using api.Dtos;
using models;

namespace api.Controllers
{
	[Route("/api/[controller]")]
	public class RefundsController : Controller
	{
		private readonly PacBillContext _context;
		private readonly IRefundRepository _refunds;
		private readonly ISchoolDistrictRepository _districts;
		private readonly ILogger<RefundsController> _logger;

		public RefundsController(
			PacBillContext context,
			IRefundRepository refunds,
			ISchoolDistrictRepository districts,
			ILogger<RefundsController> logger)
		{
			_context = context;
			_refunds = refunds;
			_districts = districts;
			_logger = logger;
		}

		public struct CreateUpdateRefund
		{
			[BindRequired]
			[Range(0, double.PositiveInfinity)]
			public decimal Amount { get; set; }

			[Required]
			[MinLength(1)]
			public string CheckNumber { get; set; }

			[BindRequired]
			public DateTime Date { get; set; }

			[Required]
			[MinLength(1)]
			public string SchoolYear { get; set; }

			[BindRequired]
			[Range(100000000, 999999999)]
			public int SchoolDistrictAun { get; set; }
		}

		public struct RefundResponse
		{
			public RefundDto Refund { get; set; }
		}

		[HttpPost]
		[Authorize(Policy = "PAY+")]
		[ProducesResponseType(typeof(RefundResponse), 200)]
		[ProducesResponseType(typeof(ErrorsResponse), 400)]
		[ProducesResponseType(409)]
		public async Task<IActionResult> Create([FromBody]CreateUpdateRefund create)
		{
			if (!ModelState.IsValid)
				return new BadRequestObjectResult(new ErrorsResponse(ModelState));

			var refund = new Refund
			{
				Amount = create.Amount,
				CheckNumber = create.CheckNumber,
				Date = create.Date,
				SchoolYear = create.SchoolYear,
				SchoolDistrict = _districts.GetByAun(create.SchoolDistrictAun),
			};

			try
			{
				refund = await Task.Run(() => _context.SaveChanges(() => _refunds.Create(refund)));
				return new CreatedResult($"/api/refunds/{refund.Id}", new RefundResponse
				{
					Refund = new RefundDto(refund),
				});
			}
			catch (DbUpdateException)
			{
				return new StatusCodeResult(409);
			}
		}

		public struct RefundsResponse
		{
			public IList<RefundDto> Refunds { get; set; }
		}

		[HttpGet("{id}")]
		[Authorize(Policy = "PAY+")]
		[ProducesResponseType(typeof(RefundResponse), 200)]
		[ProducesResponseType(404)]
		public async Task<IActionResult> Get(int id)
		{
			try
			{
				var refund = await Task.Run(() => _refunds.Get(id));
				return new ObjectResult(new RefundResponse
				{
					Refund = new RefundDto(refund),
				});
			}
			catch (NotFoundException)
			{
				return NotFound();
			}
		}

		[HttpGet]
		[Authorize(Policy = "PAY+")]
		[ProducesResponseType(typeof(RefundsResponse), 200)]
		public async Task<IActionResult> GetMany()
		{
			var refunds = await Task.Run(() => _refunds.GetMany());
			if (refunds == null)
				refunds = new List<Refund>();

			return new ObjectResult(new RefundsResponse
			{
				Refunds = refunds.Select(r => new RefundDto(r)).ToList(),
			});
		}

		[HttpPut("{id}")]
		[Authorize(Policy = "PAY+")]
		[ProducesResponseType(200)]
		[ProducesResponseType(typeof(ErrorsResponse), 400)]
		[ProducesResponseType(404)]
		public async Task<IActionResult> Update(int id, [FromBody]CreateUpdateRefund update)
		{
			if (!ModelState.IsValid)
				return new BadRequestObjectResult(new ErrorsResponse(ModelState));

			var refund = new Refund
			{
				Id = id,
				Amount = update.Amount,
				CheckNumber = update.CheckNumber,
				Date = update.Date,
				SchoolYear = update.SchoolYear,
				SchoolDistrict = _districts.GetByAun(update.SchoolDistrictAun),
			};

			try
			{
				await Task.Run(() => _context.SaveChanges(() => _refunds.Update(refund)));
				return Ok();
			}
			catch (NotFoundException)
			{
				return NotFound();
			}
		}
	}
}
