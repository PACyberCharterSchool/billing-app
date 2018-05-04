using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using Swashbuckle.AspNetCore.SwaggerGen;

using models;
using models.Transformers;

namespace api.Controllers
{
	[Route("api/[controller]")]
	public class StudentStatusRecordsController : Controller
	{
		private readonly PacBillContext _context;
		private readonly IPendingStudentStatusRecordRepository _pending;
		private readonly IAuditRecordRepository _audits;
		private readonly ITransformer _transformer;
		private readonly ILogger<StudentStatusRecordsController> _logger;

		public StudentStatusRecordsController(
			PacBillContext context,
			IPendingStudentStatusRecordRepository pending,
			IAuditRecordRepository audits,
			ITransformer transformer,
			ILogger<StudentStatusRecordsController> logger)
		{
			_context = context;
			_pending = pending;
			_audits = audits;
			_transformer = transformer;
			_logger = logger;
		}

		public struct PendingStudentStatusRecordsResponse
		{
			public IList<PendingStudentStatusRecord> StudentStatusRecords { get; set; }
		}

		public class GetManyArgs
		{
			[Range(0, int.MaxValue)]
			public int Skip { get; set; }

			[Range(0, int.MaxValue)]
			public int Take { get; set; }
		}

		[HttpGet("pending")]
		[Authorize(Policy = "STD+")]
		[ProducesResponseType(typeof(PendingStudentStatusRecordsResponse), 200)]
		public async Task<IActionResult> GetManyPending([FromQuery]GetManyArgs args)
		{
			if (!ModelState.IsValid)
				return new BadRequestObjectResult(new ErrorsResponse(ModelState));

			var records = await Task.Run(() => _pending.GetMany(skip: args.Skip, take: args.Take));
			if (records == null)
				records = new List<PendingStudentStatusRecord>();

			return new ObjectResult(new PendingStudentStatusRecordsResponse { StudentStatusRecords = records.ToList() });
		}

		private static readonly object _lock = new object();

		[HttpPost("pending/commit")]
		[Authorize(Policy = "PAY+")]
		[ProducesResponseType(200)]
		[SwaggerResponse(statusCode: 423, description: "Locked")]
		public IActionResult Commit()
		{
			bool acquired = false;
			try
			{
				// Monitor and async don't play well together, apparently.
				Monitor.TryEnter(_lock, ref acquired);
				if (!acquired)
					return StatusCode(423);

				// var pending = await Task.Run(() => _pending.GetMany());
				var pending = _pending.GetMany();
				if (pending == null)
					return Ok();

				using (var tx = _context.Database.BeginTransaction())
				{
					_transformer.Transform(pending).ToList();

					_pending.Truncate();

					var username = User.FindFirst(c => c.Type == JwtRegisteredClaimNames.Sub).Value;
					_audits.Create(new AuditRecord
					{
						Username = username,
						Activity = AuditRecordActivity.COMMIT_GENIUS,
					});

					_context.SaveChanges();
					tx.Commit();
				}

				return Ok();
			}
			finally
			{
				if (acquired)
					Monitor.Exit(_lock);
			}
		}
	}
}
