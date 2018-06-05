using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using api.Dtos;
using models;

namespace api.Controllers
{
	[Route("api/[controller]")]
	public class TemplatesController : Controller
	{
		private readonly PacBillContext _context;
		private readonly ITemplateRepository _templates;
		private readonly ILogger<TemplatesController> _logger;

		public TemplatesController(
			PacBillContext context,
			ITemplateRepository templates,
			ILogger<TemplatesController> logger)
		{
			_context = context;
			_templates = templates;
			_logger = logger;
		}

		[HttpGet("{type}/{year}")]
		[Authorize(Policy = "ADM=")]
		[Produces("application/vnd.openxmlformats-officedocument.spreadsheetml.sheet")]
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
			return new FileStreamResult(stream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet")
			{
				FileDownloadName = template.Name,
			};
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

			if (content == null)
				return new BadRequestObjectResult(
					new ErrorResponse($"Could not find parameter named '{nameof(content)}'."));

			if (content.ContentType != "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet")
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
			return new CreatedResult($"/api/templates/{template.ReportType}/{template.SchoolYear}", new TemplateResponse
			{
				Template = new TemplateDto(template),
			});
		}
	}
}
