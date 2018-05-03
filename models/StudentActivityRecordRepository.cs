using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;

namespace models
{
	public interface IStudentActivityRecordRepository
	{
		IList<StudentActivityRecord> GetMany(string id = null, string activity = null, int skip = 0, int take = 0);
		StudentActivityRecord Create(StudentActivityRecord record);
		IList<StudentActivityRecord> CreateMany(IList<StudentActivityRecord> records);
	}

	public class StudentActivityRecordRepository : IStudentActivityRecordRepository
	{
		private readonly PacBillContext _context;
		private readonly DbSet<StudentActivityRecord> _records;
		private readonly ILogger<StudentActivityRecordRepository> _logger;

		public StudentActivityRecordRepository(PacBillContext context, ILogger<StudentActivityRecordRepository> logger)
		{
			_context = context;
			_records = context.StudentActivityRecords;
			_logger = logger;
		}

		public IList<StudentActivityRecord> GetMany(string id = null, string activity = null, int skip = 0, int take = 0)
		{
			var records = _records.AsQueryable();
			if (!string.IsNullOrWhiteSpace(id))
				records = records.Where(r => r.PACyberId == id);

			if (!string.IsNullOrWhiteSpace(activity))
				records = records.Where(r => r.Activity == StudentActivity.FromString(activity.Trim()));

			if (skip > 0)
				records = records.Skip(skip);

			if (take > 0)
				records = records.Take(take);

			return records.
				OrderBy(r => r.Timestamp).
				ThenBy(r => r.PACyberId).
				ThenBy(r => r.Sequence).
				ToList();
		}

		public StudentActivityRecord Create(StudentActivityRecord record) =>
			CreateMany(new List<StudentActivityRecord> { record })[0];

		public IList<StudentActivityRecord> CreateMany(IList<StudentActivityRecord> records)
		{
			_records.AddRange(records);
			return records;
		}
	}
}
