using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;

namespace models
{
	public interface IEnrollmentRepository
	{
		IEnumerable<Enrollment> GetMany(DateTime start, DateTime end, string paCyberId = null, int aun = 0);
	}

	public class EnrollmentRepository : IEnrollmentRepository
	{
		private readonly DbQuery<Enrollment> _enrollments;
		private readonly ILogger<EnrollmentRepository> _logger;

		public EnrollmentRepository(PacBillContext context, ILogger<EnrollmentRepository> logger)
		{
			_enrollments = context.Enrollments;
			_logger = logger;
		}

		public IEnumerable<Enrollment> GetMany(DateTime start, DateTime end, string paCyberId = null, int aun = 0)
		{
			var results = _enrollments.Where(e => e.StartDate != e.EndDate &&
				e.StartDate <= end &&
				(e.EndDate == null || e.EndDate >= start));

			if (!string.IsNullOrWhiteSpace(paCyberId))
				results = results.Where(e => e.PACyberId == paCyberId);

			if (aun != 0)
				results = results.Where(e => e.Aun == aun);

			return results;
		}
	}
}
