using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace models
{
	public class PacBillContext : DbContext
	{
		public PacBillContext(DbContextOptions<PacBillContext> options) : base(options) { }

		public DbSet<AuditRecord> AuditRecords { get; set; }
		public DbSet<Calendar> Calendars { get; set; }
		public DbSet<Payment> Payments { get; set; }
		public DbSet<Refund> Refunds { get; set; }
		public DbSet<Report> Reports { get; set; }
		public DbSet<Template> Templates { get; set; }
		public DbSet<SchoolDistrict> SchoolDistricts { get; set; }
		public DbSet<DigitalSignature> DigitalSignatures { get; set; }
		public DbSet<StudentRecord> StudentRecords { get; set; }
		public DbSet<StudentRecordsHeader> StudentRecordsHeaders { get; set; }

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

			builder.Entity<Report>().
				HasIndex(e => e.Name).
				IsUnique();

			builder.Entity<Report>().
				Property(e => e.Type).
				HasConversion(
					v => v.Value,
					v => ReportType.FromString(v)
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

			builder.Entity<DigitalSignature>().
				HasIndex(s => s.Title).
				IsUnique();

			builder.Entity<StudentRecordsHeader>().
				HasIndex(e => e.Scope).
				IsUnique();

			builder.Entity<StudentRecordsHeader>().
				HasMany(h => h.Records).
				WithOne(r => r.Header).
				OnDelete(DeleteBehavior.Cascade);
		}
	}
}
