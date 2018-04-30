using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.Converters;

namespace models
{
	public class PacBillContext : DbContext
	{
		public PacBillContext(DbContextOptions<PacBillContext> options) : base(options) { }

		public DbSet<AuditRecord> AuditRecords { get; set; }
		public DbSet<CommittedStudentStatusRecord> CommittedStudentStatusRecords { get; set; }
		public DbSet<PendingStudentStatusRecord> PendingStudentStatusRecords { get; set; }
		public DbSet<SchoolDistrict> SchoolDistricts { get; set; }
		public DbSet<Student> Students { get; set; }
		public DbSet<StudentActivityRecord> StudentActivityRecords { get; set; }

		protected override void OnModelCreating(ModelBuilder builder)
		{
			builder.Entity<SchoolDistrict>().
				HasIndex(e => e.Aun).
				IsUnique();

			builder.Entity<SchoolDistrict>().
				Property(e => e.Rate).
				HasDefaultValue(0m);

			builder.Entity<SchoolDistrict>().
				Property(e => e.PaymentType).
				HasDefaultValue(SchoolDistrictPaymentType.ACH);

			builder.Entity<Student>().
				HasIndex(s => s.PACyberId).
				IsUnique();
		}
	}
}
