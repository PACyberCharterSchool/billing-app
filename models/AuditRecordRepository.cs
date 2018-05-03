using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;

namespace models
{
	public interface IAuditRecordRepository
	{
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
