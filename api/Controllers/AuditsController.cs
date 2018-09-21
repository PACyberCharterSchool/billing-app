using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Threading.Tasks;

using api.Dtos;
using models;

namespace api.Controllers
{
	[Route("/api/[controller]")]
	public class AuditsController : Controller
  {
		private readonly PacBillContext _context;
    private readonly IAuditRecordRepository _audits;
		private readonly ILogger<AuditsController> _logger;

    public AuditsController(
      PacBillContext context,
      IAuditRecordRepository audits,
      ILogger<AuditsController> logger
    )
    {
      _context = context;
      _audits = audits;
      _logger = logger;
    }

		public struct AuditsResponse
		{
			public IList<AuditDto> Audits { get; set; }
		}

    [HttpGet]
		[Authorize(Policy = "PAY+")]
		[ProducesResponseType(typeof(AuditsResponse), 200)]
		public async Task<IActionResult> GetMany()
    {
			var audits = await Task.Run(() => _audits.GetMany());
			if (audits == null)
				audits = new List<AuditRecord>();

			return new ObjectResult(new AuditsResponse
			{
				Audits = audits.Select(a => new AuditDto(a)).ToList(),
			});
    }
  }
}