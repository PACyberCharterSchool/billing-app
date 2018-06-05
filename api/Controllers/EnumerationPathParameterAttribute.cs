using System;
using System.Collections.Generic;
using System.Reflection;

using static api.Common.TypeExtensions;
using models;

namespace api.Controllers
{
	public class EnumerationPathParameterAttribute : Attribute
	{
		public string Param { get; private set; }
		public readonly IList<string> Values;

		public EnumerationPathParameterAttribute(string param, Type type)
		{
			Param = param;

			var t = type.GetGenericSubclass(typeof(Enumeration<>));
			if (t == null)
				throw new ArgumentException($"{t} does not inherit from {typeof(Enumeration<>)}.");

			Values = t.GetMethod("Values", BindingFlags.Public | BindingFlags.Static).
				Invoke(null, null) as IList<string>;
		}
	}
}
