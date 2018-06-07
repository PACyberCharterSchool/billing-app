using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.IdentityModel.Tokens.Jwt;

using api.Dtos;
using models;

namespace api.Controllers
{
  [Route("/api/[controller]")]
  public class DigitalSignaturesController : Controller
  {
    private readonly PacBillContext _context;
    private readonly IDigitalSignatureRepository _signatures;
    private readonly ILogger<DigitalSignaturesController> _logger;

    public DigitalSignaturesController(
        PacBillContext context,
        IDigitalSignatureRepository signatures,
        ILogger<DigitalSignaturesController> logger
        )
    {
      _context = context;
      _signatures = signatures;
      _logger = logger;
    }

		public struct DigitalSignatureResponse
		{
			public DigitalSignatureDto DigitalSignature { get; set; }
		}

    public struct DigitalSignaturesResponse
    {
      public IList<DigitalSignatureDto> DigitalSignatures { get; set; }
    }

    [HttpGet]
		[Authorize(Policy = "ADM=")]
    [ProducesResponseType(typeof(DigitalSignaturesResponse), 200)]
    public async Task<IActionResult> GetMany()
    {
      var signatures = await Task.Run(() => _signatures.GetMany());
      if (signatures == null)
        signatures = new List<DigitalSignature>();
      return new ObjectResult(
        new DigitalSignaturesResponse
        {
          DigitalSignatures = signatures.Select(s => new DigitalSignatureDto(s)).ToList(),
        }
      );
    }

    [HttpGet("{id}")]
		[Authorize(Policy = "ADM=")]
    [ProducesResponseType(typeof(DigitalSignatureResponse), 200)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> Get(int id)
    {
      try
      {
        var signature = await Task.Run(() => _signatures.Get(id));
        _logger.LogInformation($"DigitalSignaturesController#Get(): signature is {signature}.");
        return new ObjectResult(new DigitalSignatureResponse
        {
          DigitalSignature = new DigitalSignatureDto(signature)
        });
      }
      catch (NotFoundException)
      {
        return NotFound();
      }
    }

		public class CreateUpdateDigitalSignature
		{
      public CreateUpdateDigitalSignature() {}

			[Required]
			[MinLength(1)]
			public string title { get; set; }

			[Required]
			[MinLength(1)]
			public string fileName { get; set; }

      public string userName { get; set; }

      public IFormFile file { get; set; }

      public byte[] imgData { get; set; }
		}

    [HttpPost]
    [Authorize(Policy = "ADM=")]
    [ProducesResponseType(typeof(DigitalSignatureResponse), 200)]
    [ProducesResponseType(typeof(ErrorsResponse), 400)]
    [ProducesResponseType(409)]
    public async Task<IActionResult> Create(CreateUpdateDigitalSignature create)
    {
      _logger.LogInformation($"DigitalSignatureController#Create():  create is {create}.");

      if (!ModelState.IsValid)
        return new BadRequestObjectResult(new ErrorsResponse(ModelState));

      if (create.file == null)
				return new BadRequestObjectResult(
					new ErrorResponse($"Could not find parameter named '{nameof(create.file)}'."));

      Byte[] imgData = new Byte[create.file.Length];

      using(var stream = new MemoryStream())
      {
        create.file.CopyTo(stream);
        imgData = stream.ToArray(); 
      }

      _logger.LogInformation($"DigitalSignatureController#Create():  imgData length is {imgData.Length}.");
      var username = User.FindFirst(c => c.Type == JwtRegisteredClaimNames.Sub).Value;
      var signature = new DigitalSignature
      {
        Title = create.title,
        FileName = create.fileName,
        UserName = username,
        imgData = imgData
      };
      
      try {
        signature = await Task.Run(() => _context.SaveChanges(() => _signatures.Create(signature)));
        return new CreatedResult($"/api/signatures/{signature.Id}", new DigitalSignatureResponse
        {
          DigitalSignature = new DigitalSignatureDto(signature),
        });
      }
      catch (DbUpdateException)
      {
        return new StatusCodeResult(409);
      }
    }

    [HttpDelete("{id}")]
    [Authorize(Policy = "ADM=")]
    [ProducesResponseType(200)]
    [ProducesResponseType(typeof(ErrorsResponse), 400)]
    public async Task<IActionResult> Delete(int id)
    {
      if (!ModelState.IsValid)
        return new BadRequestObjectResult(new ErrorsResponse(ModelState));

      var username = User.FindFirst(c => c.Type == JwtRegisteredClaimNames.Sub).Value;

      try {
        await Task.Run(() => _context.SaveChanges(() => _signatures.Delete(id)));
        return Ok();
      }
      catch (NotFoundException)
      {
        return NotFound();
      }
    }

    [HttpPut("{id}")]
    [Authorize(Policy = "ADM=")]
    [ProducesResponseType(200)]
    [ProducesResponseType(typeof(ErrorsResponse), 400)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> Update(int id, [FromBody]CreateUpdateDigitalSignature update)
    {
      if (!ModelState.IsValid)
        return new BadRequestObjectResult(new ErrorsResponse(ModelState));

			var username = User.FindFirst(c => c.Type == JwtRegisteredClaimNames.Sub).Value;
      var signature = new DigitalSignature
      {
        Id = id,
        Title = update.title,
        UserName = update.userName,
        FileName = update.fileName,
        imgData = update.imgData
      };

      try {
        await Task.Run(() => _context.SaveChanges(() => _signatures.Update(signature)));
        return Ok();
      }
      catch (NotFoundException)
      {
        return NotFound();
      }
    }
  }
}
