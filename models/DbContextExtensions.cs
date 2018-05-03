using Microsoft.EntityFrameworkCore;
using System;

namespace models
{
	public static class DbContextExtensions
	{
		public static T SaveChanges<T>(this DbContext context, Func<T> func)
		{
			T result = func();
			context.SaveChanges();
			return result;
		}
	}
}
