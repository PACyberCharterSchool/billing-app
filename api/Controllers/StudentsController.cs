using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using api.Models;
using System.Linq;
using Microsoft.Extensions.Logging;

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
        public IEnumerable<Student> GetAll()
        {
            return _context.Students.ToList();
        }

        [HttpGet("{id}", Name = "GetStudent")]
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
        public IActionResult Update(int id, [FromBody]Student model)
        {
            if (!ModelState.IsValid) {
                return BadRequest(ModelState);
            }

            var student = _context.Students.Find(id);
            if (student == null) {
                return NotFound();
            }

            student.firstName = model.firstName;
            student.lastName = model.lastName;

            _context.SaveChanges();

            return Ok(student);
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            var student = _context.Students.Find(id);
            if (student == null) {
                return NotFound();
            }

            _context.Remove(student);
            _context.SaveChanges();

            return Ok(student);
        }
    }
}