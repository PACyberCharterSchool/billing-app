using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace models
{
	public class PacBillContext : DbContext
	{
		public PacBillContext(DbContextOptions<PacBillContext> options) : base(options) { }

		public DbSet<AuditRecord> AuditRecords { get; set; }
		public DbSet<Calendar> Calendars { get; set; }
		public DbSet<CommittedStudentStatusRecord> CommittedStudentStatusRecords { get; set; }
		public DbSet<Payment> Payments { get; set; }
		public DbSet<PendingStudentStatusRecord> PendingStudentStatusRecords { get; set; }
		public DbSet<Refund> Refunds { get; set; }
		public DbSet<Template> Templates { get; set; }
		public DbSet<SchoolDistrict> SchoolDistricts { get; set; }
		public DbSet<Student> Students { get; set; }
		public DbSet<StudentActivityRecord> StudentActivityRecords { get; set; }
    public DbSet<DigitalSignature> DigitalSignatures { get; set; }

		protected override void OnModelCreating(ModelBuilder builder)
		{
			builder.Entity<Calendar>().
				HasIndex(e => e.SchoolYear).
				IsUnique();

			builder.Entity<Payment>().
				HasIndex(e => new { e.PaymentId, e.Split }).
				IsUnique();

			builder.Entity<Payment>().
				Property(e => e.Type).
				HasDefaultValue(PaymentType.Check).
				HasConversion(
					v => v.Value,
					v => PaymentType.FromString(v)
				);

			builder.Entity<Template>().
				HasIndex(e => new { e.ReportType, e.SchoolYear }).
				IsUnique();

			builder.Entity<Template>().
				Property(e => e.ReportType).
				HasConversion(
					v => v.Value,
					v => ReportType.FromString(v)
				);

			builder.Entity<SchoolDistrict>().
				HasIndex(e => e.Aun).
				IsUnique();

			builder.Entity<SchoolDistrict>().
				Property(e => e.Rate).
				HasDefaultValue(0m);

			builder.Entity<SchoolDistrict>().
				Property(e => e.PaymentType).
				HasDefaultValue(SchoolDistrictPaymentType.Ach).
				HasConversion(
					v => v.Value,
					v => SchoolDistrictPaymentType.FromString(v)
				);

			builder.Entity<Student>().
				HasIndex(s => s.PACyberId).
				IsUnique();

			builder.Entity<StudentActivityRecord>().
				Property(s => s.Activity).
				HasConversion(
					v => v.Value,
					v => StudentActivity.FromString(v)
				);

      builder.Entity<DigitalSignature>().
        HasIndex(s => s.Title).
        IsUnique();
		}
	}
}
