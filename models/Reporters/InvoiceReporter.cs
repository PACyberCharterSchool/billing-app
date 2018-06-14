using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

using Dapper;

namespace models.Reporters
{
	public class InvoiceSchoolDistrict
	{
		public int Id { get; set; }
		public int Aun { get; set; }
		public string Name { get; set; }
		public decimal RegularRate { get; set; }
		public decimal SpecialRate { get; set; }
	}

	public class InvoiceEnrollments
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

	public class InvoicePayment
	{
		public string Type { get; set; }
		public string CheckNumber { get; set; }
		public decimal? CheckAmount { get; set; }
		public decimal? UniPayAmount { get; set; }
		public DateTime Date { get; set; }
	}

	public class InvoiceTransaction
	{
		public InvoicePayment Payment { get; set; }
		public decimal? Refund { get; set; }
	}

	public class InvoiceTransactions
	{
		public InvoiceTransaction July { get; set; }
		public InvoiceTransaction August { get; set; }
		public InvoiceTransaction September { get; set; }
		public InvoiceTransaction October { get; set; }
		public InvoiceTransaction November { get; set; }
		public InvoiceTransaction December { get; set; }
		public InvoiceTransaction January { get; set; }
		public InvoiceTransaction February { get; set; }
		public InvoiceTransaction March { get; set; }
		public InvoiceTransaction April { get; set; }
		public InvoiceTransaction May { get; set; }
		public InvoiceTransaction June { get; set; }
	}

	public class InvoiceStudent
	{
		// TODO(Erik): what do we display if null?
		public ulong? PASecuredID { get; set; }
		public string FirstName { get; set; }
		public string MiddleInitial { get; set; }
		public string LastName { get; set; }
		public string FullName =>
			$"{LastName}, {FirstName}{(string.IsNullOrEmpty(MiddleInitial) ? "" : $" {MiddleInitial}")}";
		public string Street1 { get; set; }
		public string Street2 { get; set; }
		public string Address1 => $"{Street1}{(string.IsNullOrEmpty(Street2) ? "" : $" {Street2}")}";
		public string City { get; set; }
		public string State { get; set; }
		public string ZipCode { get; set; }
		public string Address2 => $"{City}, {State} {ZipCode}";
		public DateTime DateOfBirth { get; set; }
		public string Grade { get; set; }
		public DateTime FirstDay { get; set; }
		public DateTime? LastDay { get; set; }
		public bool IsSpecialEducation { get; set; }
		public DateTime? CurrentIep { get; set; }
		public DateTime? FormerIep { get; set; }
	}

	public class Invoice
	{
		public string Number { get; set; } // TODO(Erik): auto-increment
		public string SchoolYear { get; set; }
		public int FirstYear => int.Parse(SchoolYear.Split("-")[0]);
		public int SecondYear => int.Parse(SchoolYear.Split("-")[1]);
		public DateTime AsOf { get; set; }
		public string AsOfMonth => AsOf.ToString("MMMM");
		public int AsOfYear => AsOf.Year;
		public DateTime Prepared { get; set; }
		public DateTime ToSchoolDistrict { get; set; }
		public DateTime ToPDE { get; set; }
		public InvoiceSchoolDistrict SchoolDistrict { get; set; }
		public InvoiceEnrollments RegularEnrollments { get; set; }
		public InvoiceEnrollments SpecialEnrollments { get; set; }
		public InvoiceTransactions Transactions { get; set; }
		public IList<InvoiceStudent> Students { get; set; }
	}

	public class InvoiceReporter : IReporter<Invoice, InvoiceReporter.Config>
	{
		private readonly IDbConnection _conn;

		public InvoiceReporter(PacBillContext context) => _conn = context.Database.GetDbConnection();

		private InvoiceSchoolDistrict GetSchoolDistrict(int aun) =>
			_conn.Query<InvoiceSchoolDistrict>(@"
				SELECT
					Id,
					Aun,
					Name,
					COALESCE(AlternateRate, Rate) AS RegularRate,
					COALESCE(AlternateSpecialEducationRate, SpecialEducationRate) AS SpecialRate
				FROM SchoolDistricts
				WHERE Aun = @Aun",
				new { Aun = aun }).Single();

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

		private static bool IsBeforeAsOf(DateTime asOf, int month) =>
			month >= 7 && month >= asOf.Month || month < 7 && month <= asOf.Month;

		private static DateTime EndOfMonth(int year, int month) =>
			new DateTime(year, month, DateTime.DaysInMonth(year, month));

