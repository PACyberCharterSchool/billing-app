
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
		StudentRecordsHeader Get(string scope, int skip = 0, int take = 0, string filter = null);
		IList<string> GetScopes(bool? locked = null);
		bool IsLocked(string scope);
		void Lock(string scope);
		StudentRecord Update(StudentRecord update);
	}

	public class StudentRecordRepository : IStudentRecordRepository
	{
		private readonly PacBillContext _context;
		private readonly IFilterParser _parser;
		private readonly ILogger<StudentRecordRepository> _logger;

		public StudentRecordRepository(PacBillContext context, ILogger<StudentRecordRepository> logger)
		{
			_context = context;
			_parser = new FilterParser();
			_logger = logger;
		}

		public StudentRecordsHeader Get(string scope, int skip = 0, int take = 0, string filter = null)
		{
			var header = _context.
				StudentRecordsHeaders.
				AsNoTracking(). // HACK(Erik): Without this EF complains about changes, slowing everything down
				Include(h => h.Records).
				SingleOrDefault(h => h.Scope == scope);
			if (header == null)
				return null;

			header.Records = header.Records.OrderBy(r => r.Id);

			if (filter != null)
				header.Records = header.Records.Filter(_parser, filter);

			if (skip > 0)
				header.Records = header.Records.Skip(skip);

			if (take > 0)
				header.Records = header.Records.Take(take);

			return header;
		}

		public IList<string> GetScopes(bool? locked = null)
		{
			var scopes = _context.StudentRecordsHeaders.AsQueryable();

			if (locked != null)
				scopes = scopes.Where(h => h.Locked == locked.Value);

			return scopes.Select(h => h.Scope).ToList();
		}

		public bool IsLocked(string scope)
		 => _context.StudentRecordsHeaders.Where(r => r.Scope == scope).Select(r => r.Locked).SingleOrDefault();

		public void Lock(string scope)
		{
			var header = _context.StudentRecordsHeaders.Where(r => r.Scope == scope).Single();
			header.Locked = true;
			_context.Update(header);
		}

		public StudentRecord Update(StudentRecord update)
		{
			var current = _context.StudentRecords.Single(r => r.Id == update.Id);
			MergeProperties(current, update, new[] {
				 nameof(StudentRecord.Id),
				 nameof(StudentRecord.StudentId),
				 nameof(StudentRecord.SchoolDistrictId),
				 nameof(StudentRecord.SchoolDistrictName),
				 nameof(StudentRecord.Header),
				 nameof(StudentRecord.LastUpdated),
				 nameof(StudentRecord.ActivitySchoolYear),
				 nameof(StudentRecord.StudentPaSecuredId),
			});
			current.LastUpdated = DateTime.Now;
			_context.Update(current);

			return current;
		}
	}
}
