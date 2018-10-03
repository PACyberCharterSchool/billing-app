using System;
using System.Collections.Generic;
using System.Linq;
using models.Common;
using Newtonsoft.Json;

namespace models.Reporters
{
	public class CsiuAccount
	{
		public string Number { get; set; }
		public decimal Amount { get; set; }
		public string Description { get; set; }
	}

	public class CsiuReport
	{
		public DateTime AsOf { get; set; }
		public string SchoolYear { get; set; }
		public IEnumerable<CsiuAccount> Accounts { get; set; }
	}

	public class CsiuReporter : IReporter<CsiuReport, CsiuReporter.Config>
	{
		private readonly PacBillContext _context;

		public CsiuReporter(PacBillContext context) => _context = context;

		private IList<int> GetAuns() => _context.SchoolDistricts.Select(d => d.Aun).ToList();

		private class Amounts
		{
			public decimal Regular { get; set; }
			public decimal Special { get; set; }
			public decimal Received { get; set; }
		}

		private IList<CsiuAccount> GetAccounts(DateTime asOf, string schoolYear, IList<int> auns = null)
		{
			if (auns == null)
				auns = GetAuns();

			var first = new DateTime(asOf.Year, 1, 1);
			var invoices = _context.Reports.
				Where(r => r.Created >= first && r.Created <= asOf.AddDays(1)).
				Where(r => r.Type == ReportType.BulkInvoice || r.Type == ReportType.Invoice).
				Select(r => JsonConvert.DeserializeObject<BulkInvoice>(r.Data)).
				OrderByDescending(i => i.Prepared);

			var districts = new Dictionary<int, List<BulkInvoiceSchoolDistrict>>();
			foreach (var i in invoices)
			{
				var dd = i.Districts.Where(d => auns.Contains(d.SchoolDistrict.Aun));
				foreach (var d in dd)
				{
					var aun = d.SchoolDistrict.Aun;
					if (!districts.ContainsKey(aun))
					{
						districts.Add(aun, new List<BulkInvoiceSchoolDistrict> { d });
						continue;
					}

					if (districts[aun].Count < 2)
						districts[aun].Add(d);
				}
			}

			var latest = new Amounts();
			var previous = new Amounts();
			foreach (var d in districts)
			{
				FillAmounts(latest, d.Value[0]);

				if (d.Value.Count == 2)
					FillAmounts(previous, d.Value[1]);
			}

			latest.Regular -= previous.Regular;
			latest.Special -= previous.Special;
			latest.Received -= previous.Received;

			var month = asOf.ToString("MM");
			var year = asOf.ToString("yy");
			return new List<CsiuAccount>
			{
				new CsiuAccount
				{
					Number = "10-6944-000-000-00-000-000-000-0000",
					Amount = latest.Regular.Round(),
					Description = $"TBD - K-12 Reg Ed SD Revenue for {month}/{year}",
				},
				new CsiuAccount
				{
					Number = "10-6944-000-000-00-000-000-000-0000",
					Amount = latest.Special.Round(),
					Description = $"TBD - K-12 Spec Ed SD Revenue for {month}/{year}",
				},
				new CsiuAccount
				{
					Number = $"10-0145-000-000-00-000-000-000-{schoolYear}",
					Amount = latest.Received.Round(),
					Description = $"TBD - K-12 SD A/R for {month}/{year}",
				},
			};

			void FillAmounts(Amounts a, BulkInvoiceSchoolDistrict d)
			{
				a.Regular -= d.SchoolDistrict.RegularRate * ((decimal)d.RegularEnrollments.Values.Sum() / 12);
				a.Special -= d.SchoolDistrict.SpecialRate * ((decimal)d.SpecialEnrollments.Values.Sum() / 12);

				var tt = d.Transactions.AsDictionary().Values.Where(t => t.Payment != null || t.Refund.HasValue);
				var check = tt.
					Where(t => t.Payment != null && t.Payment.CheckAmount.HasValue).
					Sum(t => t.Payment.CheckAmount.Value);
				var unipay = tt.
					Where(t => t.Payment != null && t.Payment.UniPayAmount.HasValue).
					Sum(t => t.Payment.UniPayAmount.Value);
				var refund = tt.
					Sum(t => t.Refund.HasValue ? t.Refund.Value : 0);
				a.Received += (check + unipay) - refund;
			}
		}

		public class Config
		{
			public DateTime AsOf { get; set; }
			public IList<int> Auns { get; set; }
		}

		private string SchoolYearFromAsOf(DateTime asOf)
		{
			var m = asOf.Month;
			var y = asOf.Year;
			if (Month.ByNumber()[m].FirstYear)
				return $"{Year(y)}{Year(y + 1)}";
			else
				return $"{Year(y - 1)}{Year(y)}";

			string Year(int i) => i.ToString().Substring(2, 2);
		}

		public CsiuReport GenerateReport(Config config)
		{
			var schoolYear = SchoolYearFromAsOf(config.AsOf);
			return new CsiuReport
			{
				AsOf = config.AsOf,
				SchoolYear = schoolYear,
				Accounts = GetAccounts(config.AsOf, schoolYear, config.Auns),
			};
		}
	}
}
