using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;

using static models.Common.PropertyMerger;

namespace models
{
	public interface ITemplateRepository
	{
		Template CreateOrUpdate(DateTime time, Template update);
		Template CreateOrUpdate(Template update);
		Template Get(ReportType type, string schoolYear);
		// TODO(Erik): GetMetadata?
		// TODO(Erik): GetManyMetadata?
	}

	public class TemplateRepository : ITemplateRepository
	{
		private readonly DbSet<Template> _templates;
		private readonly ILogger<TemplateRepository> _logger;

		public TemplateRepository(PacBillContext context, ILogger<TemplateRepository> logger)
		{
			_templates = context.Templates;
			_logger = logger;
		}

		private static readonly IList<string> _excludedFields = new List<string>
		{
			nameof(Template.Id),
			nameof(Template.ReportType),
			nameof(Template.SchoolYear),
			nameof(Template.Created),
			nameof(Template.LastUpdated),
		};

		public Template CreateOrUpdate(DateTime time, Template update)
		{
			var template = Get(update.ReportType, update.SchoolYear);
			if (template == null)
			{
				update.Created = time;
				update.LastUpdated = time;

				_templates.Add(update);
				return update;
			}

			MergeProperties(template, update, _excludedFields);
			template.LastUpdated = time;
			_templates.Update(template);

			return template;
		}

		public Template CreateOrUpdate(Template update) => CreateOrUpdate(DateTime.Now, update);

		public Template Get(ReportType type, string schoolYear) =>
			_templates.SingleOrDefault(t => t.ReportType == type && t.SchoolYear == schoolYear);
	}
}
