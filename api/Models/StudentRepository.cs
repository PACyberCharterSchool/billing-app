using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;

namespace api.Models
{
	public interface IStudentRepository
	{
		Student Get(int id);
		IList<Student> GetMany();
	}

	public class StudentRepository : IStudentRepository
	{
		private readonly DbSet<Student> _students;

		public StudentRepository(PacBillContext ctx)
		{
			_students = ctx.Students;
		}

		public Student Get(int id) => _students.Where(s => s.Id == id).SingleOrDefault();

		public IList<Student> GetMany() => _students.ToList();
	}
}
