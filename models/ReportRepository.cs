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
		IList<Report> CreateMany(DateTime time, IList<Report> creates);
		IList<Report> CreateMany(IList<Report> creates);
		Report CreateBulk(Report createBulk);
		Report Create(DateTime time, Report create);
		Report Create(Report create);
		Report Get(string name);
		IEnumerable<Report> GetMany(
			string name = null,
			ReportType type = null,
			string scope = null,
			bool? approved = null);
		IList<ReportMetadata> GetManyMetadata( // TODO(Erik): skip take
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

		public IList<Report> CreateMany(DateTime time, IList<Report> creates)
		{
			foreach (var report in creates)
				report.Created = time;

			_reports.AddRange(creates);
			return creates;
		}

		public IList<Report> CreateMany(IList<Report> creates) => CreateMany(DateTime.Now, creates);

		public Report Create(DateTime time, Report create) => CreateMany(time, new[] { create })[0];

		public Report CreateBulk(Report createBulk)
		{
			_reports.Add(createBulk);
			return createBulk;
		}

		public Report Create(Report create) => Create(DateTime.Now, create);

		public Report Get(string name) => _reports.SingleOrDefault(r => r.Name == name);

		public IEnumerable<Report> GetMany(
			string name = null,
			ReportType type = null,
			string scope = null,
			bool? approved = null)
		{
			var reports = _reports.AsQueryable();

			if (name != null)
				reports = reports.Where(r => r.Name == name);

			if (type != null)
				reports = reports.Where(r => r.Type == type);

			if (scope != null)
				reports = reports.Where(r => r.Scope == scope);

			if (approved != null)
				reports = reports.Where(r => r.Approved == approved);

			return reports;
		}

		// TODO(Erik): filter by report data?
		public IList<ReportMetadata> GetManyMetadata(
			string name = null,
			ReportType type = null,
			string scope = null,
			bool? approved = null)
		{
			return GetMany(name, type, scope, approved).Select(r => new ReportMetadata
			{
				Id = r.Id,
				Type = r.Type,
				Scope = r.Scope,
				Name = r.Name,
				Approved = r.Approved,
				Created = r.Created,
			}).ToList();
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
