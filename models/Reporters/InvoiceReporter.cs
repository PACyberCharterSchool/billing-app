using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

using models.Reporters.Generators;
using static models.Reporters.Generators.Generator;

namespace models.Reporters
{
	public class InvoiceReporter
	{
		private readonly IDbConnection _conn;

		public InvoiceReporter(PacBillContext context)
		{
			_conn = context.Database.GetDbConnection();
		}

		public class SchoolDistrict
		{
			public int Id { get; set; }
			public int Aun { get; set; }
			public string Name { get; set; }
			public decimal RegularRate { get; set; }
			public decimal SpecialRate { get; set; }
		}

		private static GeneratorFunc GetSchoolDistrict(IDbConnection conn, GeneratorFunc aun) =>
			SqlObject<SchoolDistrict>(conn, @"
				SELECT
					Id,
					Aun,
					Name,
					COALESCE(AlternateRate, Rate) AS RegularRate,
					COALESCE(AlternateSpecialEducationRate, SpecialEducationRate) AS SpecialRate
				FROM SchoolDistricts
				WHERE Aun = @Aun",
				("Aun", aun));

		private static readonly List<(string Name, int Number)> _months = new List<(string Name, int Number)>{
				("July", 7),
				("August", 8),
				("September", 9),
				("October", 10),
				("November", 11),
				("December", 12),
				("January", 1),
				("February", 2),
				("March", 3),
				("April", 4),
				("May", 5),
				("June", 6),
			};

		public class Enrollments
		{
			public int July { get; set; }
			public int August { get; set; }
			public int September { get; set; }
			public int October { get; set; }
			public int November { get; set; }
			public int December { get; set; }
			public int January { get; set; }
			public int February { get; set; }
			public int March { get; set; }
			public int April { get; set; }
			public int May { get; set; }
			public int June { get; set; }
		}

		private static bool IsBeforeAsOf(DateTime asOf, int month) =>
			month >= 7 && month >= asOf.Month || month < 7 && month <= asOf.Month;

		private static DateTime EndOfMonth(int year, int month) =>
			new DateTime(year, month, DateTime.DaysInMonth(year, month));

		private static GeneratorFunc GetEnrollments(
			IDbConnection conn,
			DateTime asOf,
			GeneratorFunc aun,
			GeneratorFunc firstYear,
			GeneratorFunc secondYear,
			bool isSpecial)
		{
			string EnrollmentCount(string month) =>
				$@"SELECT COUNT(*)
					FROM CommittedStudentStatusRecords
					WHERE SchoolDistrictId = @Aun
					AND StudentIsSpecialEducation = @IsSpecial
					AND StudentEnrollmentDate <= @End{month}
					AND (
						StudentWithdrawalDate IS NULL
						OR (
							StudentEnrollmentDate != StudentWithdrawalDate
							AND StudentWithdrawalDate >= @{month}
						)
					)";

			var sb = new StringBuilder();
			sb.AppendLine("SELECT ");

			var args = new List<(string Key, GeneratorFunc Generator)> {
				("Aun", aun),
				("IsSpecial", Constant(isSpecial)),
			};

			foreach (var month in _months)
			{
				if (!IsBeforeAsOf(asOf, month.Number))
				{
					sb.AppendLine($"0 AS {month.Name}, ");
					continue;
				}

				sb.AppendLine($"({EnrollmentCount(month.Name)}) AS {month.Name}, ");

				var startDate = Lambda((string year) => new DateTime(int.Parse(year), month.Number, 1),
					month.Number >= 7 ? firstYear : secondYear);

				var endDate = Lambda((string year) => EndOfMonth(int.Parse(year), month.Number),
					month.Number >= 7 ? firstYear : secondYear);

				args.Add((month.Name, startDate));
				args.Add(($"End{month.Name}", endDate));
			}

			var query = sb.ToString().Trim().TrimEnd(',');

			return SqlObject<Enrollments>(conn, query, args.ToArray());
		}

		public class Payment
		{
			public string Type { get; set; }
			public string CheckNumber { get; set; }
			public decimal Amount { get; set; }
			public DateTime Date { get; set; }
		}

		private static GeneratorFunc GetTransactions(
			IDbConnection conn,
			DateTime asOf,
			GeneratorFunc schoolDistrictId,
			GeneratorFunc schoolYear,
			GeneratorFunc firstYear,
			GeneratorFunc secondYear)
		{
			var generators = new List<(string Key, GeneratorFunc Generator)>();
			foreach (var month in _months)
			{
				if (!IsBeforeAsOf(asOf, month.Number))
				{
					generators.Add((month.Name, Object(
						("Payment", Constant<Payment>(null)),
						("Refund", Constant(0m))
					)));
					continue;
				}

				var startDate = Lambda((string year) => new DateTime(int.Parse(year), month.Number, 1),
					month.Number >= 7 ? firstYear : secondYear);

				var endDate = Lambda((string year) => EndOfMonth(int.Parse(year), month.Number),
					month.Number >= 7 ? firstYear : secondYear);

				generators.Add((month.Name, Object(
					("Payment", SqlObject<Payment>(conn, @"
						SELECT Type, ExternalId AS CheckNumber, Amount, Date
						FROM Payments
						WHERE SchoolDistrictId = @SchoolDistrictId
						AND SchoolYear = @SchoolYear
						AND (Date >= @StartDate AND Date <= @EndDate)",
						("SchoolDistrictId", schoolDistrictId),
						("SchoolYear", schoolYear),
						("StartDate", startDate),
						("EndDate", endDate)
					)),
					("Refund", SqlObject<decimal>(conn, @"
						SELECT Amount
						From Refunds
						WHERE SchoolDistrictId = @SchoolDistrictId
						AND SchoolYear = @SchoolYear
						AND (Date >= @StartDate AND Date <= @EndDate)",
						("SchoolDistrictId", schoolDistrictId),
						("SchoolYear", schoolYear),
						("StartDate", startDate),
						("EndDate", endDate)
					))
				)));
			}

			return Object(generators.ToArray());
		}

