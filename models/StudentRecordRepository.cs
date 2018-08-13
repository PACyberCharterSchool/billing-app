
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using static models.Common.PropertyMerger;

namespace models
{
	public interface IStudentRecordRepository
	{
		StudentRecordsHeader Get(string scope, int skip = 0, int take = 0);
		IList<string> GetScopes();
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

		public StudentRecordsHeader Get(string scope, int skip = 0, int take = 0)
		{
			var header = _context.StudentRecordsHeaders.SingleOrDefault(h => h.Scope == scope);
			if (header == null)
				return null;

			header.Records = header.Records.OrderBy(r => r.Id);
			if (skip > 0)
				header.Records = header.Records.Skip(skip);

			if (take > 0)
				header.Records = header.Records.Take(take);

			return header;
		}

		public IList<string> GetScopes()
			=> _context.StudentRecordsHeaders.Select(h => h.Scope).ToList();

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
