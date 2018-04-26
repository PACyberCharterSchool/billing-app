using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;

using models;

namespace api.Controllers
{
	[Route("api/[controller]")]
	public class SchoolDistrictsController : Controller
	{
		private readonly ISchoolDistrictRepository _schoolDistricts;
		private readonly ILogger<SchoolDistrictsController> _logger;

		public SchoolDistrictsController(
			ISchoolDistrictRepository schoolDistricts,
			ILogger<SchoolDistrictsController> logger)
		{
			_schoolDistricts = schoolDistricts;
			_logger = logger;
		}

		public struct SchoolDistrictResponse
		{
			public SchoolDistrict SchoolDistrict { get; set; }
		}

		[HttpGet("{id}")]
		[Authorize(Policy = "STD+")]
		[ProducesResponseType(typeof(SchoolDistrictResponse), 200)]
		[ProducesResponseType(404)]
		public async Task<IActionResult> GetById(int id)
		{
			var district = await Task.Run(() => _schoolDistricts.Get(id));
			if (district == null)
				return NotFound();

			return new ObjectResult(new SchoolDistrictResponse { SchoolDistrict = district });
		}

		public struct SchoolDistrictsResponse
		{
			public IList<SchoolDistrict> SchoolDistricts { get; set; }
		}

		[HttpGet]
		[Authorize(Policy = "STD+")]
		[ProducesResponseType(typeof(SchoolDistrictsResponse), 200)]
		public async Task<IActionResult> GetMany()
		{
			var districts = await Task.Run(() => _schoolDistricts.GetMany());
			if (districts == null)
				districts = new List<SchoolDistrict>();

			return new ObjectResult(new SchoolDistrictsResponse { SchoolDistricts = districts });
		}

		public class SchoolDistrictUpdate
		{
			[Required]
			[Range(100000000, 999999999)]
			public int Aun { get; set; }

			[Required]
			[MinLength(1)]
			public string Name { get; set; }

			[Required]
			[Range(0, double.PositiveInfinity)]
			public decimal Rate { get; set; }

			[Range(0, double.PositiveInfinity)]
			public decimal? AlternateRate { get; set; }

			[Required]
			[RegularExpression("^ACH|Check$")]
			public string PaymentType { get; set; }
		}

		[HttpPut("{id}")]
		[Authorize(Policy = "PAY+")]
		[ProducesResponseType(typeof(ErrorsResponse), 400)]
		[ProducesResponseType(404)]
		public async Task<IActionResult> Update(int id, [FromBody]SchoolDistrictUpdate update)
		{
			if (!ModelState.IsValid)
				return new BadRequestObjectResult(new ErrorsResponse(ModelState));

			if (_schoolDistricts.Get(id) == null)
				return NotFound();

			var district = new SchoolDistrict
			{
				Id = id,
				Aun = update.Aun,
				Name = update.Name,
				Rate = update.Rate,
				AlternateRate = update.AlternateRate,
				PaymentType = update.PaymentType,
			};
			await Task.Run(() => _schoolDistricts.CreateOrUpdate(district));
			return Ok();
		}
	}
}