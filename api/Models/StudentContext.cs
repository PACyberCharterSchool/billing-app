using Microsoft.EntityFrameworkCore;

namespace api.Models
{
	public class StudentContext : DbContext
	{
		public StudentContext(DbContextOptions<StudentContext> options) : base(options)
		{
			this.Database.EnsureCreated();
		}
		public DbSet<Student> Students { get; set; }
	}
}
