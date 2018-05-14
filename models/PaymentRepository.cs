using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;

using static models.Common.PropertyMerger;

namespace models
{
	public interface IPaymentRepository
	{
		IList<Payment> CreateOrUpdateMany(DateTime time, IList<Payment> updates);
		IList<Payment> CreateOrUpdateMany(IList<Payment> updates);
		IEnumerable<Payment> GetMany(string id);
	}

	public class PaymentRepository : IPaymentRepository
	{
		private readonly DbSet<Payment> _payments;
		private readonly ILogger<PaymentRepository> _logger;

		public PaymentRepository(PacBillContext context, ILogger<PaymentRepository> logger)
		{
			_payments = context.Payments;
			_logger = logger;
		}

		private static IList<string> _excludedFields = new List<string>
		{
			nameof(Payment.Id),
			nameof(Payment.PaymentId),
			nameof(Payment.Split),
			nameof(Payment.Created),
			nameof(Payment.LastUpdated),
		};

		public IList<Payment> CreateOrUpdateMany(DateTime time, IList<Payment> us)
		{
			var updated = new List<Payment>();

			var map = us.GroupBy(u => u.PaymentId).ToDictionary(u => u.Key, u => u.ToList());
			foreach (var kv in map)
			{
				var id = kv.Key;
				var updates = kv.Value;
				var payments = GetMany(id).ToList();
				if (payments.Count > updates.Count)
					_payments.RemoveRange(payments.Skip(updates.Count));

				foreach (var update in updates)
				{
					var payment = payments.SingleOrDefault(p => p.Split == update.Split);
					if (payment == null)
					{
						update.Created = time;
						update.LastUpdated = time;

						_payments.Add(update);
						updated.Add(update);
						continue;
					}

					MergeProperties(payment, update, _excludedFields);
					payment.LastUpdated = time;
					_payments.Update(payment);
					updated.Add(payment);
				}
			}

			return updated;
		}

		public IList<Payment> CreateOrUpdateMany(IList<Payment> updates) => CreateOrUpdateMany(DateTime.Now, updates);

		public IEnumerable<Payment> GetMany(string id) => _payments.Where(p => p.PaymentId == id).OrderBy(p => p.Split);
	}
}
