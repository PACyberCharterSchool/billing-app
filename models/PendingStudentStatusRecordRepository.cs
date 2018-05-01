using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;

namespace models
{
	public interface IPendingStudentStatusRecordRepository
	{
		IEnumerable<PendingStudentStatusRecord> GetMany();
		IEnumerable<PendingStudentStatusRecord> GetMany(int skip, int take);
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

		public IEnumerable<PendingStudentStatusRecord> GetMany() => GetMany(0, 0);

		public IEnumerable<PendingStudentStatusRecord> GetMany(int skip, int take)
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
