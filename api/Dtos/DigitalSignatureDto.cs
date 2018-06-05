using System;

using models;

namespace api.Dtos
{
	public class DigitalSignatureDto
	{
		public int Id { get; set; }
		public string Title { get; set; }
		public string Username { get; set; }
    public string FileName { get; set; }
		public DateTime Created { get; set; }
		public DateTime LastUpdated { get; set; }


		public DigitalSignatureDto(DigitalSignature model)
		{
			this.Id = model.Id;
      this.Title = model.Title;
			this.Username = model.Username;
      this.FileName = model.FileName;
			this.Created = model.Created;
			this.LastUpdated = model.LastUpdated;
		}
	}
}
