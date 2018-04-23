using Microsoft.EntityFrameworkCore;

namespace api.Models
{
	public class PacBillContext : DbContext
	{
		public PacBillContext(DbContextOptions<PacBillContext> options) : base(options) { }

		public DbSet<Student> Students { get; set; }
		public DbSet<SchoolDistrict> SchoolDistricts { get; set; }

		protected override void OnModelCreating(ModelBuilder builder)
		{
			builder.Entity<SchoolDistrict>().
				Property(d => d.Rate).
				HasDefaultValue(0m);
		}
	}
}
