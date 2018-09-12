using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

using CsvHelper;
using Aspose.Cells;

using api.Dtos;
using models;

namespace api.Controllers
{
	[Route("api/[controller]")]
	public class SchoolDistrictsController : Controller
	{
		private readonly PacBillContext _context;
		private readonly ISchoolDistrictRepository _schoolDistricts;
		private readonly ILogger<SchoolDistrictsController> _logger;

		public SchoolDistrictsController(
			PacBillContext context,
			ISchoolDistrictRepository schoolDistricts,
			ILogger<SchoolDistrictsController> logger)
		{
			_context = context;
			_schoolDistricts = schoolDistricts;
			_logger = logger;
		}

		public struct SchoolDistrictResponse
		{
			public SchoolDistrictDto SchoolDistrict { get; set; }
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

			return new ObjectResult(new SchoolDistrictResponse { SchoolDistrict = new SchoolDistrictDto(district) });
		}

		public struct SchoolDistrictsResponse
		{
			public IList<SchoolDistrictDto> SchoolDistricts { get; set; }
		}

		[HttpGet]
		[Authorize(Policy = "STD+")]
		[ProducesResponseType(typeof(SchoolDistrictsResponse), 200)]
		public async Task<IActionResult> GetMany()
		{
			var districts = await Task.Run(() => _schoolDistricts.GetMany());
			if (districts == null)
				districts = new List<SchoolDistrict>();

			return new ObjectResult(new SchoolDistrictsResponse
			{
				SchoolDistricts = districts.Select(d => new SchoolDistrictDto(d)).ToList(),
			});
		}

		public class SchoolDistrictUpdate
		{
			[Required]
			[Range(100000000, 999999999)]
			public int Aun { get; set; }

			[Required]
			[MinLength(1)]
			public string Name { get; set; }

			[BindRequired]
			[Range(0, double.PositiveInfinity)]
			public decimal Rate { get; set; }

			[Range(0, double.PositiveInfinity)]
			public decimal? AlternateRate { get; set; }

			[Range(0, double.PositiveInfinity)]
			public decimal SpecialEducationRate { get; set; }

			[Range(0, double.PositiveInfinity)]
			public decimal? AlternateSpecialEducationRate { get; set; }

			[Required]
			[JsonConverter(typeof(SchoolDistrictPaymentTypeJsonConverter))]
			public SchoolDistrictPaymentType PaymentType { get; set; }
		}

		[HttpPut("{id}")]
		[Authorize(Policy = "PAY+")]
		[ProducesResponseType(200)]
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
				SpecialEducationRate = update.SpecialEducationRate,
				AlternateSpecialEducationRate = update.AlternateSpecialEducationRate,
				PaymentType = update.PaymentType,
			};
			await Task.Run(() => _context.SaveChanges(() => _schoolDistricts.CreateOrUpdate(district)));
			return Ok();
		}

		private static IList<SchoolDistrict> CsvToDistricts(Stream stream)
		{
			using (var reader = new CsvReader(new StreamReader(stream)))
			{
				reader.Configuration.RegisterClassMap<SchoolDistrictClassMap>();
				reader.Configuration.HeaderValidated = null;
				return reader.GetRecords<SchoolDistrict>().ToList();
			}
		}
		private static IList<SchoolDistrict> XlsxToDistricts(Stream stream)
		{
			var wb = new Workbook(stream);
			var sheet = wb.Worksheets[0];
			int aunIndex, nameIndex, rateIndex, specialRateIndex, typeIndex;
			FindOptions fopts = new FindOptions();
			fopts.LookAtType = LookAtType.EntireContent;
			fopts.LookInType = LookInType.Values;

			Cell cell = sheet.Cells.Find("AUN", null, fopts);
			aunIndex = cell.Column;
			cell = sheet.Cells.Find("School District", null, fopts);
			nameIndex = cell.Column;
			cell = sheet.Cells.Find("Nonspecial", null, fopts);
			rateIndex = cell.Column;
			cell = sheet.Cells.Find("Special", null, fopts);
			specialRateIndex = cell.Column;
			cell = sheet.Cells.Find("PaymentType", null, fopts);
			typeIndex = cell.Column;

			var districts = new List<SchoolDistrict>();
			for (var i = 1; i <= sheet.Cells.MaxDataRow; i++)
			{
				var row = sheet.Cells.GetRow(i);
				if (row == null || row.IsBlank)
					continue;

				var ptype = sheet.Cells[i, typeIndex].StringValue;
				districts.Add(new SchoolDistrict
				{
					Aun = (int) sheet.Cells[i, aunIndex].IntValue,
					Name = sheet.Cells[i, nameIndex].StringValue,
					Rate = (decimal)sheet.Cells[i, rateIndex].DoubleValue,
					SpecialEducationRate = (decimal)sheet.Cells[i, specialRateIndex].DoubleValue,
					PaymentType = string.IsNullOrWhiteSpace(ptype) ? null : SchoolDistrictPaymentType.FromString(ptype),
				});
			}

			return districts;
		}

		private static Dictionary<string, Func<Stream, IList<SchoolDistrict>>> _parsers =
			new Dictionary<string, Func<Stream, IList<SchoolDistrict>>>
			{
				{"text/csv", CsvToDistricts},
				{"application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", XlsxToDistricts},
			};

		[HttpPut]
		[Authorize(Policy = "PAY+")]
		[ProducesResponseType(typeof(SchoolDistrictsResponse), 201)]
		[ProducesResponseType(typeof(ErrorResponse), 400)]
		public async Task<IActionResult> Upload(IFormFile file)
		{
			if (file == null)
				return new BadRequestObjectResult(
					new ErrorResponse($"Could not find parameter named '{nameof(file)}'."));

			if (!_parsers.ContainsKey(file.ContentType))
				return new BadRequestObjectResult(
					new ErrorResponse($"Invalid file Content-Type '{file.ContentType}'."));

			var parse = _parsers[file.ContentType];
			var districts = parse(file.OpenReadStream());

			districts = await Task.Run(() => _context.SaveChanges(() => _schoolDistricts.CreateOrUpdateMany(districts)));

			return new CreatedResult($"/api/schooldistricts", new SchoolDistrictsResponse
			{
				SchoolDistricts = districts.Select(d => new SchoolDistrictDto(d)).ToList(),
			});
		}
	}
}
