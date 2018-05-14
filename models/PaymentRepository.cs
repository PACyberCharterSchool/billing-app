using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

using static models.Common.PropertyMerger;

namespace models
{
	public interface IPaymentRepository
	{
		IList<Payment> CreateMany(DateTime time, IList<Payment> creates);
		IList<Payment> CreateMany(IList<Payment> creates);
		IEnumerable<Payment> GetMany(string id);
		IList<Payment> UpdateMany(DateTime time, IList<Payment> updates);
		IList<Payment> UpdateMany(IList<Payment> updates);
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

		private static string HashPayment(Payment payment)
		{
			var bytes = Encoding.UTF8.GetBytes(
				$"{payment.Date}-{payment.ExternalId}-{payment.Type}-{payment.SchoolDistrict}");
			var hash = SHA256.Create().ComputeHash(bytes);

			var sb = new StringBuilder();
			foreach (var b in hash)
				sb.Append(b.ToString("x2"));

			return sb.ToString().Substring(0, 10);
		}


		public IList<Payment> CreateMany(DateTime time, IList<Payment> creates)
		{
			foreach (var create in creates)
			{
				create.PaymentId = HashPayment(create);
				create.Created = time;
			}

			// TODO(Erik): wrap DbUpdateException in custom exception
			_payments.AddRange(creates);
			return creates;
		}

		public IList<Payment> CreateMany(IList<Payment> creates) => CreateMany(DateTime.Now, creates);

		public IEnumerable<Payment> GetMany(string id) => _payments.Where(p => p.PaymentId == id).OrderBy(p => p.Split);

		private static IList<string> _excludedFields = new List<string>
		{
			nameof(Payment.Id),
			nameof(Payment.PaymentId),
			nameof(Payment.Split),
			nameof(Payment.Created),
			nameof(Payment.LastUpdated),
		};

		public IList<Payment> UpdateMany(DateTime time, IList<Payment> us)
		{
			var updated = new List<Payment>();

			var map = us.GroupBy(u => u.PaymentId).ToDictionary(u => u.Key, u => u.ToList());
			foreach (var kv in map)
			{
				var id = kv.Key;
				var updates = kv.Value;
				var payments = GetMany(id).ToList();
				if (payments == null || payments.Count == 0)
					throw new ArgumentException($"Could not find payments with ID {id}."); // TODO(Erik): custom exception

				if (payments.Count > updates.Count)
				{
					_payments.RemoveRange(payments.Skip(updates.Count));
				}

				foreach (var update in updates)
				{
					var payment = payments.SingleOrDefault(p => p.Split == update.Split);
					if (payment == null)
					{
						update.PaymentId = id;
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

		public IList<Payment> UpdateMany(IList<Payment> updates) => UpdateMany(DateTime.Now, updates);
	}
}
