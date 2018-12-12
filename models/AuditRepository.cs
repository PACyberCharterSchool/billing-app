using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.Extensions.Logging;

namespace models
{
	public interface IAuditRepository
	{
		IEnumerable<AuditHeader> GetMany();
		AuditHeader Create(AuditHeader header);
	}

	public class AuditRepository : IAuditRepository
	{
		private readonly PacBillContext _context;
		private readonly IIncludableQueryable<AuditHeader, IList<AuditDetail>> _records;
		private readonly ILogger<AuditRepository> _logger;

		public AuditRepository(PacBillContext context, ILogger<AuditRepository> logger)
		{
			_context = context;
			_records = _context.AuditHeaders.Include(h => h.Details);
			_logger = logger;
		}

		public AuditHeader Create(AuditHeader header)
		{
			_context.Add(header);
			return header;
		}

		public IEnumerable<AuditHeader> GetMany() => _records.OrderByDescending(r => r.Timestamp);
	}
}
