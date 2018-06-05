using System;

using models;

namespace api.Dtos
{
	public class TemplateDto
	{
		public int Id { get; set; }
		// TODO(Erik): JsonConverter
		public ReportType ReportType { get; set; }
		public string SchoolYear { get; set; }
		public string Name { get; set; }
		public DateTime Created { get; set; }
		public DateTime LastUpdated { get; set; }

		public TemplateDto(Template model)
		{
			this.Id = model.Id;
			this.ReportType = model.ReportType;
			this.SchoolYear = model.SchoolYear;
			this.Name = model.Name;
			this.Created = model.Created;
			this.LastUpdated = model.LastUpdated;
		}
	}
}
