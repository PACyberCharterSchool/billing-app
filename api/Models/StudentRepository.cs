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
		IList<Student> GetMany(string field);
		IList<Student> GetMany(string field, string dir);
		IList<Student> GetMany(int skip, int take);
		IList<Student> GetMany(string field, string dir, int skip, int take);
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

		public IList<Student> GetMany(string field) => GetMany(field, null, 0, 0);

		public IList<Student> GetMany(string field, string dir) => GetMany(field, dir, 0, 0);

		public IList<Student> GetMany(int skip, int take) => GetMany(null, null, skip, take);

		public IList<Student> GetMany(string field, string dir, int skip, int take)
		{
			if (string.IsNullOrWhiteSpace(field))
				field = "Id";

			if (!Student.IsValidField(field))
				throw new ArgumentException($"Invalid sort field '{field}'.", "field");

			if (string.IsNullOrWhiteSpace(dir))
				dir = "asc";

			if (dir != "asc" && dir != "desc")
				throw new ArgumentException($"Invalid sort direction '{dir}'.", "dir");

			if (skip < 0)
				throw new ArgumentException($"Skip must be 0 or greater; was '{skip}'.", "skip");

			if (take < 0)
				throw new ArgumentException($"Take must be 0 or greater; was '{take}'.", "take");

			var students = _students.SortBy(field, dir);
			if (skip > 0)
				students = students.Skip(skip);

			if (take > 0)
				students = students.Take(take);

			return students.ToList();
		}
	}
}
