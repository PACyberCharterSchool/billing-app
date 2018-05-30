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

		private class SchoolDistrict
		{
			public int Id { get; set; }
			public int Aun { get; set; }
			public string Name { get; set; }
			public decimal RegularRate { get; set; }
			public decimal SpecialRate { get; set; }
		}

		private static GeneratorFunc GetSchoolDistrict(IDbConnection conn, GeneratorFunc aun) =>
			// TODO(Erik): return alternate rate instead of rate if exists
			// TODO(Erik): SpecialEducationRate
			SqlObject<SchoolDistrict>(conn, @"
				SELECT Id, Aun, Name, Rate AS RegularRate
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

		private class Enrollments
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

		private static DateTime EndOfMonth(int year, int month) =>
			new DateTime(year, month, DateTime.DaysInMonth(year, month));

		// TODO(Erik): 0 for future months
		private static GeneratorFunc GetEnrollments(
			IDbConnection conn,
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
			foreach (var month in _months)
				sb.AppendLine($"({EnrollmentCount(month.Name)}) AS {month.Name}{(month.Name != "June" ? ", " : "")}");
			var query = sb.ToString();

			var args = new List<(string Key, GeneratorFunc Generator)> {
				("Aun", aun),
				("IsSpecial", Constant(isSpecial)),
			};
			foreach (var month in _months)
			{
				args.Add((month.Name,
					Lambda((string year) => new DateTime(int.Parse(year), month.Number, 1),
						month.Number >= 7 ? firstYear : secondYear)));
				args.Add(($"End{month.Name}",
					Lambda((string year) => EndOfMonth(int.Parse(year), month.Number),
						month.Number >= 7 ? firstYear : secondYear)));
			}

			return SqlObject<Enrollments>(conn, query, args.ToArray());
		}

		private static decimal CalculateAmountDue(Enrollments enrollments, decimal rate)
		{
			var sum = 0;
			foreach (var month in _months)
			{
				var property = typeof(Enrollments).GetProperty(month.Name);
				sum += (int)property.GetValue(enrollments);
			}

			// TODO(Erik): round to nearest $0.01
			return (sum * rate) / 12;
		}

		private class Payment
		{
			public string Type { get; set; }
			public string CheckNumber { get; set; }
			public decimal Amount { get; set; }
			public DateTime Date { get; set; }
		}

		private static GeneratorFunc GetTransactions(IDbConnection conn,
			GeneratorFunc schoolDistrictId,
			GeneratorFunc schoolYear,
			GeneratorFunc firstYear,
			GeneratorFunc secondYear)
		{
			var generators = new List<(string Key, GeneratorFunc Generator)>();
			foreach (var month in _months)
			{
				var startDate = Lambda((string year) => new DateTime(int.Parse(year), month.Number, 1),
					month.Number >= 7 ? firstYear : secondYear);

				var endDate = Lambda((string year) => EndOfMonth(int.Parse(year), month.Number),
					month.Number >= 7 ? firstYear : secondYear);

				generators.Add(
					(month.Name, Object(
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
					))
				);
			}

			return Object(generators.ToArray());
		}

		private static decimal CalculatePaid(dynamic transactions, PaymentType type)
		{
			const string key = "Payment";

			var payments = new List<Payment>();
			foreach (var month in _months)
			{
				var transaction = transactions[month.Name] as State;
				if (!transaction.ContainsKey(key) || transaction[key] == null)
					continue;

				var payment = transaction[key] as Payment;
				if (payment.Type != type.Value)
					continue;

				payments.Add(transaction[key] as Payment);
			}

			return payments.Sum(p => p.Amount);
		}

		private static decimal CalculateRefunded(dynamic transactions)
		{
			const string key = "Refund";

			var refunds = new List<decimal>();
			foreach (var month in _months)
			{
				var transaction = transactions[month.Name] as State;
				if (!transaction.ContainsKey(key) || transaction[key] == null)
					continue;

				refunds.Add(transaction[key]);
			}

			return refunds.Sum();
		}

		private class Student
		{
			// TODO(Erik): what do we display if null?
			public ulong? PASecureID { get; set; }
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
				)",
				("Aun", aun),
				("Start", start),
				("End", end)
			);
		}

		public class Config
		{
			public string InvoiceNumber { get; set; }
			public string SchoolYear { get; set; }
			public DateTime Prepared { get; set; }
			public DateTime ToSchoolDistrict { get; set; }
			public DateTime ToPDE { get; set; }
			public int SchoolDistrictAun { get; set; }
		}

		// TODO(Erik): signature
		private GeneratorFunc BuildGenerator()
		{
			return Object(
				("Number", Input<Config>(i => i.InvoiceNumber)),
				("SchoolYear", Input<Config>(i => i.SchoolYear)),
				("FirstYear",
					Lambda((string year) => year.Split("-")[0], Input<Config>(i => i.SchoolYear))
				),
				("SecondYear",
					Lambda((string year) => year.Split("-")[1], Input<Config>(i => i.SchoolYear))
				),
				("Prepared", Input<Config>(i => i.Prepared)),
				("ToSchoolDistrict", Input<Config>(i => i.ToSchoolDistrict)),
				("ToPDE", Input<Config>(i => i.ToPDE)),
				("SchoolDistrict", GetSchoolDistrict(_conn, aun: Input<Config>(i => i.SchoolDistrictAun))),
				("RegularEnrollments", GetEnrollments(_conn,
					aun: Input<Config>(i => i.SchoolDistrictAun),
					firstYear: Reference(s => s["FirstYear"]),
					secondYear: Reference(s => s["SecondYear"]),
					isSpecial: false)
				),
				("RegularRate", Reference(s => s["SchoolDistrict"].RegularRate)),
				("DueForRegular",
					Lambda((Enrollments enrollments, decimal rate) => CalculateAmountDue(enrollments, rate),
					 	Reference(s => s["RegularEnrollments"]),
					 	Reference(s => s["RegularRate"])
					)
				),
				("SpecialEnrollments", GetEnrollments(_conn,
					aun: Input<Config>(i => i.SchoolDistrictAun),
					firstYear: Reference(s => s["FirstYear"]),
					secondYear: Reference(s => s["SecondYear"]),
					isSpecial: true)
				),
				("SpecialRate", Reference(s => s["SchoolDistrict"].SpecialRate)),
				("DueForSpecial",
					Lambda((Enrollments enrollments, decimal rate) => CalculateAmountDue(enrollments, rate),
						Reference(s => s["SpecialEnrollments"]),
						Reference(s => s["SpecialRate"])
					)
				),
				("TotalDue",
					Lambda((decimal regular, decimal special) => regular + special,
						Reference(s => s["DueForRegular"]),
						Reference(s => s["DueForSpecial"])
					)
				),
				("Transactions", GetTransactions(_conn,
					schoolDistrictId: Reference(s => s["SchoolDistrict"].Id),
					schoolYear: Input<Config>(i => i.SchoolYear),
					firstYear: Reference(s => s["FirstYear"]),
					secondYear: Reference(s => s["SecondYear"]))
				),
				("PaidByCheck",
					Lambda((dynamic transactions) => CalculatePaid(transactions, PaymentType.Check),
						Reference(s => s["Transactions"])
					)
				),
				("PaidByUniPay",
					Lambda((dynamic transactions) => CalculatePaid(transactions, PaymentType.UniPay),
						Reference(s => s["Transactions"])
					)
				),
				("TotalPaid",
					Lambda((decimal check, decimal unipay) => check + unipay,
						Reference(s => s["PaidByCheck"]),
						Reference(s => s["PaidByUniPay"])
					)
				),
				("Refunded",
					Lambda((dynamic transactions) => CalculateRefunded(transactions),
						Reference(s => s["Transactions"])
					)
				),
				// TODO(Erik): less refunded?
				("NetDue",
					Lambda((decimal due, decimal paid) => due - paid,
						Reference(s => s["TotalDue"]),
						Reference(s => s["TotalPaid"])
					)
				),
				("Students", GetStudents(_conn,
					aun: Input<Config>(i => i.SchoolDistrictAun),
					start: Lambda((string year) => new DateTime(int.Parse(year), 7, 1),
						Reference(s => s["FirstYear"])
					),
					// TODO(Erik): end of current month
					end: Lambda((string year) => EndOfMonth(int.Parse(year), 6),
						Reference(s => s["SecondYear"])
					)
				))
			);
		}

		public dynamic GenerateReport(Config config) => BuildGenerator()(input: config);
	}
}
