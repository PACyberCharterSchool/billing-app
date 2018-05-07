using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;

namespace models
{
	public interface IStudentRepository
	{
		Student Get(int id);
		Student GetByPACyberId(string id);

		IList<Student> GetMany(string sort = "Id", SortDirection dir = null, int skip = 0, int take = 0, string filter = null);
	}

	public class StudentRepository : IStudentRepository
	{
		private readonly DbSet<Student> _students;
		private readonly IFilterParser _parser;
		private readonly ILogger<StudentRepository> _logger;

		public StudentRepository(PacBillContext ctx, IFilterParser parser, ILogger<StudentRepository> logger)
		{
			_students = ctx.Students;
			_parser = parser;
			_logger = logger;
		}

		public Student Get(int id) => _students.SingleOrDefault(s => s.Id == id);

		public Student GetByPACyberId(string id) => _students.SingleOrDefault(s => s.PACyberId == id);

		public IList<Student> GetMany(
			string sort = "Id",
			SortDirection dir = null,
			int skip = 0,
			int take = 0,
			string filter = null)
		{
			var students = _students.AsQueryable();
			if (!string.IsNullOrWhiteSpace(filter))
				students = students.Filter(_parser, filter);

			if (string.IsNullOrWhiteSpace(sort))
				sort = "Id";

			dir = dir ?? SortDirection.Ascending;
			students = students.SortBy(sort, dir);

			if (skip > 0)
				students = students.Skip(skip);

			if (take > 0)
				students = students.Take(take);

			return students.ToList();
		}
	}
}
