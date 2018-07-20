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

				DateTime endDate;
				if (new[] {7, 8, 9}.Contains(month.Number)) {
					endDate = EndOfMonth(year, 9);
				}
				else {
					endDate = EndOfMonth(year, month.Number);
				}

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

    private InvoiceStudent GetStudentInvoiceEntryWithLatestWithdrawalDate(List<InvoiceStudent> list, ulong studentID)
    {
      int index = list.FindIndex(i => i.LastDay == null);

      if (index >= 0) {
        return list[index];
      }

      List<InvoiceStudent> sorted = list.OrderBy(i => i.LastDay.Value).ToList();

      return list[list.Count - 1];
    }

		private IList<InvoiceStudent> FilterForAdditionalTraits(IList<InvoiceStudent>studentList)
		{
			List<InvoiceStudent> newList = new List<InvoiceStudent>();

			// when a student record has an enrollment date that matches the withdrawal date, and
			// the IEP enrollment month and day match the month and day of the enrollment and
			// withdrawal dates, the record should be counted.
			newList = studentList.Where(s => {
				if (s.LastDay.HasValue) {
					return true;
				}
				else if (s.FirstDay == s.LastDay) {
					if (s.CurrentIep.Value.Month == s.FirstDay.Month &&
						s.CurrentIep.Value.Month == s.LastDay.Value.Month &&
						s.CurrentIep.Value.Day == s.FirstDay.Day &&
						s.CurrentIep.Value.Day == s.LastDay.Value.Day) {
						return true;
					}
					else {
						return false;
					}
				}

				return true;
			}).ToList();

			newList = FilterByActivityDatesAndSPEDStatus(newList).ToList();

			// newList = FilterByActivityDatesAndEnrollmentStatus(newList).ToList();

			return newList;	
		}

		private IList<InvoiceStudent> FilterByActivityDatesAndSPEDStatus(IList<InvoiceStudent>list)
		{
			// when a student has multiple record entries, and the records have enrollment
			// dates or withdrawal dates in the same month, only count the record if the SPED
			// column has the value of "NO"

			List<InvoiceStudent> newList = new List<InvoiceStudent>();

      // if the list of students are all unique, then we don't need to do the filter
      if (!list.GroupBy(i => i.PASecuredID).Any(c => c.Count() > 1)) {
        return list;
      }

			var result = list.GroupBy(i => i.PASecuredID);
			foreach (var group in result) {
				if (group.Count() > 1) {
					// multiple records for the student with the PASecuredID
					var item = group.ToList().Find(i => !i.IsSpecialEducation);
					if (item == null) {
						// no special education status, so we should just add all of the records to the new list
						newList.AddRange(group.ToList());
					}
					else {
						foreach (var i in group) {
							if (!i.LastDay.HasValue) {
								newList.Add(i);
								continue;
							}

							if (i.FirstDay.Month == i.LastDay.Value.Month && !i.IsSpecialEducation) {
								newList.Add(i);
							}
							else if (i.FirstDay.Month != i.LastDay.Value.Month) {
								newList.Add(i);
							}
						}
					}
				}
				else {
					// there's only one record for this given student with the relevant PASecuredID.  just pass it through.
					newList.Add(group.First());
				}
			}

			return newList.OrderBy(i => i.LastName).ToList();
		}

    private IList<InvoiceStudent> FilterByActivityDatesAndEnrollmentStatus(IList<InvoiceStudent>studentList)
    {
			// when a student has multiple record entries, and the enrollment and withdrawal
			// dates are in the same month, and the SPED values of the records are the same
			// value (i.e. both "YES" or both "NO"), then those records should only be counted as 1 

      List<InvoiceStudent> newList = new List<InvoiceStudent>();

      // if the list of students are all unique, then we don't need to do the filter
      if (!studentList.GroupBy(i => i.PASecuredID).Any(c => c.Count() > 1)) {
        return studentList;
      }

      foreach (var invoiceStudent in studentList) {
        IList<InvoiceStudent> subList = studentList.Where(s => s.PASecuredID == invoiceStudent.PASecuredID).ToList();
        if (subList.Count > 1) {
          // check whether student is special education and whether the month value for enrollment and withdrawal are
          // the same

          IList<InvoiceStudent> subSubList = subList.Where(s => {
            if (!s.LastDay.HasValue) {
              return true;
            }

            // you can't dereference anything from a nullable variable type...
            /* DateTime d = s.LastDay ?? DateTime.Now; */
            if (s.IsSpecialEducation == invoiceStudent.IsSpecialEducation && s.FirstDay.Month == s.LastDay.Value.Month) {
              return true;
            }
            return false;
          }).ToList();

          if (subSubList.Count == subList.Count) {
            int index = newList.FindIndex(i => i.PASecuredID == invoiceStudent.PASecuredID);
            if (index < 0 && invoiceStudent.PASecuredID.HasValue) {
              newList.Add(GetStudentInvoiceEntryWithLatestWithdrawalDate(subSubList.ToList(), invoiceStudent.PASecuredID.Value));
            }
          }
        }
        else {
          newList.Add(invoiceStudent);
          continue;
        }
      }

      return newList;
    }

		private IList<InvoiceStudent> GetStudents(
			int aun,
			DateTime start,
			DateTime end)
		{
			if (new[] {7, 8, 9}.Contains(end.Month)) {
				end = new DateTime(end.Year, 9, end.Day);
			}

      IList<InvoiceStudent> studentList = _conn.Query<InvoiceStudent>($@"
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
						StudentWithdrawaldate >= @Start
						AND StudentWithdrawalDate >= StudentEnrollmentDate
					)
				)
				ORDER BY StudentLastName, StudentFirstName, StudentMiddleInitial",
				new
				{
					Aun = aun,
					Start = start,
					End = end,
				}).ToList();

			return FilterForAdditionalTraits(studentList);
      // return studentList;
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
