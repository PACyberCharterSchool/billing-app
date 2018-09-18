using System;
using System.Collections.Generic;
using System.Linq;

using models.Common;

namespace models.Reporters
{
	public class BulkStudentInformationStudent : InvoiceStudent
	{
		public string SchoolDistrictName { get; set; }
	}

	public class BulkStudentInformation
	{
		public IEnumerable<BulkStudentInformationStudent> Students { get; set; }
	}

	public class BulkStudentInformationReporter : IReporter<BulkStudentInformation, BulkStudentInformationReporter.Config>
	{
		private readonly PacBillContext _context;

		public BulkStudentInformationReporter(PacBillContext context) => _context = context;

		private IList<BulkStudentInformationStudent> GetInvoiceStudents(
			string scope,
			DateTime start,
			DateTime end,
			IList<int> auns = null)
		{
			var headerId = _context.StudentRecordsHeaders.Where(h => h.Scope == scope).Select(h => h.Id).Single();

			var records = _context.StudentRecords.Where(r => r.Header.Id == headerId);
			if (auns != null && auns.Count > 0)
				records = records.Where(r => auns.Contains(r.SchoolDistrictId));

			return records.Enrolled(start, end).
				OrderBy(r => r.SchoolDistrictName).
				ThenBy(r => r.StudentLastName).
				ThenBy(r => r.StudentFirstName).
				ThenBy(r => r.StudentMiddleInitial).
				ThenBy(r => r.StudentEnrollmentDate).
				ThenBy(r => r.StudentWithdrawalDate).
				Select(r => new BulkStudentInformationStudent
				{
					SchoolDistrictAun = r.SchoolDistrictId,
					SchoolDistrictName = r.SchoolDistrictName,
					PASecuredID = r.StudentPaSecuredId,
					PACyberID = r.StudentId,
					FirstName = r.StudentFirstName,
					MiddleInitial = r.StudentMiddleInitial,
					LastName = r.StudentLastName,
					Street1 = r.StudentStreet1,
					Street2 = r.StudentStreet2,
					City = r.StudentCity,
					State = r.StudentState,
					ZipCode = r.StudentZipCode,
					DateOfBirth = r.StudentDateOfBirth,
					Grade = r.StudentGradeLevel,
					FirstDay = r.StudentEnrollmentDate,
					LastDay = r.StudentWithdrawalDate,
					IsSpecialEducation = r.StudentIsSpecialEducation,
					CurrentIep = r.StudentCurrentIep,
					FormerIep = r.StudentFormerIep,
				}).ToList();
		}

		public class Config
		{
			public string Scope { get; set; }
			public DateTime AsOf { get; set; }
			public IList<int> Auns { get; set; }
		}

		private string SchoolYearFromScope(string scope)
		{
			var year = int.Parse(scope.Substring(0, 4));
			var month = int.Parse(scope.Substring(5, 2));

			if (Month.ByNumber()[month].FirstYear)
				return $"{year}-{year + 1}";
			else
				return $"{year - 1}-{year}";
		}

		public BulkStudentInformation GenerateReport(Config config)
		{
			var result = new BulkStudentInformation { };

			var schoolYear = SchoolYearFromScope(config.Scope);
			var calendar = _context.Calendars.SingleOrDefault(c => c.SchoolYear == schoolYear);
			if (calendar == null)
				throw new MissingCalendarException(ReportType.BulkStudentInformation, schoolYear);

			var firstDay = calendar.Days.Single(d => d.SchoolDay == 1).Date;

			result.Students = GetInvoiceStudents(
				config.Scope,
				firstDay,
				config.AsOf.EndOfMonth(),
				config.Auns
			);
			return result;
		}
	}
}
