using System;
using System.ComponentModel.DataAnnotations;

namespace models
{
	public class DigitalSignature
	{
		public int Id { get; set; }

		[Required]
		public string UserName { get; set; }

		[Required]
		public string Title { get; set; }
    public string FileName { get; set; }
    public byte[] imgData { get; set; }
		public DateTime Created { get; set; }
    public DateTime LastUpdated { get; set; }
	}
}
