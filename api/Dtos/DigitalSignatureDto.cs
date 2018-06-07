using System;

using models;

namespace api.Dtos
{
	public class DigitalSignatureDto
	{
		public int Id { get; set; }
		public string Title { get; set; }
		public string UserName { get; set; }
    public string FileName { get; set; }
    public byte[] imgData { get; set; }
		public DateTime Created { get; set; }
		public DateTime LastUpdated { get; set; }


		public DigitalSignatureDto(DigitalSignature model)
		{
			this.Id = model.Id;
      this.Title = model.Title;
			this.UserName = model.UserName;
      this.FileName = model.FileName;
      this.imgData = model.imgData;
			this.Created = model.Created;
			this.LastUpdated = model.LastUpdated;
		}
	}
}
