using System;
using System.Collections.Generic;
using System.Linq;

using models.Common;

namespace models.Reporters
{
	public class TotalsOnlyEnrollment
	{
		public int Regular { get; set; }
		public int Special { get; set; }
	}

	public class TotalsOnlyEnrollments : Dictionary<string, TotalsOnlyEnrollment> { }

	public class TotalsOnlyTransaction
	{
		public double? Sd { get; set; }
		public double? Pde { get; set; }
		public double? Refund { get; set; }
	}

	public class TotalsOnlyTransactions : Dictionary<string, TotalsOnlyTransaction> { }

	public class TotalsOnlyInvoice
	{
		public string SchoolYear { get; set; }
		public int FirstYear => int.Parse(SchoolYear.Split("-")[0]);
		public int SecondYear => int.Parse(SchoolYear.Split("-")[1]);
		public string Scope { get; set; }
		public DateTime Prepared { get; set; }
		public TotalsOnlyEnrollments Enrollments { get; set; }
		public double RegularDue { get; set; }
		public double SpecialDue { get; set; }
		public double TotalDue { get; set; }
		public TotalsOnlyTransactions Transactions { get; set; }
		public double TotalSd { get; set; }
		public double TotalPde { get; set; }
		public double TotalRefund { get; set; }
		public double TotalPaid { get; set; }
		public double NetDue { get; set; }
	}

	public class TotalsOnlyInvoiceReporter : IReporter<TotalsOnlyInvoice, TotalsOnlyInvoiceReporter.Config>
	{
		private readonly PacBillContext _context;

		public TotalsOnlyInvoiceReporter(PacBillContext context) => _context = context;

		public class Config
		{
			public string SchoolYear { get; set; }
			public string Scope { get; set; }
			public IList<int> Auns { get; set; }
			public SchoolDistrictPaymentType PaymentType { get; set; }
		}

		private TotalsOnlyEnrollments GetEnrollments(BulkInvoice bulk)
		{
			var enrollments = new TotalsOnlyEnrollments();
			foreach (var month in Month.AsEnumerable())
				enrollments.Add(month.Name, new TotalsOnlyEnrollment
				{
					Regular = bulk.Districts.Sum(d => EnrollmentsForMonth(d.RegularEnrollments, month)),
					Special = bulk.Districts.Sum(d => EnrollmentsForMonth(d.SpecialEnrollments, month)),
				});

			return enrollments;

			int EnrollmentsForMonth(InvoiceEnrollments ee, Month month)
			{
				return ee.ContainsKey(month.Name) ? ee[month.Name] : 0;
			}
		}

		private TotalsOnlyTransactions GetTransactions(BulkInvoice bulk)
		{
			var transactions = new TotalsOnlyTransactions();
			foreach (var month in Month.AsEnumerable())
			{
				transactions.Add(month.Name, new TotalsOnlyTransaction
				{
					Sd = bulk.Districts.Sum(d => SdForMonth(d.Transactions.AsDictionary(), month)).Round(true),
					Pde = bulk.Districts.Sum(d => PdeForMonth(d.Transactions.AsDictionary(), month)).Round(true),
					Refund = bulk.Districts.Sum(d => RefundForMonth(d.Transactions.AsDictionary(), month)).Round(true),
				});
			}

			return transactions;

			double? SdForMonth(IDictionary<string, InvoiceTransaction> tt, Month month)
			{
				return tt.ContainsKey(month.Name) ? tt[month.Name].Payments.Sum(p => p.CheckAmount) : null;
			}

			double? PdeForMonth(IDictionary<string, InvoiceTransaction> tt, Month month)
			{
				return tt.ContainsKey(month.Name) ? tt[month.Name].Payments.Sum(p => p.UniPayAmount) : null;
			}

			double? RefundForMonth(IDictionary<string, InvoiceTransaction> tt, Month month)
			{
				return tt.ContainsKey(month.Name) ? tt[month.Name].Refunds.Sum() : (double?)null;
			}
		}

		public TotalsOnlyInvoice GenerateReport(Config config)
		{
			var bulk = new BulkInvoiceReporter(_context).GenerateReport(new BulkInvoiceReporter.Config
			{
				SchoolYear = config.SchoolYear,
				Scope = config.Scope,
				Auns = config.Auns,
				PaymentType = config.PaymentType,
			});

			var report = new TotalsOnlyInvoice
			{
				SchoolYear = config.SchoolYear,
				Scope = config.Scope,
				Prepared = DateTime.Now,
			};

			report.Enrollments = GetEnrollments(bulk);
			report.RegularDue = bulk.Districts.Sum(d => (d.SchoolDistrict.RegularRate * d.RegularEnrollments.Sum(e => e.Value) / 12).Round());
			report.SpecialDue = bulk.Districts.Sum(d => (d.SchoolDistrict.SpecialRate * d.SpecialEnrollments.Sum(e => e.Value) / 12).Round());
			report.TotalDue = (report.RegularDue + report.SpecialDue).Round();

			report.Transactions = GetTransactions(bulk);
			report.TotalSd = report.Transactions.Sum(t => t.Value.Sd.HasValue ? t.Value.Sd.Value : 0D).Round();
			report.TotalPde = report.Transactions.Sum(t => t.Value.Pde.HasValue ? t.Value.Pde.Value : 0D).Round();
			report.TotalRefund = report.Transactions.Sum(t => t.Value.Refund.HasValue ? t.Value.Refund.Value : 0D).Round();
			report.TotalPaid = ((report.TotalSd + report.TotalPde) - report.TotalRefund).Round();

			report.NetDue = (report.TotalDue - report.TotalPaid).Round();

			return report;
		}
	}
}
