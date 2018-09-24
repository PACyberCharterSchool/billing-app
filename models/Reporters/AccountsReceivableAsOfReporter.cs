using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;

using models.Common;
using System.IO;

namespace models.Reporters
{
	public class AccountsReceivableAsOfSchoolDistrict
	{
		public int Aun { get; set; }
		public string Name { get; set; }
		public string PaymentType { get; set; }
		public decimal RegularEducationDue { get; set; }
		public decimal SpecialEducationDue { get; set; }
		public decimal TotalDue => RegularEducationDue + SpecialEducationDue;
		public decimal PaidByDistrict { get; set; }
		public decimal PaidByPDE { get; set; }
		public decimal Refunded { get; set; }
		public decimal TotalPaid => PaidByDistrict + PaidByPDE - Refunded;
		public decimal NetDue => TotalDue - TotalPaid;
	}

	public class AccountsReceivableAsOf
	{
		public string SchoolYear { get; set; }
		public DateTime AsOf { get; set; }
		public IEnumerable<AccountsReceivableAsOfSchoolDistrict> SchoolDistricts { get; set; }
	}

	public class AccountsReceivableAsOfReporter : IReporter<AccountsReceivableAsOf, AccountsReceivableAsOfReporter.Config>
	{
		private readonly PacBillContext _context;

		public AccountsReceivableAsOfReporter(PacBillContext context) => _context = context;

		private IList<int> GetAuns() => _context.SchoolDistricts.Select(d => d.Aun).ToList();

		private static IList<string> ScopesFromAsOf(DateTime asOf)
		{
			var months = Month.ByNumber();
			var firstYear = months[asOf.Month].FirstYear;
			var july = Month.July.AsDateTime(firstYear ? asOf.Year : asOf.Year - 1);

			List<string> scopes = new List<string>();
			foreach (var month in months.Values)
			{
				var year = asOf.Year;
				if (!month.FirstYear && firstYear)
					year++;
				else if (month.FirstYear && !firstYear)
					year--;

				var dt = month.AsDateTime(year);
				if (dt > july && dt <= asOf)
					scopes.Add($"{year}.{month.Number:00}");
			}

			return scopes;
		}

		private BulkInvoice Deserialize(string data)
		{
			using (var tr = new StringReader(data))
				return new JsonSerializer().Deserialize<BulkInvoice>(new JsonTextReader(tr));
		}

		private IList<AccountsReceivableAsOfSchoolDistrict> GetSchoolDistricts(
			DateTime asOf,
			string schoolYear,
			IList<int> auns = null)
		{
			if (auns == null)
				auns = GetAuns();

			var scopes = ScopesFromAsOf(asOf);
			var invoices = _context.Reports.
				Where(r => scopes.Contains(r.Scope)).
				Where(r => r.Type == ReportType.BulkInvoice || r.Type == ReportType.Invoice).
				Select(r => Deserialize(r.Data)).
				OrderByDescending(i => i.Prepared).
				ToList();

			var districts = new Dictionary<int, BulkInvoiceSchoolDistrict>();
			foreach (var i in invoices)
			{
				var dd = i.Districts.Where(d => auns.Contains(d.SchoolDistrict.Aun));
				foreach (var d in dd)
				{
					if (!districts.ContainsKey(d.SchoolDistrict.Aun))
						districts.Add(d.SchoolDistrict.Aun, d);
				}
			}

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

			return districts.Values.OrderBy(d => d.SchoolDistrict.Name).Select(d =>
			{
				var check = 0m;
				var unipay = 0m;
				if (payments.ContainsKey(d.SchoolDistrict.Aun))
				{
					var pp = payments[d.SchoolDistrict.Aun];
					check = pp.Where(p => p.Type == PaymentType.Check).Sum(p => p.Amount);
					unipay = pp.Where(p => p.Type == PaymentType.UniPay).Sum(p => p.Amount);
				}

				var refund = 0m;
				if (refunds.ContainsKey(d.SchoolDistrict.Aun))
					refund = refunds[d.SchoolDistrict.Aun].Sum(r => r.Amount);

				return new AccountsReceivableAsOfSchoolDistrict
				{
					Aun = d.SchoolDistrict.Aun,
					Name = d.SchoolDistrict.Name,
					PaymentType = d.SchoolDistrict.PaymentType,
					RegularEducationDue = d.SchoolDistrict.RegularRate * ((decimal)d.RegularEnrollments.Values.Sum() / 12),
					SpecialEducationDue = d.SchoolDistrict.SpecialRate * ((decimal)d.SpecialEnrollments.Values.Sum() / 12),
					PaidByDistrict = check,
					PaidByPDE = unipay,
					Refunded = refund,
				};
			}).ToList();
		}

		public class Config
		{
			public string SchoolYear { get; set; }
			public DateTime AsOf { get; set; }
			public IList<int> Auns { get; set; }
		}

		public AccountsReceivableAsOf GenerateReport(Config config)
		{
			var report = new AccountsReceivableAsOf
			{
				SchoolYear = config.SchoolYear,
				AsOf = config.AsOf,
				SchoolDistricts = GetSchoolDistricts(config.AsOf, config.SchoolYear, config.Auns),
			};

			return report;
		}
	}
}
