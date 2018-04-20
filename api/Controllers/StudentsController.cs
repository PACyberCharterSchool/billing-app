using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
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
			[StudentField]
			public string Sort { get; set; }

			[RegularExpression("^(?:a|de)sc$")]
			public string Dir { get; set; }

			[Range(0, int.MaxValue)]
			public int Skip { get; set; }

			[Range(0, int.MaxValue)]
			public int Take { get; set; }

			public string Filter { get; set; }
		}

		[HttpGet]
		[Authorize(Policy = "STD+")]
		[ProducesResponseType(typeof(StudentsResponse), 200)]
		public async Task<IActionResult> GetMany([FromQuery]GetManyArgs args)
		{
			if (!ModelState.IsValid)
				return new BadRequestObjectResult(new ErrorsResponse(ModelState));

			IList<Student> students = null;
			try
			{
				students = await Task.Run(() => _students.GetMany(
					sort: args.Sort,
					dir: args.Dir,
					skip: args.Skip,
					take: args.Take,
					filter: args.Filter
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
		[ProducesResponseType(typeof(StudentResponse), 200)]
		public async Task<IActionResult> GetById(int id)
		{
			var student = await Task.Run(() => _students.Get(id));
			if (student == null)
				return NotFound();

			return new ObjectResult(new StudentResponse(student));
		}
	}
}
