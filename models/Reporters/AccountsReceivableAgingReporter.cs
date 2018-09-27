using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;

namespace models.Reporters
{
	public class AccountsReceivableAgingTransaction
	{
		public string Identifier { get; set; }
		public string Type { get; set; }
		public DateTime Date { get; set; }
		public decimal? Amount { get; set; }
		public decimal? WriteOff { get; set; }
		public IList<decimal?> Buckets { get; set; }
	}

	public class AccountsReceivableAgingSchoolDistrict
	{
		public int Aun { get; set; }
		public string Name { get; set; }
		public IList<AccountsReceivableAgingTransaction> Transactions { get; set; }
	}

	public class AccountsReceivableAging
	{
		public DateTime? From { get; set; }
		public IList<AccountsReceivableAgingSchoolDistrict> SchoolDistricts { get; set; }
		public IList<decimal?> BucketTotals { get; set; }
		public decimal Balance { get; set; }
	}

	public class AccountsReceivableAgingReporter : IReporter<AccountsReceivableAging, AccountsReceivableAgingReporter.Config>
	{
		private readonly PacBillContext _context;
		public AccountsReceivableAgingReporter(PacBillContext context) => _context = context;

		public class Config
		{
			public DateTime? From { get; set; }
			public DateTime AsOf { get; set; }
			public IList<int> Auns { get; set; }
		}

		private IList<int> GetAuns() => _context.SchoolDistricts.Select(d => d.Aun).ToList();

		private IList<BulkInvoice> GetInvoices(DateTime? from)
		{
			var invoices = _context.Reports.Where(r => r.Type == ReportType.BulkInvoice || r.Type == ReportType.Invoice);
			if (from.HasValue)
				invoices = invoices.Where(r => r.Created >= from.Value);

			return invoices.
				Select(r => JsonConvert.DeserializeObject<BulkInvoice>(r.Data)).
				OrderByDescending(i => i.Scope).
				ThenByDescending(i => i.Prepared).
				ToList();
		}

		private IDictionary<int, List<Payment>> GetPayments(DateTime? from, IList<int> auns)
		{
			var payments = _context.Payments.Where(p => auns.Contains(p.SchoolDistrict.Aun));
			if (from.HasValue)
				payments = payments.Where(p => p.Date >= from.Value);

			return payments.
				GroupBy(p => p.SchoolDistrict.Aun).
				ToDictionary(g => g.Key, g => g.ToList());
		}

		private IDictionary<int, List<Refund>> GetRefunds(DateTime? from, IList<int> auns)
		{
			var refunds = _context.Refunds.Where(r => auns.Contains(r.SchoolDistrict.Aun));
			if (from.HasValue)
				refunds = refunds.Where(r => r.Date >= from.Value);

			return refunds.
				GroupBy(r => r.SchoolDistrict.Aun).
				ToDictionary(g => g.Key, g => g.ToList());
		}

		private IList<AccountsReceivableAgingSchoolDistrict> GetSchoolDistricts(DateTime? from, IList<int> auns = null)
		{
			if (auns == null)
				auns = GetAuns();

			var invoices = GetInvoices(from);
			var districts = new Dictionary<int, List<BulkInvoiceSchoolDistrict>>();
			foreach (var i in invoices)
			{
				var dd = i.Districts.Where(d => auns.Contains(d.SchoolDistrict.Aun));
				foreach (var d in dd)
				{
					if (!districts.ContainsKey(d.SchoolDistrict.Aun))
						districts.Add(d.SchoolDistrict.Aun, new List<BulkInvoiceSchoolDistrict> { d });
					else
						districts[d.SchoolDistrict.Aun].Add(d);
				}
			}

			var payments = GetPayments(from, auns);
			var refunds = GetRefunds(from, auns);

			var results = new List<AccountsReceivableAgingSchoolDistrict>();
			foreach (var dd in districts)
			{
				var transactions = new List<AccountsReceivableAgingTransaction>();
				foreach (var d in dd.Value)
				{
					// TODO(Erik): all the things
				}

				results.Add(new AccountsReceivableAgingSchoolDistrict
				{
					Aun = dd.Key,
					Name = dd.Value[0].SchoolDistrict.Name,
				});
			}

			return results;
		}

		public AccountsReceivableAging GenerateReport(Config config)
		{
			var report = new AccountsReceivableAging
			{
				From = config.From,
				SchoolDistricts = GetSchoolDistricts(config.From, config.Auns),
			};

			// TODO(Erik): BucketTotals from Transactions

			report.Balance = report.BucketTotals.Sum(t => t.HasValue ? t.Value : 0);

			return report;
		}
	}
}
