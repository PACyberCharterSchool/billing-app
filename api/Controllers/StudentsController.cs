using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Threading.Tasks;

using api.Models;

namespace api.Controllers
{
	[Route("api/[controller]")]
	public class StudentsController : Controller
	{
		private readonly IStudentRepository _students;
		private readonly ILogger _logger;

		public StudentsController(IStudentRepository students, ILogger<StudentsController> logger)
		{
			_students = students;
			_logger = logger;
		}

		public struct StudentsResponse
		{
			public IList<Student> Students { get; }

			public StudentsResponse(IList<Student> students)
			{
				Students = students;
			}
		}

		public class GetManyArgs
		{
			public string Field { get; set; }
			public string Dir { get; set; }
			public int Skip { get; set; }
			public int Take { get; set; }
		}

		[HttpGet]
		[Authorize(Policy = "STD+")]
		public async Task<IActionResult> GetMany([FromQuery]GetManyArgs args)
		{
			var students = await Task.Run(() => _students.GetMany(
				field: args.Field,
				dir: args.Dir,
				skip: args.Skip,
				take: args.Take
			));
			if (students == null)
				students = new List<Student>();

			return new ObjectResult(new StudentsResponse(students));
		}

		public struct StudentResponse
		{
			public Student Student { get; }

			public StudentResponse(Student student)
			{
				Student = student;
			}
		}

		[HttpGet("{id}")]
		[Authorize(Policy = "STD+")]
		public async Task<IActionResult> GetById(int id)
		{
			var student = await Task.Run(() => _students.Get(id));
			if (student == null)
				return NotFound();

			return new ObjectResult(new StudentResponse(student));
		}
	}
}
