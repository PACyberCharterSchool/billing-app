using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;

namespace models
{
	public interface ICommittedStudentStatusRecordRepository
	{
		CommittedStudentStatusRecord Create(CommittedStudentStatusRecord record);
		CommittedStudentStatusRecord Create(DateTime time, CommittedStudentStatusRecord record);
		IList<CommittedStudentStatusRecord> CreateMany(IList<CommittedStudentStatusRecord> records);
		IList<CommittedStudentStatusRecord> CreateMany(DateTime time, IList<CommittedStudentStatusRecord> records);
	}

	public class CommittedStudentStatusRecordRepository : ICommittedStudentStatusRecordRepository
	{
		private readonly PacBillContext _context;
		private readonly DbSet<CommittedStudentStatusRecord> _records;
		private readonly ILogger<CommittedStudentStatusRecordRepository> _logger;

		public CommittedStudentStatusRecordRepository(
			PacBillContext context,
			ILogger<CommittedStudentStatusRecordRepository> logger)
		{
			_context = context;
			_records = context.CommittedStudentStatusRecords;
			_logger = logger;
		}

		public IList<CommittedStudentStatusRecord> CreateMany(DateTime time, IList<CommittedStudentStatusRecord> records)
		{
			foreach (var r in records)
				r.CommitTime = time;

			_records.AddRange(records);
			return records;
		}

		public IList<CommittedStudentStatusRecord> CreateMany(IList<CommittedStudentStatusRecord> records) =>
			CreateMany(DateTime.Now, records);

		public CommittedStudentStatusRecord Create(DateTime time, CommittedStudentStatusRecord record) =>
			CreateMany(time, new List<CommittedStudentStatusRecord> { record })[0];

		public CommittedStudentStatusRecord Create(CommittedStudentStatusRecord record) =>
			Create(DateTime.Now, record);
	}
}
