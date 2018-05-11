using System;
using System.Linq.Expressions;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore.Storage.Converters;

namespace models
{
	public class EnumerationConverter<T> : ValueConverter<T, string> where T : Enumeration<T>
	{
		public EnumerationConverter() : base(
			v => v.Value,
			v => (T)typeof(T).GetMethod("FromString").Invoke(null, new object[] { v }))
		{ }
	}
}
