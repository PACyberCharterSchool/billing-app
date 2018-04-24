using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.Converters;

namespace api.Models
{
	public class PacBillContext : DbContext
	{
		public PacBillContext(DbContextOptions<PacBillContext> options) : base(options) { }

		public DbSet<Student> Students { get; set; }
		public DbSet<SchoolDistrict> SchoolDistricts { get; set; }
		public DbSet<PendingStudentStatusRecord> PendingStudentStatusRecords { get; set; }
		public DbSet<CommittedStudentStatusRecord> CommittedStudentStatusRecords { get; set; }

		protected override void OnModelCreating(ModelBuilder builder)
		{
			builder.Entity<SchoolDistrict>().
				Property(e => e.Rate).
				HasDefaultValue(0m);

			builder.Entity<SchoolDistrict>().
				Property(e => e.PaymentType).
				HasDefaultValue(SchoolDistrictPaymentType.ACH);
		}
	}
}
