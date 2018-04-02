using Microsoft.EntityFrameworkCore;

namespace api.Models
{
	public class StudentContext : DbContext
	{
		public StudentContext(DbContextOptions<StudentContext> options) : base(options) { }

		public DbSet<Student> Students { get; set; }
	}
}
