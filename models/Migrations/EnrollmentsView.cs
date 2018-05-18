using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;

using models;

namespace models.Migrations
{
	[DbContext(typeof(PacBillContext))]
	[Migration("EnrollmentsView")]
	public class EnrollmentsView : Migration
	{
		private const string VIEW_NAME = "Enrollments";

		private static string Column(string left, string right) => $"{left} as {right}";

		protected override void Up(MigrationBuilder migrationBuilder)
		{
			var columns = new[] {
				Column(nameof(CommittedStudentStatusRecord.StudentId), nameof(Enrollment.PACyberId)),
				Column(nameof(CommittedStudentStatusRecord.SchoolDistrictId), nameof(Enrollment.Aun)),
				Column(nameof(CommittedStudentStatusRecord.StudentEnrollmentDate), nameof(Enrollment.StartDate)),
				Column(nameof(CommittedStudentStatusRecord.StudentWithdrawalDate), nameof(Enrollment.EndDate)),
				Column(nameof(CommittedStudentStatusRecord.StudentIsSpecialEducation), nameof(Enrollment.IsSpecialEducation)),
			};
			migrationBuilder.Sql(
				$"CREATE VIEW {VIEW_NAME} AS " +
				$"SELECT {string.Join(", ", columns)} " +
				$"FROM {nameof(PacBillContext.CommittedStudentStatusRecords)}"
			);
		}

		protected override void Down(MigrationBuilder migrationBuilder)
		{
			migrationBuilder.Sql($"DROP VIEW {VIEW_NAME}");
		}
	}
}
