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
		public IList<decimal> Totals { get; set; }
		public decimal Balance { get; set; }
	}

	public class AccountsReceivableAging
	{
		public DateTime? From { get; set; }
		public IList<AccountsReceivableAgingSchoolDistrict> SchoolDistricts { get; set; }
		public IList<decimal> GrandTotals { get; set; }
		public decimal GrandBalance { get; set; }
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
				OrderBy(p => p.Date).
				GroupBy(p => p.SchoolDistrict.Aun).
				ToDictionary(g => g.Key, g => g.ToList());
		}

		private IDictionary<int, List<Refund>> GetRefunds(DateTime? from, IList<int> auns)
		{
			var refunds = _context.Refunds.Where(r => auns.Contains(r.SchoolDistrict.Aun));
			if (from.HasValue)
				refunds = refunds.Where(r => r.Date >= from.Value);

			return refunds.
				OrderBy(r => r.Date).
				GroupBy(r => r.SchoolDistrict.Aun).
				ToDictionary(g => g.Key, g => g.ToList());
		}

		private struct InvoiceDistrict
		{
			public string Scope { get; set; }
			public DateTime Prepared { get; set; }
			public BulkInvoiceSchoolDistrict Bisd { get; set; }

			public InvoiceDistrict(string s, DateTime p, BulkInvoiceSchoolDistrict d)
			{
				Scope = s;
				Prepared = p;
				Bisd = d;
			}
		}

		private IList<AccountsReceivableAgingSchoolDistrict> GetSchoolDistricts(DateTime? from, IList<int> auns = null)
		{
			if (auns == null)
				auns = GetAuns();

			var invoices = GetInvoices(from).OrderBy(i => i.Scope).ThenBy(i => i.Prepared);

			var districts = new Dictionary<int, List<InvoiceDistrict>>();
			foreach (var i in invoices)
			{
				var dd = i.Districts.Where(d => auns.Contains(d.SchoolDistrict.Aun));
				foreach (var d in dd)
				{
					if (!districts.ContainsKey(d.SchoolDistrict.Aun))
						districts.Add(d.SchoolDistrict.Aun, new List<InvoiceDistrict> { new InvoiceDistrict(i.Scope, i.Prepared, d) });
					else
						districts[d.SchoolDistrict.Aun].Add(new InvoiceDistrict(i.Scope, i.Prepared, d));
				}
			}

			var payments = GetPayments(from, auns); // TODO(Erik): queue?
			var refunds = GetRefunds(from, auns);

			var now = DateTime.Now.Date;
			var results = new List<AccountsReceivableAgingSchoolDistrict>();
			// TODO(Erik): either next invoice or next refund
			foreach (var dd in districts)
			{
				List<Payment> pp = null;
				if (payments.ContainsKey(dd.Key))
					pp = payments[dd.Key];

				List<Refund> rr = null;
				if (refunds.ContainsKey(dd.Key))
					rr = refunds[dd.Key];

				var transactions = new List<AccountsReceivableAgingTransaction>();
				for (var i = 0; i < dd.Value.Count; i++)
				{
					var d = dd.Value[i];
					var amount = GetTotal(d.Bisd);
					if (i > 0)
						amount -= GetTotal(dd.Value[i - 1].Bisd);

					var transaction = new AccountsReceivableAgingTransaction
					{
						Identifier = d.Scope,
						Type = "INV", // TODO(Erik): consts
						Date = d.Prepared,
						Amount = amount,
					};

					var buckets = new decimal?[4];
					var bi = 3;
					if (d.Prepared >= now.LessDays(30))
						bi = 0;
					else if (d.Prepared >= now.LessDays(60) && d.Prepared <= now.LessDays(31))
						bi = 1;
					else if (d.Prepared >= now.LessDays(90) && d.Prepared <= now.LessDays(61))
						bi = 2;
					else
						bi = 3;
					buckets[bi] = amount;
					transaction.Buckets = buckets;

					transactions.Add(transaction);

					if (pp != null && pp.Count > 0)
					{
						var rem = buckets[bi];
						foreach (var p in pp)
						{
							if (rem <= 0)
								break;

							var pt = new AccountsReceivableAgingTransaction
							{
								Identifier = $"PYMT{p.Date.ToString("yyyyMMdd")}",
								Type = "PYMT", // TODO(Erik): consts
								Date = p.Date,
								Amount = -p.Amount,
							};
							var pbb = new decimal?[4];
							pbb[bi] = -p.Amount;
							pt.Buckets = pbb;

							// TODO(Erik): remove payment from list
							transactions.Add(pt);

							rem -= p.Amount;
						}
					}
				}

				// TODO(Erik): leftover payments

				var sd = new AccountsReceivableAgingSchoolDistrict
				{
					Aun = dd.Key,
					Name = dd.Value[0].Bisd.SchoolDistrict.Name,
					Transactions = transactions,
				};
				sd.Totals = new[] {
					sd.Transactions.Sum(t => t.Buckets[0] ?? 0).Round(),
					sd.Transactions.Sum(t => t.Buckets[1] ?? 0).Round(),
					sd.Transactions.Sum(t => t.Buckets[2] ?? 0).Round(),
					sd.Transactions.Sum(t => t.Buckets[3] ?? 0).Round(),
				};
				sd.Balance = sd.Totals.Sum().Round();

				results.Add(sd);
			}

			return results;

			decimal GetTotal(BulkInvoiceSchoolDistrict d)
			{
				var regular = (d.SchoolDistrict.RegularRate * ((decimal)d.RegularEnrollments.Values.Sum() / 12)).Round();
				var special = (d.SchoolDistrict.SpecialRate * ((decimal)d.SpecialEnrollments.Values.Sum() / 12)).Round();
				return (regular + special).Round();
			}
		}

		public AccountsReceivableAging GenerateReport(Config config)
		{
			var report = new AccountsReceivableAging
			{
				From = config.From,
				SchoolDistricts = GetSchoolDistricts(config.From, config.Auns),
			};

			// TODO(Erik): BucketTotals from Transactions

			// report.Balance = report.BucketTotals.Sum(t => t.HasValue ? t.Value : 0);

			return report;
		}
	}
}
