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

		public IList<StudentStatusRecord> GetMany() => _records.ToList();
	}
}