		public class Student
		{
			// TODO(Erik): what do we display if null?
			public ulong? PASecuredID { get; set; }
			public string FirstName { get; set; }
			public string MiddleInitial { get; set; }
			public string LastName { get; set; }
			public string Street1 { get; set; }
			public string Street2 { get; set; }
			public string City { get; set; }
			public string State { get; set; }
			public string ZipCode { get; set; }
			public DateTime DateOfBirth { get; set; }
			public string Grade { get; set; }
			public DateTime FirstDay { get; set; }
			public DateTime? LastDay { get; set; }
			public bool IsSpecialEducation { get; set; }
			public DateTime? CurrentIep { get; set; }
			public DateTime? FormerIep { get; set; }
		}

		private static GeneratorFunc GetStudents(
			IDbConnection conn,
			GeneratorFunc aun,
			GeneratorFunc start,
			GeneratorFunc end)
		{
			return SqlList<Student>(conn, $@"
				SELECT
					StudentPASecuredId AS PASecuredId,
					StudentFirstName AS FirstName,
					StudentMiddleInitial AS MiddleInitial,
					StudentLastName AS LastName,
					StudentStreet1 AS Street1,
					StudentStreet2 AS Street2,
					StudentCity AS City,
					StudentState AS State,
					StudentZipCode AS ZipCode,
					StudentDateOfBirth AS DateOfBirth,
					StudentGradeLevel AS Grade,
					StudentEnrollmentDate AS FirstDay,
					StudentWithdrawalDate AS LastDay,
					StudentIsSpecialEducation AS IsSpecialEducation,
					StudentCurrentIep AS CurrentIep,
					StudentFormerIep AS FormerIep
				FROM CommittedStudentStatusRecords
				WHERE SchoolDistrictId = @Aun
				AND StudentEnrollmentDate <= @End
				AND (
					StudentWithdrawalDate IS NULL
					OR (
						StudentWithdrawalDate != StudentEnrollmentDate
						AND StudentWithdrawalDate >= @Start
					)
				)
				ORDER BY StudentLastName, StudentFirstName, StudentMiddleInitial",
				("Aun", aun),
				("Start", start),
				("End", end)
			);
		}

		public class Config
		{
			public string InvoiceNumber { get; set; }
			public string SchoolYear { get; set; }
			public DateTime AsOf { get; set; }
			public DateTime Prepared { get; set; }
			public DateTime ToSchoolDistrict { get; set; }
			public DateTime ToPDE { get; set; }
			public int SchoolDistrictAun { get; set; }
		}

		// TODO(Erik): signature
		private GeneratorFunc BuildGenerator(DateTime asOf)
		{
			const string firstYearKey = "FirstYear";
			const string secondYearKey = "SecondYear";
			const string schoolDistrictKey = "SchoolDistrict";

			return Object(
				("Number", Input<Config>(i => i.InvoiceNumber)),
				("SchoolYear", Input<Config>(i => i.SchoolYear)),
				(firstYearKey,
					Lambda((string year) => year.Split("-")[0], Input<Config>(i => i.SchoolYear))
				),
				(secondYearKey,
					Lambda((string year) => year.Split("-")[1], Input<Config>(i => i.SchoolYear))
				),
				("AsOf", Input<Config>(i => i.AsOf.Date)),
				("Prepared", Input<Config>(i => i.Prepared)),
				("ToSchoolDistrict", Input<Config>(i => i.ToSchoolDistrict)),
				("ToPDE", Input<Config>(i => i.ToPDE)),
				(schoolDistrictKey, GetSchoolDistrict(_conn, aun: Input<Config>(i => i.SchoolDistrictAun))),
				("RegularEnrollments", GetEnrollments(_conn, asOf,
					aun: Input<Config>(i => i.SchoolDistrictAun),
					firstYear: Reference(s => s[firstYearKey]),
					secondYear: Reference(s => s[secondYearKey]),
					isSpecial: false)
				),
				("RegularRate", Reference(s => s[schoolDistrictKey].RegularRate)),
				("SpecialEnrollments", GetEnrollments(_conn, asOf,
					aun: Input<Config>(i => i.SchoolDistrictAun),
					firstYear: Reference(s => s[firstYearKey]),
					secondYear: Reference(s => s[secondYearKey]),
					isSpecial: true)
				),
				("SpecialRate", Reference(s => s[schoolDistrictKey].SpecialRate)),
				("Transactions", GetTransactions(_conn, asOf,
					schoolDistrictId: Reference(s => s[schoolDistrictKey].Id),
					schoolYear: Input<Config>(i => i.SchoolYear),
					firstYear: Reference(s => s[firstYearKey]),
					secondYear: Reference(s => s[secondYearKey]))
				),
				("Students", GetStudents(_conn,
					aun: Input<Config>(i => i.SchoolDistrictAun),
					start: Lambda((string year) => new DateTime(int.Parse(year), 7, 1),
						Reference(s => s[firstYearKey])
					),
					end: Lambda(() => EndOfMonth(asOf.Year, asOf.Month))
				))
			);
		}

		public dynamic GenerateReport(Config config) => BuildGenerator(config.AsOf)(input: config);
	}
}
