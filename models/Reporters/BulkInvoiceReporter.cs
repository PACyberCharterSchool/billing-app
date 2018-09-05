using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace models.Reporters
{
	public class BulkInvoiceSchoolDistrict
	{
		public InvoiceSchoolDistrict SchoolDistrict { get; set; }
		public InvoiceEnrollments RegularEnrollments { get; set; }
		public InvoiceEnrollments SpecialEnrollments { get; set; }
		public InvoiceTransactions Transactions { get; set; }
		public IEnumerable<InvoiceStudent> Students { get; set; }
	}

	public class BulkInvoice
	{
		public string SchoolYear { get; set; }
		public int FirstYear => int.Parse(SchoolYear.Split("-")[0]);
		public int SecondYear => int.Parse(SchoolYear.Split("-")[1]);
		public DateTime AsOf { get; set; }
		public string AsOfMonth => AsOf.ToString("MMMM");
		public int AsOfYear => AsOf.Year;
		public string Scope { get; set; }
		public string ScopeMonth => new DateTime(DateTime.Now.Year, int.Parse(Scope.Substring(5, 2)), 1).ToString("MMMM");
		public int ScopeYear => int.Parse(Scope.Substring(0, 4));
		public DateTime Prepared { get; set; }
		public DateTime ToSchoolDistrict { get; set; }
		public DateTime ToPDE { get; set; }
		public IEnumerable<BulkInvoiceSchoolDistrict> Districts { get; set; }
	}

	public class BulkInvoiceReporter : IReporter<BulkInvoice, BulkInvoiceReporter.Config>
	{
		private readonly PacBillContext _context;

		public BulkInvoiceReporter(PacBillContext context)
		{
			_context = context;
		}

		private IList<InvoiceSchoolDistrict> GetInvoiceSchoolDistricts()
			=> _context.SchoolDistricts.
				OrderBy(d => d.Name).
				Select(d => new InvoiceSchoolDistrict
				{
					Id = d.Id,
					Aun = d.Aun,
					Name = d.Name,
					RegularRate = d.AlternateRate != null ? d.AlternateRate.Value : d.Rate,
					SpecialRate = d.AlternateSpecialEducationRate != null ?
						d.AlternateSpecialEducationRate.Value :
						d.SpecialEducationRate,
				}).ToList();

		// TODO(Erik): filter by school district aun
		private IList<InvoiceStudent> GetInvoiceStudents(int[] auns, string scope, DateTime start, DateTime end)
		{
			var headerId = _context.StudentRecordsHeaders.Where(h => h.Scope == scope).Select(h => h.Id).Single();

			return _context.StudentRecords.
				Where(r => r.Header.Id == headerId).
				Where(r => auns.Contains(r.SchoolDistrictId)).
				Where(r => r.StudentEnrollmentDate <= end && (
					r.StudentWithdrawalDate == null || (
						r.StudentWithdrawalDate >= start && (
							r.StudentWithdrawalDate != r.StudentEnrollmentDate || (
								r.StudentWithdrawalDate == r.StudentEnrollmentDate &&
								r.StudentCurrentIep.Value.Month == r.StudentEnrollmentDate.Month &&
								r.StudentCurrentIep.Value.Day == r.StudentEnrollmentDate.Day
							)
						)
					)
				)).
				OrderBy(r => r.StudentLastName).
				ThenBy(r => r.StudentFirstName).
				ThenBy(r => r.StudentMiddleInitial).
				ThenBy(r => r.StudentEnrollmentDate).
				ThenBy(r => r.StudentWithdrawalDate).
				Select(r => new InvoiceStudent
				{
					SchoolDistrictAun = r.SchoolDistrictId,
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

		private IDictionary<int, InvoiceTransactions> GetInvoiceTransactions(
			int[] auns,
			string schoolYear,
			int firstYear,
			int secondYear,
			DateTime asOf)
		{
			var payments = _context.Payments.
				Where(p => p.SchoolYear == schoolYear).
				Select(p => new InvoicePayment
				{
					Type = p.Type.Value,
					CheckNumber = p.ExternalId,
					CheckAmount = p.Type == PaymentType.Check ? (decimal?)p.Amount : null,
					UniPayAmount = p.Type == PaymentType.UniPay ? (decimal?)p.Amount : null,
					Date = p.Date,
				}).ToList();

			var refunds = _context.Refunds.
				Where(r => r.SchoolYear == schoolYear).
				Select(r => new
				{
					Amount = r.Amount,
					Date = r.Date,
				}).ToList();

			var result = new Dictionary<int, InvoiceTransactions>();
			foreach (var aun in auns)
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
					var start = new DateTime(year, month.Number, 1);
					var end = EndOfMonth(year, month.Number);

					property.SetValue(transactions, new InvoiceTransaction
					{
						Payment = payments.SingleOrDefault(p => p.Date >= start && p.Date <= end),
						Refund = refunds.
							Where(r => r.Date >= start && r.Date <= end).
							Select(r => (decimal?)r.Amount).
							SingleOrDefault(),
					});
				}

				result.Add(aun, transactions);
			}

			return result;
		}

		bool AreEnrollmentAndWithdrawalInSameMonth(IGrouping<string, InvoiceStudent> group)
		{
			// this assumes to never run in a group where there is a single entry...
			var activityDates = group.OrderBy(s => s.FirstDay).Select(s => new { s.FirstDay, s.LastDay }).ToList();
			for (int i = 1; i < activityDates.Count(); i++)
			{
				if (activityDates[i].FirstDay.Month == activityDates[i - 1].LastDay.Value.Month)
				{
					return true;
				}
			}
			return false;
		}

		private Dictionary<int, (InvoiceEnrollments Regular, InvoiceEnrollments Special)> GetInvoiceEnrollments(
			int[] auns,
			IList<InvoiceStudent> allStudents,
			int firstYear,
			int secondYear,
			DateTime asOf)
		{
			var result = new Dictionary<int, (InvoiceEnrollments Regular, InvoiceEnrollments Special)>();
			foreach (var aun in auns)
			{
				var regularEnrollments = new InvoiceEnrollments();
				var specialEnrollments = new InvoiceEnrollments();

				foreach (var month in _months)
				{
					var regularCount = 0;
					var specialCount = 0;

					var year = month.Number >= 7 ? firstYear : secondYear;
					var start = new DateTime(year, month.Number, 1);

					DateTime end;
					if (new[] { 7, 8, 9 }.Contains(month.Number))
						end = EndOfMonth(year, 9);
					else
						end = EndOfMonth(year, month.Number);

					if (end > EndOfMonth(asOf.Year, asOf.Month) && end.Month != 9)
					{
						regularEnrollments[month.Name] = regularCount;
						specialEnrollments[month.Name] = specialCount;
						continue;
					}

					var students = allStudents.Where(s => s.SchoolDistrictAun == aun);
					var groups = students.Where(s =>
						s.FirstDay <= end && (
							(s.LastDay == null || s.LastDay >= start) ||
							(end.Month == 9 && (s.LastDay == null || (s.LastDay.Value.Month >= 7 || s.LastDay.Value.Month <= 9)))
						)
					).GroupBy(s => s.PACyberID);

					foreach (var group in groups)
					{
						if (group.Count() == 1)
						{
							if (group.Single().IsSpecialEducation)
								specialCount++;
							else
								regularCount++;

							continue;
						}

						if (group.All(s => s.IsSpecialEducation) && !AreEnrollmentAndWithdrawalInSameMonth(group))
						{
							specialCount++;
							continue;
						}

						regularCount++;
					}

					regularEnrollments[month.Name] = regularCount;
					specialEnrollments[month.Name] = specialCount;
				}

				result.Add(aun, (regularEnrollments, specialEnrollments));
			}

			return result;
		}

		private IList<BulkInvoiceSchoolDistrict> GetBulkInvoiceSchoolDistricts(BulkInvoice bulk)
		{
			var districts = GetInvoiceSchoolDistricts();
			var auns = districts.Select(d => d.Aun).ToArray();

			var students = GetInvoiceStudents(
				auns,
				bulk.Scope,
				new DateTime(bulk.FirstYear, 7, 1),
				EndOfMonth(bulk.AsOf.Year, bulk.AsOf.Month));
			var transactions = GetInvoiceTransactions(
				auns,
				bulk.SchoolYear,
				bulk.FirstYear,
				bulk.SecondYear,
				bulk.AsOf);
			var enrollments = GetInvoiceEnrollments(
				auns,
				students,
				bulk.FirstYear,
				bulk.SecondYear,
				bulk.AsOf);

			var result = new List<BulkInvoiceSchoolDistrict>();
			foreach (var district in districts)
				result.Add(new BulkInvoiceSchoolDistrict
				{
					SchoolDistrict = district,
					RegularEnrollments = enrollments[district.Aun].Regular,
					SpecialEnrollments = enrollments[district.Aun].Special,
					Transactions = transactions[district.Aun],
					Students = students.Where(s => s.SchoolDistrictAun == district.Aun),
				});

			return result;
		}

		public class Config
		{
			public string SchoolYear { get; set; }
			public string Scope { get; set; }
			public DateTime Prepared { get; set; }
			public DateTime AsOf { get; set; }
			public DateTime ToSchoolDistrict { get; set; }
			public DateTime ToPDE { get; set; }
		}

		public BulkInvoice GenerateReport(Config config)
		{
			var bulk = new BulkInvoice
			{
				SchoolYear = config.SchoolYear,
				AsOf = config.AsOf,
				Scope = config.Scope,
				Prepared = config.Prepared,
				ToSchoolDistrict = config.ToSchoolDistrict,
				ToPDE = config.ToPDE,
			};

			bulk.Districts = GetBulkInvoiceSchoolDistricts(bulk);

			return bulk;
		}
	}
}
