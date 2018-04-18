using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
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
			if (!string.IsNullOrWhiteSpace(args.Field) && !Student.IsValidField(args.Field))
				return new BadRequestObjectResult(new ErrorResponse($"Invalid sort field '{args.Field}'."));

			if (!string.IsNullOrWhiteSpace(args.Dir) && (args.Dir != "asc" && args.Dir != "desc"))
				return new BadRequestObjectResult(new ErrorResponse($"Sort direction must be 'asc' or 'desc'; was '{args.Dir}'."));

			if (args.Skip < 0)
				return new BadRequestObjectResult(new ErrorResponse($"Skip must be 0 or greater; was '{args.Skip}'."));

			if (args.Take < 0)
				return new BadRequestObjectResult(new ErrorResponse($"Take must be 0 or greater; was '{args.Take}'."));

			IList<Student> students = null;
			try
			{
				students = await Task.Run(() => _students.GetMany(
					field: args.Field,
					dir: args.Dir,
					skip: args.Skip,
					take: args.Take
				));
			}
			catch (ArgumentException e)
			{
				return new BadRequestObjectResult(new ErrorResponse(e.Message));
			}

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
