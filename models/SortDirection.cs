namespace models
{
	public class SortDirection : Enumerable<SortDirection>
	{
		private SortDirection(string value) : base(value) { }

		public static readonly SortDirection Ascending = new SortDirection("asc");
		public static readonly SortDirection Descending = new SortDirection("desc");
	}
}
