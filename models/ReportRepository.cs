using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;

namespace models
{
	public interface IReportRepository
	{
		void Approve(string name);
		Report Create(DateTime time, Report create);
		Report Create(Report create);
		Report Get(string name);
		IList<ReportMetadata> GetManyMetadata(
			string name = null,
			ReportType type = null,
			string year = null,
			bool? approved = null);
		void Reject(string name);
	}

	public class ReportRepository : IReportRepository
	{
		private readonly DbSet<Report> _reports;
		private readonly ILogger<ReportRepository> _logger;

		public ReportRepository(PacBillContext context, ILogger<ReportRepository> logger)
		{
			_reports = context.Reports;
			_logger = logger;
		}

		public void Approve(string name)
		{
			var report = Get(name);
			if (report == null)
				throw new NotFoundException(typeof(Report), name);

			report.Approved = true;
			_reports.Update(report);
		}

		public Report Create(DateTime time, Report create)
		{
			create.Created = time;
			_reports.Add(create);
			return create;
		}

		public Report Create(Report create) => Create(DateTime.Now, create);

		public Report Get(string name) => _reports.SingleOrDefault(r => r.Name == name);

		public IList<ReportMetadata> GetManyMetadata(
			string name = null,
			ReportType type = null,
			string year = null,
			bool? approved = null)
		{
			var reports = _reports.Select(r => new ReportMetadata
			{
				Id = r.Id,
				Type = r.Type,
				SchoolYear = r.SchoolYear,
				Name = r.Name,
				Approved = r.Approved,
				Created = r.Created,
			});

			if (name != null)
				reports = reports.Where(r => r.Name == name);

			if (type != null)
				reports = reports.Where(r => r.Type == type);

			if (year != null)
				reports = reports.Where(r => r.SchoolYear == year);

			if (approved != null)
				reports = reports.Where(r => r.Approved == approved);

			return reports.ToList();
		}

		public void Reject(string name)
		{
			var report = Get(name);
			if (report == null)
				return;

			_reports.Remove(report);
		}
	}
}
