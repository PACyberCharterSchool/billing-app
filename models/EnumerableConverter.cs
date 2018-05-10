using System;
using System.Linq.Expressions;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore.Storage.Converters;

namespace models
{
	public class EnumerableConverter<T> : ValueConverter<T, string> where T : Enumerable<T>
	{
		public EnumerableConverter() : base(
			v => v.Value,
			v => (T)typeof(T).GetMethod("FromString").Invoke(null, new object[] { v }))
		{ }
	}
}
