using System;

using Newtonsoft.Json;

using models;

namespace api.Dtos
{
	// TODO(Erik): return data? ex: Invoice
	public class ReportDto
	{
		public int Id { get; set; }
		[JsonConverter(typeof(ReportTypeJsonConverter))]
		public ReportType Type { get; set; }
		public string SchoolYear { get; set; }
		public string Name { get; set; }
		public bool Approved { get; set; }
		public DateTime Created { get; set; }

		public ReportDto(ReportMetadata model)
		{
			this.Id = model.Id;
			this.Type = model.Type;
			this.SchoolYear = model.SchoolYear;
			this.Name = model.Name;
			this.Approved = model.Approved;
			this.Created = model.Created;
		}
	}
}
