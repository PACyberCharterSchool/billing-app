using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Collections.Generic;

using static models.Common.PropertyMerger;

namespace models
{
	public interface IRefundRepository
	{
		IList<Refund> CreateMany(DateTime time, IList<Refund> creates);
		IList<Refund> CreateMany(IList<Refund> creates);
		Refund Create(DateTime time, Refund create);
		Refund Create(Refund create);
		Refund Get(int id);
		IEnumerable<Refund> GetMany();
		IList<Refund> UpdateMany(DateTime time, IList<Refund> updates);
		IList<Refund> UpdateMany(IList<Refund> updates);
		Refund Update(DateTime time, Refund update);
		Refund Update(Refund update);
	}

	public class RefundRepository : IRefundRepository
	{
		private readonly DbSet<Refund> _refunds;
		private readonly ILogger<RefundRepository> _logger;

		public RefundRepository(PacBillContext context, ILogger<RefundRepository> logger)
		{
			_refunds = context.Refunds;
			_logger = logger;
		}

		public IList<Refund> CreateMany(DateTime time, IList<Refund> creates)
		{
			foreach (var create in creates)
			{
				create.Created = time;
				create.LastUpdated = time;
			}

			_refunds.AddRange(creates);
			return creates;
		}

		public IList<Refund> CreateMany(IList<Refund> creates) => CreateMany(DateTime.Now, creates);

		public Refund Create(DateTime time, Refund create) => CreateMany(time, new[] { create })[0];

		public Refund Create(Refund create) => Create(DateTime.Now, create);

		public Refund Get(int id) => _refunds.SingleOrDefault(r => r.Id == id);

		public IEnumerable<Refund> GetMany() => _refunds.OrderBy(r => r.Date);

		private static IList<string> _excludedFields = new List<string>{
			nameof(Refund.Id),
			nameof(Refund.Created),
			nameof(Refund.LastUpdated),
		};

		public IList<Refund> UpdateMany(DateTime time, IList<Refund> updates)
		{
			var updated = new List<Refund>();
			foreach (var update in updates)
			{
				var refund = _refunds.SingleOrDefault(r => r.Id == update.Id);
				if (refund == null)
					throw new NotFoundException(typeof(Refund), update.Id);

				MergeProperties(refund, update, _excludedFields);
				refund.LastUpdated = time;

				_refunds.Update(refund);
				updated.Add(refund);
			}

			return updated;
		}

		public IList<Refund> UpdateMany(IList<Refund> updates) => UpdateMany(DateTime.Now, updates);

		public Refund Update(DateTime time, Refund update) => UpdateMany(time, new[] { update })[0];

		public Refund Update(Refund update) => Update(DateTime.Now, update);
	}
}
