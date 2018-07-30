namespace models
{
	public class SortDirection : Enumeration<SortDirection>
	{
		private SortDirection(string value) : base(value) { }
		private SortDirection() : base() { }

		public static readonly SortDirection Ascending = new SortDirection("asc");
		public static readonly SortDirection Descending = new SortDirection("desc");
	}
}
