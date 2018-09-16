using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;

using models.Common;

namespace models.Reporters
{
	public class AccountsReceivableAsOfSchoolDistrict
	{
		public int Aun { get; set; }
		public string Name { get; set; }
		public string PaymentType { get; set; }
		public decimal RegularEducationDue { get; set; }
		public decimal SpecialEducationDue { get; set; }
		// TODO(Erik): maybe use formulas for this stuff
		public decimal TotalDue => RegularEducationDue + SpecialEducationDue;
		public decimal PaidByDistrict { get; set; }
		public decimal PaidByPDE { get; set; }
		public decimal Refunded { get; set; }
		public decimal TotalPaid => (PaidByDistrict + PaidByPDE) - Refunded;
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

				// TODO(Erik): returning too many scopes
				if (month.AsDateTime(year) > july)
					scopes.Add($"{year}.{month.Number:00}");
			}

			return scopes;
		}

		private IList<AccountsReceivableAsOfSchoolDistrict> GetSchoolDistricts(DateTime asOf, IList<int> auns = null)
		{
			if (auns == null)
				auns = GetAuns();

			var scopes = ScopesFromAsOf(asOf);
			var invoices = _context.Reports.
				Where(r => (r.Type == ReportType.BulkInvoice || r.Type == ReportType.Invoice) && scopes.Contains(r.Scope)).
				Select(r => JsonConvert.DeserializeObject<BulkInvoice>(r.Data)).
				OrderByDescending(i => i.Prepared);

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

			return districts.Values.OrderBy(d => d.SchoolDistrict.Name).Select(d => new AccountsReceivableAsOfSchoolDistrict
			{
				Aun = d.SchoolDistrict.Aun,
				Name = d.SchoolDistrict.Name,
				PaymentType = d.SchoolDistrict.PaymentType,
				RegularEducationDue = d.SchoolDistrict.RegularRate * (d.RegularEnrollments.Values.Sum() / 12),
				SpecialEducationDue = d.SchoolDistrict.SpecialRate * (d.SpecialEnrollments.Values.Sum() / 12),
				PaidByDistrict = d.Transactions.CheckTotalPaid,
				PaidByPDE = d.Transactions.UniPayTotalPaid,
				Refunded = d.Transactions.TotalRefunded,
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
				SchoolDistricts = GetSchoolDistricts(config.AsOf, config.Auns),
			};

			return report;
		}
	}
}
