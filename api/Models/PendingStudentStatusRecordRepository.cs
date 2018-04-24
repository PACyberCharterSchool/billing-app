using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;

namespace api.Models
{
	public interface IPendingStudentStatusRecordRepository
	{
		IList<StudentStatusRecord> GetMany();
		IList<StudentStatusRecord> GetMany(int skip, int take);
	}

	public class PendingStudentStatusRecordRepository : IPendingStudentStatusRecordRepository
	{
		private readonly DbSet<StudentStatusRecord> _records;
		private readonly ILogger<PendingStudentStatusRecordRepository> _logger;

		public PendingStudentStatusRecordRepository(
			PacBillContext context,
			ILogger<PendingStudentStatusRecordRepository> logger)
		{
			_records = context.PendingStudentStatusRecords;
			_logger = logger;
		}

		public IList<StudentStatusRecord> GetMany() => GetMany(0, 0);

		public IList<StudentStatusRecord> GetMany(int skip, int take)
		{
			var records = _records.AsQueryable();

			if (skip > 0)
				records = records.Skip(skip);

			if (take > 0)
				records = records.Take(take);

			return records.OrderBy(r => r.Id).ToList();
		}
	}
}
