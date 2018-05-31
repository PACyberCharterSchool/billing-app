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

		private static decimal CalculateAmountDue(Enrollments enrollments, decimal rate)
		{
			var sum = 0;
			foreach (var month in _months)
			{
				var property = typeof(Enrollments).GetProperty(month.Name);
				sum += (int)property.GetValue(enrollments);
			}

			return Decimal.Round((sum * rate) / 12, 2, MidpointRounding.ToEven);
		}

		private class Payment
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

		private static decimal CalculatePaid(dynamic transactions, PaymentType type)
		{
			const string key = "Payment";

			var payments = new List<Payment>();
			foreach (var month in _months)
			{
				if (!transactions.ContainsKey(month.Name) || transactions[month.Name] == null)
					continue;

				var transaction = transactions[month.Name] as State;
				if (!transaction.ContainsKey(key) || transaction[key] == null)
					continue;

				var payment = transaction[key] as Payment;
				if (payment.Type != type.Value)
					continue;

				payments.Add(transaction[key] as Payment);
			}

			return Decimal.Round(payments.Sum(p => p.Amount), 2, MidpointRounding.ToEven);
		}

		private static decimal CalculateRefunded(dynamic transactions)
		{
			const string key = "Refund";

			var refunds = new List<decimal>();
			foreach (var month in _months)
			{
				if (!transactions.ContainsKey(month.Name) || transactions[month.Name] == null)
					continue;

				var transaction = transactions[month.Name] as State;
				if (!transaction.ContainsKey(key) || transaction[key] == null)
					continue;

				refunds.Add(transaction[key]);
			}

			return Decimal.Round(refunds.Sum(), 2, MidpointRounding.ToEven);
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
			const string regularEnrollmentsKey = "RegularEnrollments";
			const string regularRateKey = "RegularRate";
			const string dueForRegularKey = "DueForRegular";
			const string specialEnrollmentsKey = "SpecialEnrollments";
			const string specialRateKey = "SpecialRate";
			const string dueForSpecialKey = "DueForSpecial";
			const string transactionsKey = "Transactions";
			const string paidByCheckKey = "PaidByCheck";
			const string paidByUniPayKey = "PaidByUniPay";
			const string totalDueKey = "TotalDue";
			const string totalPaidKey = "TotalPaid";

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
				(regularEnrollmentsKey, GetEnrollments(_conn, asOf,
					aun: Input<Config>(i => i.SchoolDistrictAun),
					firstYear: Reference(s => s[firstYearKey]),
					secondYear: Reference(s => s[secondYearKey]),
					isSpecial: false)
				),
				(regularRateKey, Reference(s => s[schoolDistrictKey].RegularRate)),
				(dueForRegularKey,
					Lambda((Enrollments enrollments, decimal rate) => CalculateAmountDue(enrollments, rate),
					 	Reference(s => s[regularEnrollmentsKey]),
					 	Reference(s => s[regularRateKey])
					)
				),
				(specialEnrollmentsKey, GetEnrollments(_conn, asOf,
					aun: Input<Config>(i => i.SchoolDistrictAun),
					firstYear: Reference(s => s[firstYearKey]),
					secondYear: Reference(s => s[secondYearKey]),
					isSpecial: true)
				),
				(specialRateKey, Reference(s => s[schoolDistrictKey].SpecialRate)),
				(dueForSpecialKey,
					Lambda((Enrollments enrollments, decimal rate) => CalculateAmountDue(enrollments, rate),
						Reference(s => s[specialEnrollmentsKey]),
						Reference(s => s[specialRateKey])
					)
				),
				(totalDueKey,
					Lambda((decimal regular, decimal special) => Decimal.Round(regular + special, 2, MidpointRounding.ToEven),
						Reference(s => s[dueForRegularKey]),
						Reference(s => s[dueForSpecialKey])
					)
				),
				(transactionsKey, GetTransactions(_conn, asOf,
					schoolDistrictId: Reference(s => s[schoolDistrictKey].Id),
					schoolYear: Input<Config>(i => i.SchoolYear),
					firstYear: Reference(s => s[firstYearKey]),
					secondYear: Reference(s => s[secondYearKey]))
				),
				(paidByCheckKey,
					Lambda((dynamic transactions) => CalculatePaid(transactions, PaymentType.Check),
						Reference(s => s[transactionsKey])
					)
				),
				(paidByUniPayKey,
					Lambda((dynamic transactions) => CalculatePaid(transactions, PaymentType.UniPay),
						Reference(s => s[transactionsKey])
					)
				),
				(totalPaidKey,
					Lambda((decimal check, decimal unipay) => Decimal.Round(check + unipay, 2, MidpointRounding.ToEven),
						Reference(s => s[paidByCheckKey]),
						Reference(s => s[paidByUniPayKey])
					)
				),
				("Refunded",
					Lambda((dynamic transactions) => CalculateRefunded(transactions),
						Reference(s => s[transactionsKey])
					)
				),
				// TODO(Erik): less refunded?
				("NetDue",
					Lambda((decimal due, decimal paid) => Decimal.Round(due - paid, 2, MidpointRounding.ToEven),
						Reference(s => s[totalDueKey]),
						Reference(s => s[totalPaidKey])
					)
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