		private InvoiceEnrollments GetEnrollments(
			DateTime asOf,
			int aun,
			int firstYear,
			int secondYear,
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

			var args = new Dictionary<string, object> {
				{"Aun", aun},
				{"IsSpecial", isSpecial},
			};

			foreach (var month in _months)
			{
				if (!IsBeforeAsOf(asOf, month.Number))
				{
					sb.AppendLine($"0 AS {month.Name}, ");
					continue;
				}

				sb.AppendLine($"({EnrollmentCount(month.Name)}) AS {month.Name}, ");

				var year = month.Number >= 7 ? firstYear : secondYear;
				var startDate = new DateTime(year, month.Number, 1);
				var endDate = EndOfMonth(year, month.Number);

				args.Add(month.Name, startDate);
				args.Add($"End{month.Name}", endDate);
			}

			var query = sb.ToString().Trim().TrimEnd(',');

			return _conn.Query<InvoiceEnrollments>(query, args).SingleOrDefault();
		}

		private InvoiceTransactions GetTransactions(
			DateTime asOf,
			int schoolDistrictId,
			string schoolYear,
			int firstYear,
			int secondYear)
		{
			var transactions = new InvoiceTransactions();
			foreach (var month in _months)
			{
				var property = typeof(InvoiceTransactions).GetProperty(month.Name);
				if (!IsBeforeAsOf(asOf, month.Number))
				{
					property.SetValue(transactions, new InvoiceTransaction());
					continue;
				}

				var year = month.Number >= 7 ? firstYear : secondYear;
				var startDate = new DateTime(year, month.Number, 1);
				var endDate = EndOfMonth(year, month.Number);

				var args = new
				{
					SchoolDistrictId = schoolDistrictId,
					SchoolYear = schoolYear,
					StartDate = startDate,
					EndDate = endDate,
				};

				property.SetValue(transactions, new InvoiceTransaction
				{
					Payment = _conn.Query<InvoicePayment>($@"
						SELECT
							Type,
							ExternalId AS CheckNumber,
							CASE Type
								WHEN '{PaymentType.Check.Value}' THEN Amount
								ELSE NULL
							END AS CheckAmount,
							CASE Type
								WHEN '{PaymentType.UniPay.Value}' THEN Amount
								ELSE NULL
							END	AS UniPayAmount,
							Date
						FROM Payments
						WHERE SchoolDistrictId = @SchoolDistrictId
						AND SchoolYear = @SchoolYear
						AND (Date >= @StartDate AND Date <= @EndDate)",
						args).SingleOrDefault(),
					Refund = _conn.Query<decimal?>(@"
						SELECT Amount
						From Refunds
						WHERE SchoolDistrictId = @SchoolDistrictId
						AND SchoolYear = @SchoolYear
						AND (Date >= @StartDate AND Date <= @EndDate)",
						args).SingleOrDefault(),
				});
			}

			return transactions;
		}

		private IList<InvoiceStudent> GetStudents(
			int aun,
			DateTime start,
			DateTime end)
		{
			return _conn.Query<InvoiceStudent>($@"
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
				new
				{
					Aun = aun,
					Start = start,
					End = end,
				}).ToList();
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
		public Invoice GenerateReport(Config config)
		{
			var invoice = new Invoice
			{
				Number = config.InvoiceNumber,
				SchoolYear = config.SchoolYear,
				AsOf = config.AsOf,
				Prepared = config.Prepared,
				ToSchoolDistrict = config.ToSchoolDistrict,
				ToPDE = config.ToPDE,
			};

			invoice.SchoolDistrict = GetSchoolDistrict(config.SchoolDistrictAun);

			invoice.RegularEnrollments = GetEnrollments(
				config.AsOf,
				config.SchoolDistrictAun,
				invoice.FirstYear,
				invoice.SecondYear,
				isSpecial: false);
			invoice.SpecialEnrollments = GetEnrollments(
				config.AsOf,
				config.SchoolDistrictAun,
				invoice.FirstYear,
				invoice.SecondYear,
				isSpecial: true);

			invoice.Transactions = GetTransactions(
				config.AsOf,
				invoice.SchoolDistrict.Id,
				config.SchoolYear,
				invoice.FirstYear,
				invoice.SecondYear);

			invoice.Students = GetStudents(
				config.SchoolDistrictAun,
				new DateTime(invoice.FirstYear, 7, 1),
				EndOfMonth(config.AsOf.Year, config.AsOf.Month));

			return invoice;
		}
	}
}
