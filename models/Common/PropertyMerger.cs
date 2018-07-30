using System.Collections.Generic;

namespace models.Common
{
	public static class PropertyMerger
	{
		public static void MergeProperties<T>(T current, T update, IList<string> exclude = null)
		{
			var props = typeof(T).GetProperties();
			foreach (var prop in props)
			{
				if (exclude != null && exclude.Contains(prop.Name))
					continue;

				var c = prop.GetValue(current);
				var u = prop.GetValue(update);
				if (c != u)
					prop.SetValue(current, u);
			}
		}
	}
}
