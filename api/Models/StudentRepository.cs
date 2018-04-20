using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;

using api.Models;

namespace api.Models
{
	public interface IStudentRepository
	{
		Student Get(int id);

		// CS0854: can't use optional parameters in expression trees
		IList<Student> GetMany();
		IList<Student> GetMany(string sort, string dir);
		IList<Student> GetMany(int skip, int take);
		IList<Student> GetMany(string sort, string dir, int skip, int take);
		IList<Student> GetMany(string filter);
		IList<Student> GetMany(string sort, string dir, int skip, int take, string filter);
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

		public Student Get(int id) => _students.Where(s => s.Id == id).SingleOrDefault();

		public IList<Student> GetMany() => GetMany(null, null, 0, 0);

		public IList<Student> GetMany(string sort, string dir) => GetMany(sort, dir, 0, 0);

		public IList<Student> GetMany(int skip, int take) => GetMany(null, null, skip, take);

		public IList<Student> GetMany(string sort, string dir, int skip, int take) =>
			GetMany(sort, dir, skip, take, null);

		public IList<Student> GetMany(string filter) =>
			GetMany(null, null, 0, 0, filter);

		public IList<Student> GetMany(string sort, string dir, int skip, int take, string filter)
		{
			var students = _students.AsQueryable();
			if (!string.IsNullOrWhiteSpace(filter))
				students = students.Filter(_parser, filter);

			if (string.IsNullOrWhiteSpace(sort))
				sort = "Id";

			if (string.IsNullOrWhiteSpace(dir))
				dir = "asc";

			students = students.SortBy(sort, dir);

			if (skip > 0)
				students = students.Skip(skip);

			if (take > 0)
				students = students.Take(take);

			return students.ToList();
		}
	}
}
