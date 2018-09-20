using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;

namespace models
{
	public interface IAuditRecordRepository
	{
		IEnumerable<AuditRecord> GetMany();
		AuditRecord Create(AuditRecord record);
		AuditRecord Create(DateTime time, AuditRecord record);
		IList<AuditRecord> CreateMany(IList<AuditRecord> records);
		IList<AuditRecord> CreateMany(DateTime time, IList<AuditRecord> records);
	}

	public class AuditRecordRepository : IAuditRecordRepository
	{
		private readonly PacBillContext _context;
		private readonly DbSet<AuditRecord> _records;
		private readonly ILogger<AuditRecordRepository> _logger;

		public AuditRecordRepository(PacBillContext context, ILogger<AuditRecordRepository> logger)
		{
			_context = context;
			_records = context.AuditRecords;
			_logger = logger;
		}
		public IEnumerable<AuditRecord> GetMany() => _records.OrderBy(r => r.Timestamp);

		public AuditRecord Create(AuditRecord record) => Create(DateTime.Now, record);

		public AuditRecord Create(DateTime time, AuditRecord record)
			=> CreateMany(time, new List<AuditRecord> { record })[0];

		public IList<AuditRecord> CreateMany(IList<AuditRecord> records) => CreateMany(DateTime.Now, records);

		public IList<AuditRecord> CreateMany(DateTime time, IList<AuditRecord> records)
		{
			foreach (var record in records)
				record.Timestamp = time;

			_records.AddRange(records);
			return records;
		}
	}
}
