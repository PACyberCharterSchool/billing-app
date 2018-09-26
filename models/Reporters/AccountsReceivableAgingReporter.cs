using System;

namespace models.Reporters
{
	public class AccountsReceivableAging { }

	public class AccountsReceivableAgingReporter : IReporter<AccountsReceivableAging, AccountsReceivableAgingReporter.Config>
	{
		private readonly PacBillContext _context;
		public AccountsReceivableAgingReporter(PacBillContext context) => _context = context;

		public class Config
		{
			public DateTime From { get; set; }
			public DateTime AsOf { get; set; }
		}

		public AccountsReceivableAging GenerateReport(Config config)
		{
			throw new System.NotImplementedException();
		}
	}
}
