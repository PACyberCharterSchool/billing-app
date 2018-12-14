using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IdentityModel.Tokens.Jwt;

using api.Common;
using api.Dtos;
using models;
using System.Text.RegularExpressions;

namespace api.Controllers
{
	[Route("api/[controller]")]
	public class TemplatesController : Controller
	{
		private readonly PacBillContext _context;
		private readonly ITemplateRepository _templates;
		private readonly ILogger<TemplatesController> _logger;
		private readonly IAuditRepository _audits;

		public TemplatesController(
			PacBillContext context,
			ITemplateRepository templates,
			IAuditRepository audits,
			ILogger<TemplatesController> logger)
		{
			_context = context;
			_templates = templates;
			_audits = audits;
			_logger = logger;
		}

		[HttpGet("{type}/{year}")]
		[Authorize(Policy = "ADM=")]
		[Produces(ContentTypes.XLSX)]
		[ProducesResponseType(typeof(ErrorResponse), 400)]
		[ProducesResponseType(404)]
		[EnumerationPathParameter("type", typeof(ReportType))]
		public async Task<IActionResult> Get(string type, string year)
		{
			if (!ReportType.Values().Contains(type))
				return new BadRequestObjectResult(new ErrorResponse($"Invalid ReportType '{type}'."));

			var template = await Task.Run(() => _templates.Get(ReportType.FromString(type), year));
			if (template == null)
				return NotFound();

			var stream = new MemoryStream(template.Content);
			return new FileStreamResult(stream, ContentTypes.XLSX)
			{
				FileDownloadName = template.Name,
			};
		}

		public struct TemplatesResponse
		{
			public IList<TemplateDto> Templates { get; set; }
		}

		public class GetManyArgs
		{
			[EnumerationValidation(typeof(ReportType))]
			public string ReportType { get; set; }

			[RegularExpression(@"^\d{4}\-\d{4}$")]
			public string SchoolYear { get; set; }
		}

		[HttpGet]
		[Authorize(Policy = "ADM=")]
		[ProducesResponseType(typeof(TemplatesResponse), 200)]
		[ProducesResponseType(typeof(ErrorsResponse), 400)]
		public async Task<IActionResult> GetMany([FromQuery]GetManyArgs args)
		{
			if (!ModelState.IsValid)
				return new BadRequestObjectResult(new ErrorsResponse(ModelState));

			var templates = await Task.Run(() => _templates.GetManyMetadata(
				type: args.ReportType == null ? null : ReportType.FromString(args.ReportType),
				year: args.SchoolYear
			));

			if (templates == null)
				templates = new List<TemplateMetadata>();

			return new ObjectResult(new TemplatesResponse
			{
				Templates = templates.Select(t => new TemplateDto(t)).ToList(),
			});
		}

		public struct TemplateResponse
		{
			public TemplateDto Template { get; set; }
		}

		[HttpPut("{type}/{year}")]
		[Authorize(Policy = "ADM=")]
		[ProducesResponseType(typeof(TemplateResponse), 201)]
		[ProducesResponseType(typeof(ErrorResponse), 400)]
		[EnumerationPathParameter("type", typeof(ReportType))]
		public async Task<IActionResult> Upload(string type, string year, IFormFile content)
		{
			if (!ReportType.Values().Contains(type))
				return new BadRequestObjectResult(new ErrorResponse($"Invalid ReportType '{type}'."));

			if (!new Regex(@"^\d{4}-\d{4}$").IsMatch(year))
				return new BadRequestObjectResult(new ErrorResponse($"Invalid SchoolYear '{year}'"));

			if (content == null)
				return new BadRequestObjectResult(
					new ErrorResponse($"Could not find parameter named '{nameof(content)}'."));

			if (content.ContentType != ContentTypes.XLSX)
				return new BadRequestObjectResult(
					new ErrorResponse($"Invalid file Content-Type '{content.ContentType}'."));

			Template template;
			using (var ms = new MemoryStream())
			{
				content.OpenReadStream().CopyTo(ms);
				template = new Template
				{
					ReportType = ReportType.FromString(type),
					SchoolYear = year,
					Name = content.FileName,
					Content = ms.ToArray(),
				};
			}

			template = await Task.Run(() => _context.SaveChanges(() => _templates.CreateOrUpdate(template)));

			var username = User.FindFirst(c => c.Type == JwtRegisteredClaimNames.Sub).Value;
			using (var tx = _context.Database.BeginTransaction())
			{
				try
				{
					_context.SaveChanges(() => _audits.Create(new AuditHeader
					{
						Username = username,
						Activity = AuditActivity.UPDATE_TEMPLATE,
						Timestamp = DateTime.Now,
						Identifier = $"{template.ReportType}_{template.SchoolYear}",
					}));
					tx.Commit();
				}
				catch (Exception)
				{
					tx.Rollback();
					throw;
				}
			}

			return new CreatedResult($"/api/templates/{template.ReportType}/{template.SchoolYear}", new TemplateResponse
			{
				Template = new TemplateDto(template),
			});
		}
	}
}
