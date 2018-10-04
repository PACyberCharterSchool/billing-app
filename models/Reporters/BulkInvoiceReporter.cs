using Microsoft.EntityFrameworkCore;
using models.Common;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace models.Reporters
{
	public class InvoiceSchoolDistrict
	{
		public int Id { get; set; }
		public int Aun { get; set; }
		public string Name { get; set; }
		public string PaymentType { get; set; }
		public decimal RegularRate { get; set; }
		public decimal SpecialRate { get; set; }
	}

	public class InvoiceEnrollments : Dictionary<string, int> { }

	public class InvoicePayment
	{
		public string Type { get; set; }
		public string CheckNumber { get; set; }
		public decimal? CheckAmount { get; set; }
		public decimal? UniPayAmount { get; set; }
		public string Date { get; set; }
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

		public IDictionary<string, InvoiceTransaction> AsDictionary()
		{
			return new Dictionary<string, InvoiceTransaction>
			{
				{"July", July},
				{"August", August},
				{"September", September},
				{"October", October},
				{"November", November},
				{"December", December},
				{"January", January},
				{"February", February},
				{"March", March},
				{"April", April},
				{"May", May},
				{"June", June},
			};
		}

		public IEnumerable<InvoiceTransaction> AsEnumerable()
		{
			return new[] {
				July, August, September, October, November, December,
				January, February, March, April, May, June,
			};
		}
	}

	public class InvoiceStudent
	{
		public int SchoolDistrictAun { get; set; }
		public ulong? PASecuredID { get; set; }
		public string PACyberID { get; set; }
		public string FirstName { get; set; }
		public string MiddleInitial { get; set; }
		public string LastName { get; set; }
		public string FullName
			=> $"{LastName}, {FirstName}{(string.IsNullOrEmpty(MiddleInitial) ? "" : $" {MiddleInitial}")}";
		public string Street1 { get; set; }
		public string Street2 { get; set; }
		public string Address1 => $"{Street1}";
		public string City { get; set; }
		public string State { get; set; }
		public string ZipCode { get; set; }
		public string CityStateZipCode => $"{City}, {State} {ZipCode}";
		public string Address2 => $"{(string.IsNullOrEmpty(Street2) ? $"{CityStateZipCode}" : $"{Street2}")}";
		public string Address3 => $"{(string.IsNullOrEmpty(Street2) ? "" : $"{CityStateZipCode}")}";
		public DateTime DateOfBirth { get; set; }
		public string Grade { get; set; }
		public DateTime FirstDay { get; set; }
		public DateTime? LastDay { get; set; }
		public bool IsSpecialEducation { get; set; }
		public DateTime? CurrentIep { get; set; }
		public DateTime? FormerIep { get; set; }
	}

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
		public string Scope { get; set; }
		public DateTime ScopeDateTime => new DateTime(int.Parse(Scope.Substring(0, 4)), int.Parse(Scope.Substring(5, 2)), 1);
		public string ScopeMonth => ScopeDateTime.ToString("MMMM");
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

		private IList<InvoiceSchoolDistrict> GetInvoiceSchoolDistricts(
			IList<int> auns = null,
			SchoolDistrictPaymentType paymentType = null)
		{
			var dd = _context.SchoolDistricts.AsQueryable();
			if (auns != null && auns.Count > 0)
				dd = dd.Where(d => auns.Contains(d.Aun));

			if (paymentType != null)
				dd = dd.Where(d => d.PaymentType == paymentType);

			return dd.OrderBy(d => d.Name).
				Select(d => new InvoiceSchoolDistrict
				{
					Id = d.Id,
					Aun = d.Aun,
					Name = d.Name,
					PaymentType = d.PaymentType.Value,
					RegularRate = d.AlternateRate != null ? d.AlternateRate.Value : d.Rate,
					SpecialRate = d.AlternateSpecialEducationRate != null ?
						d.AlternateSpecialEducationRate.Value :
						d.SpecialEducationRate,
				}).ToList();
		}

		private IList<InvoiceStudent> GetInvoiceStudents(IList<int> auns, string scope, DateTime start, DateTime end)
		{
			var headerId = _context.StudentRecordsHeaders.Where(h => h.Scope == scope).Select(h => h.Id).Single();

			return _context.StudentRecords.
				Where(r => r.Header.Id == headerId).
				Where(r => auns.Contains(r.SchoolDistrictId)).
				Enrolled(start, end).
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

		private IDictionary<int, InvoiceTransactions> GetInvoiceTransactions(
			IList<int> auns,
			string schoolYear,
			int firstYear,
			int secondYear,
			DateTime period)
		{
			var payments = _context.Payments.
				Where(p => auns.Contains(p.SchoolDistrict.Aun)).
				Where(p => p.SchoolYear == schoolYear).
				GroupBy(p => p.SchoolDistrict.Aun).
				ToDictionary(g => g.Key, g => g.ToList());

			var refunds = _context.Refunds.
				Where(r => auns.Contains(r.SchoolDistrict.Aun)).
				Where(r => r.SchoolYear == schoolYear).
				GroupBy(r => r.SchoolDistrict.Aun).
				ToDictionary(g => g.Key, g => g.ToList());

			var result = new Dictionary<int, InvoiceTransactions>();
			foreach (var aun in auns)
			{
				var transactions = new InvoiceTransactions();
				foreach (var month in Month.AsEnumerable())
				{
					var property = typeof(InvoiceTransactions).GetProperty(month.Name);
					if (period.IsBefore(month))
					{
						property.SetValue(transactions, new InvoiceTransaction());
						continue;
					}

					var year = month.FirstYear ? firstYear : secondYear;
					var start = month.AsDateTime(year);
					var end = start.EndOfMonth();

					var transaction = new InvoiceTransaction();
					if (payments.ContainsKey(aun))
					{
						var districtPayments = payments[aun].
							Where(p => p.Date >= start && p.Date <= end).
							ToList();

						if (districtPayments.Count > 0)
						{
							transaction.Payment = new InvoicePayment
							{
								Type = districtPayments[0].Type.Value,
								CheckNumber = String.Join("\n", districtPayments.Select(p => p.ExternalId)),
								CheckAmount = districtPayments.Where(p => p.Type == PaymentType.Check).Sum(p => p.Amount).Round(),
								UniPayAmount = districtPayments.Where(p => p.Type == PaymentType.UniPay).Sum(p => p.Amount).Round(),
								Date = String.Join("\n", districtPayments.Select(p => p.Date.ToString("M/d/yyyy"))),
							};

							if (transaction.Payment.CheckAmount == 0)
								transaction.Payment.CheckAmount = null;

							if (transaction.Payment.UniPayAmount == 0)
								transaction.Payment.UniPayAmount = null;
						}
					}

					if (refunds.ContainsKey(aun))
					{
						var districtRefunds = refunds[aun].
							Where(r => r.Date >= start && r.Date <= end).
							ToList();
						if (districtRefunds.Count > 0)
							transaction.Refund = districtRefunds.Sum(r => (decimal?)r.Amount);
					}

					property.SetValue(transactions, transaction);
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
			IList<int> auns,
			IList<InvoiceStudent> allStudents,
			int firstYear,
			int secondYear,
			DateTime period)
		{
			var result = new Dictionary<int, (InvoiceEnrollments Regular, InvoiceEnrollments Special)>();
			foreach (var aun in auns)
			{
				var regularEnrollments = new InvoiceEnrollments();
				var specialEnrollments = new InvoiceEnrollments();

				foreach (var month in Month.AsEnumerable())
				{
					var regularCount = 0;
					var specialCount = 0;

					var year = month.Number >= 7 ? firstYear : secondYear;
					var start = new DateTime(year, month.Number, 1);

					DateTime end;
					if (new[] { 7, 8, 9 }.Contains(month.Number))
						end = new DateTime(year, 9, 1).EndOfMonth();
					else
						end = new DateTime(year, month.Number, 1).EndOfMonth();

					if (end > period && end.Month != 9)
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

		private IList<BulkInvoiceSchoolDistrict> GetBulkInvoiceSchoolDistricts(
			BulkInvoice bulk,
			IList<int> auns = null,
			SchoolDistrictPaymentType paymentType = null)
		{
			var districts = GetInvoiceSchoolDistricts(auns, paymentType);
			auns = districts.Select(d => d.Aun).ToList();

			var calendar = _context.Calendars.SingleOrDefault(c => c.SchoolYear == bulk.SchoolYear);
			if (calendar == null)
				throw new MissingCalendarException(ReportType.BulkInvoice, bulk.SchoolYear);

			var firstDay = calendar.Days.Single(d => d.SchoolDay == 1).Date;

			var period = bulk.ScopeDateTime.EndOfMonth();
			var students = GetInvoiceStudents(
				auns,
				bulk.Scope,
				firstDay,
				period);
			var transactions = GetInvoiceTransactions(
				auns,
				bulk.SchoolYear,
				bulk.FirstYear,
				bulk.SecondYear,
				period);
			var enrollments = GetInvoiceEnrollments(
				auns,
				students,
				bulk.FirstYear,
				bulk.SecondYear,
				period);

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
			public DateTime ToSchoolDistrict { get; set; }
			public DateTime ToPDE { get; set; }
			public IList<int> Auns { get; set; }
			public SchoolDistrictPaymentType PaymentType { get; set; }
		}

		public BulkInvoice GenerateReport(Config config)
		{
			var bulk = new BulkInvoice
			{
				SchoolYear = config.SchoolYear,
				Scope = config.Scope,
				Prepared = config.Prepared,
				ToSchoolDistrict = config.ToSchoolDistrict,
				ToPDE = config.ToPDE,
			};

			bulk.Districts = GetBulkInvoiceSchoolDistricts(bulk, config.Auns, config.PaymentType);

			return bulk;
		}
	}
}
