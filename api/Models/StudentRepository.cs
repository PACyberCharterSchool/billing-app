using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;

namespace api.Models
{
	public interface IStudentRepository
	{
		Student Get(int id);

		// CS0854: can't use optional parameters in expression trees
		IList<Student> GetMany();
		IList<Student> GetMany(string sort);
		IList<Student> GetMany(string sort, string dir);
		IList<Student> GetMany(int skip, int take);
		IList<Student> GetMany(string sort, string dir, int skip, int take);
		IList<Student> GetMany(string filter, string op, object value);
		IList<Student> GetMany(string sort, string dir, int skip, int take, string filter, string op, object value);
	}

	public class StudentRepository : IStudentRepository
	{
		private readonly DbSet<Student> _students;
		private readonly ILogger<StudentRepository> _logger;

		public StudentRepository(PacBillContext ctx, ILogger<StudentRepository> logger)
		{
			_students = ctx.Students;
			_logger = logger;
		}

		public Student Get(int id) => _students.Where(s => s.Id == id).SingleOrDefault();

		public IList<Student> GetMany() => GetMany(null, null, 0, 0);

		public IList<Student> GetMany(string sort) => GetMany(sort, null, 0, 0);

		public IList<Student> GetMany(string sort, string dir) => GetMany(sort, dir, 0, 0);

		public IList<Student> GetMany(int skip, int take) => GetMany(null, null, skip, take);

		public IList<Student> GetMany(string sort, string dir, int skip, int take) =>
			GetMany(sort, dir, skip, take, null, null, null);

		public IList<Student> GetMany(string filter, string op, object value) =>
			GetMany(null, null, 0, 0, filter, op, value);

		public IList<Student> GetMany(string sort, string dir, int skip, int take, string filter, string op, object value)
		{
			var students = _students.AsQueryable();
			if (!string.IsNullOrWhiteSpace(filter))
				students = students.Filter(filter, op, value);

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
