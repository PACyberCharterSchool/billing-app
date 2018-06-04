using System;

namespace models
{
	public class Template
	{
		public int Id { get; set; }
		public ReportType ReportType { get; set; }
		public string SchoolYear { get; set; }
		public string Name { get; set; }
		public byte[] Content { get; set; }
		public DateTime Created { get; set; }
		public DateTime LastUpdated { get; set; }
	}
}
