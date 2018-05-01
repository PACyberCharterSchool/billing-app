using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;

namespace models
{
	public interface IStudentActivityRecordRepository
	{
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

		public StudentActivityRecord Create(StudentActivityRecord record) =>
			CreateMany(new List<StudentActivityRecord> { record })[0];

		public IList<StudentActivityRecord> CreateMany(IList<StudentActivityRecord> records)
		{
			_records.AddRange(records);
			return records;
		}
	}
}
