using System;

using models;

namespace api.Dtos
{
	public class AuditDto
	{
		public int Id { get; set; }
		public string Username { get; set; }

    public string Activity { get; set; }
    public DateTime Timestamp { get; set; }
		public string Field { get; set; }
		public string Identifier { get; set; }

		public string Next { get; set; }
		public string Previous { get; set; }

		public AuditDto(AuditRecord model)
		{
			this.Id = model.Id;
			this.Activity = model.Activity;
			this.Timestamp = model.Timestamp;
			this.Username = model.Username;
			this.Field = model.Field;
      this.Identifier = model.Identifier;
			this.Next = model.Next;
			this.Previous = model.Previous;
		}
	}
}

