
using System;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using static models.Common.PropertyMerger;

namespace models
{
	public interface IStudentRecordRepository
	{
		StudentRecordsHeader Create(StudentRecordsHeader create);
		StudentRecordsHeader Replace(StudentRecordsHeader replace);
		StudentRecordsHeader Get(string scope);

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

		public StudentRecordsHeader Create(StudentRecordsHeader create)
		{
			create.Created = DateTime.Now;
			_context.Add(create);
			return create;
		}

		public StudentRecordsHeader Get(string scope)
			=> _context.StudentRecordsHeaders.Include(h => h.Records).SingleOrDefault(h => h.Scope == scope);

		public StudentRecordsHeader Replace(StudentRecordsHeader replace)
		{
			_context.Remove(replace);
			return Create(replace);
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
