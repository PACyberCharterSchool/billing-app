using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;

namespace api.Models
{
	public interface IStudentRepository
	{
		Student Get(int id);

		// CS0854: can't use optional parameters in expression trees
		IList<Student> GetMany(string field, string dir, int skip, int take);
	}

	public class StudentRepository : IStudentRepository
	{
		private readonly DbSet<Student> _students;

		public StudentRepository(PacBillContext ctx)
		{
			_students = ctx.Students;
		}

		public Student Get(int id) => _students.Where(s => s.Id == id).SingleOrDefault();

		public IList<Student> GetMany(string field, string dir, int skip, int take)
		{
			Console.WriteLine($"GetMany => field: {field}; dir: {dir}; skip: {skip}; take: {take}");

			// TODO(Erik): validate field
			if (string.IsNullOrWhiteSpace(field))
				field = "Id";

			// TODO(Erik): validate asc/desc
			if (string.IsNullOrWhiteSpace(dir))
				dir = "asc";

			var students = _students.SortBy(field, dir);
			if (skip > 0)
				students = students.Skip(skip);

			if (take > 0)
				students = students.Take(take);

			return students.ToList();
		}
	}
}
