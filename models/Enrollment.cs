using System;

namespace models
{
	public class Enrollment
	{
		public string PACyberId { get; set; }
		public int Aun { get; set; }
		public DateTime StartDate { get; set; }
		public DateTime? EndDate { get; set; }
		public bool IsSpecialEducation { get; set; }
	}
}
