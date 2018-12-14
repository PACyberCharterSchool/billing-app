using System;
using System.Collections.Generic;
using System.Linq;
using models;

namespace api.Dtos
{
	public class AuditHeaderDto
	{
		public int Id { get; set; }
		public string Username { get; set; }
		public string Activity { get; set; }
		public DateTime Timestamp { get; set; }
		public string Identifier { get; set; }
		public IList<AuditDetailDto> Details { get; set; }

		public AuditHeaderDto(AuditHeader model)
		{
			this.Id = model.Id;
			this.Activity = model.Activity;
			this.Timestamp = model.Timestamp;
			this.Username = model.Username;
			this.Identifier = model.Identifier;
			this.Details = model.Details.Select(d => new AuditDetailDto(d)).ToList();
		}
	}

	public class AuditDetailDto
	{
		public int Id { get; set; }
		public string Field { get; set; }
		public string Previous { get; set; }
		public string Next { get; set; }

		public AuditDetailDto(AuditDetail model)
		{
			this.Id = model.Id;
			this.Field = model.Field;
			this.Previous = model.Previous;
			this.Next = model.Next;
		}
	}
}

