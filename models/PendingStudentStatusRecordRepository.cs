using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;

namespace models
{
	public interface IPendingStudentStatusRecordRepository
	{
		IEnumerable<PendingStudentStatusRecord> GetMany(int skip = 0, int take = 0);
		void Truncate();
	}

	public class PendingStudentStatusRecordRepository : IPendingStudentStatusRecordRepository
	{
		private readonly PacBillContext _context;
		private readonly DbSet<PendingStudentStatusRecord> _records;
		private readonly ILogger<PendingStudentStatusRecordRepository> _logger;

		public PendingStudentStatusRecordRepository(
			PacBillContext context,
			ILogger<PendingStudentStatusRecordRepository> logger)
		{
			_context = context;
			_records = context.PendingStudentStatusRecords;
			_logger = logger;
		}

		public IEnumerable<PendingStudentStatusRecord> GetMany(int skip = 0, int take = 0)
		{
			var records = _records.AsQueryable();

			if (skip > 0)
				records = records.Skip(skip);

			if (take > 0)
				records = records.Take(take);

			return records.OrderBy(r => r.Id);
		}

		public void Truncate()
		{
			_context.Database.ExecuteSqlCommand("TRUNCATE TABLE " + nameof(_context.PendingStudentStatusRecords));
		}
	}
}
