using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;

using static models.Common.PropertyMerger;

namespace models
{
	public interface IStudentRepository
	{
		Student CreateOrUpdate(DateTime time, Student update);
		Student CreateOrUpdate(Student update);

		Student Create(Student create);
		Student Update(Student update);

		Student Get(int id);
		Student GetByPACyberId(string id);

		IList<Student> GetMany(string sort = "Id", SortDirection dir = null, int skip = 0, int take = 0, string filter = null);
	}

	public class StudentRepository : IStudentRepository
	{
		private readonly DbSet<Student> _students;
		private readonly IFilterParser _parser;
		private readonly ILogger<StudentRepository> _logger;

		public StudentRepository(PacBillContext context, IFilterParser parser, ILogger<StudentRepository> logger)
		{
			_students = context.Students;
			_parser = parser;
			_logger = logger;
		}

		private static readonly IList<string> _excludedFields = new List<string>
		{
			nameof(Student.Id),
			nameof(Student.PACyberId),
			nameof(Student.Created),
			nameof(Student.LastUpdated),
		};

		public Student CreateOrUpdate(DateTime time, Student update)
		{
			var student = _students.FirstOrDefault(d => d.PACyberId == update.PACyberId);
			if (student == null)
			{
				update.Created = time;
				update.LastUpdated = time;

				_students.Add(update);
				return update;
			}

			MergeProperties(student, update, _excludedFields);
			student.LastUpdated = time;
			_students.Update(student);

			return student;
		}

		public Student CreateOrUpdate(Student student) => CreateOrUpdate(DateTime.Now, student);

		public Student Create(Student create)
		{
			DateTime now = DateTime.Now;

			create.Created = now;
			create.LastUpdated = now;
			_students.Add(create); 

			return create;
		}	

		public Student Update(Student update)
		{
			DateTime now = DateTime.Now;

			update.LastUpdated = now;

			_students.Update(update);

			return update;
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
