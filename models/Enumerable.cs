using System;
using System.Linq;
using System.Reflection;
using System.Collections.Generic;

namespace models
{
	public class Enumerable<T> where T : Enumerable<T>
	{
		public string Value { get; private set; }

		protected Enumerable(string value)
		{
			Value = value;
		}

		public static T FromString(string value)
		{
			return GetAll().First(t => t.Value.ToLower() == value.ToLower());
		}

		private static IList<T> _fields;

		public static IList<T> GetAll()
		{
			if (_fields == null)
				_fields = typeof(T).GetFields(BindingFlags.Public | BindingFlags.Static | BindingFlags.DeclaredOnly).
					Where(p => p.FieldType == typeof(T)).
					Select(i => (T)i.GetValue(null)).
					OrderBy(p => p.Value).
					ToList();

			return _fields;
		}

		private static IList<string> _values;

		public static IList<string> Values()
		{
			if (_values == null)
				_values = GetAll().Select(t => t.Value).ToList();

			return _values;
		}

		public override string ToString()
		{
			return Value;
		}
	}
}
