using System;
using System.Linq;
using System.Reflection;
using System.Collections.Generic;

namespace models
{
	public class Enumeration<T> where T : Enumeration<T>
	{
		public string Value { get; private set; }

		protected Enumeration(string value)
		{
			Value = value;
		}

		protected Enumeration() { }

		public static T FromString(string value)
		{
			var s = GetAll().FirstOrDefault(t => t.Value.ToLower() == value.ToLower());
			if (s == null)
				throw new ArgumentException($"{typeof(T).Name} does not contain value '{value}'");

			return s;
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
