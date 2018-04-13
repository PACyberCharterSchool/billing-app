using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Linq;

using api.Models;

namespace api.Controllers
{
	[Route("api/[controller]")]
	public class StudentsController : Controller
	{
		private readonly StudentContext _context;

		private readonly ILogger _logger;

		public StudentsController(StudentContext context, ILogger<StudentsController> logger)
		{
			_context = context;

			_logger = logger;

			if (_context.Students.Count() == 0)
			{
				_context.Students.Add(
						new Student { firstName = "Student", lastName = "Pennsylvania" }
						);
				_context.SaveChanges();
			}
		}

		[HttpGet]
		[Authorize(Policy = "STD+")]
		public IActionResult GetAll()
		{
			return new ObjectResult(_context.Students.ToList());
		}

		[HttpGet("{id}", Name = "GetStudent")]
		[Authorize(Policy = "STD+")]
		public IActionResult GetById(long id)
		{
			var student = _context.Students.FirstOrDefault(s => s.Id == id);
			if (student == null)
			{
				return NotFound();
			}
			return new ObjectResult(student);
		}

		[HttpPost]
		[Authorize(Policy = "PAY+")]
		public IActionResult Create([FromBody] Student s)
		{
			_logger.LogDebug("StudentsController.Create():  creating students {s}.");

			if (s == null)
			{
				return BadRequest();
			}

			_logger.LogDebug("StudentsController.Create():  creating students {s}.");

			_context.Students.Add(s);
			_context.SaveChanges();

			return CreatedAtRoute("GetStudent", new Student { Id = s.Id }, s);
		}

		[HttpPut("{id}")]
		[Authorize(Policy = "PAY+")]
		public IActionResult Update(int id, [FromBody]Student model)
		{
			if (!ModelState.IsValid)
			{
				return BadRequest(ModelState);
			}

			var student = _context.Students.Find(id);
			if (student == null)
			{
				return NotFound();
			}

			student.firstName = model.firstName;
			student.lastName = model.lastName;

			_context.SaveChanges();

			return Ok(student);
		}

		[HttpDelete("{id}")]
		[Authorize(Policy = "ADM=")]
		public IActionResult Delete(int id)
		{
			var student = _context.Students.Find(id);
			if (student == null)
			{
				return NotFound();
			}

			_context.Remove(student);
			_context.SaveChanges();

			return Ok(student);
		}
	}
}
