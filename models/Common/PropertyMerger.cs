using System.Collections.Generic;

namespace models.Common
{
	public static class PropertyMerger
	{
		public static IDictionary<string, (string Previous, string Next)> MergeProperties<T>(
			T current, T update, IList<string> exclude = null)
		{
			var delta = new Dictionary<string, (string Previous, string Next)>();
			foreach (var prop in typeof(T).GetProperties())
			{
				if (exclude != null && exclude.Contains(prop.Name))
					continue;

				var c = prop.GetValue(current);
				var u = prop.GetValue(update);
				if (u == null && c == null)
					continue;

				if ((u == null && c != null) || (c == null && u != null) || !u.Equals(c))
				{
					prop.SetValue(current, u);
					delta.Add(prop.Name, (
						c != null ? c.ToString() : null,
						u != null ? u.ToString() : null
					));
				}
			}

			return delta;
		}
	}
}
