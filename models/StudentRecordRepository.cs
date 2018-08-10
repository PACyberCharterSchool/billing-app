
using System;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using static models.Common.PropertyMerger;

namespace models
{
	public interface IStudentRecordRepository
	{
		StudentRecordsHeader Get(string scope);
		void Lock(string scope);
		StudentRecord Update(StudentRecord update);
	}

	public class StudentRecordRepository : IStudentRecordRepository
	{
		private readonly PacBillContext _context;
		private readonly ILogger<StudentRecordRepository> _logger;

		public StudentRecordRepository(PacBillContext context, ILogger<StudentRecordRepository> logger)
		{
			_context = context;
			_logger = logger;
		}

		public StudentRecordsHeader Get(string scope)
			=> _context.StudentRecordsHeaders.Include(h => h.Records).SingleOrDefault(h => h.Scope == scope);

		public void Lock(string scope)
		{
			var header = _context.StudentRecordsHeaders.Single();
			header.Locked = true;
			_context.Update(header);
		}

		public StudentRecord Update(StudentRecord update)
		{
			var current = _context.StudentRecords.Single(r => r.Id == update.Id);
			MergeProperties(current, update, new[] {
				 nameof(StudentRecord.Id) ,
				 nameof(StudentRecord.Header),
				 nameof(StudentRecord.LastUpdated),
			});
			_context.Update(current);

			return current;
		}
	}
}
