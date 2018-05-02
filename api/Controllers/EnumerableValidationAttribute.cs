using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Reflection;

using api.Common;
using models;

namespace api.Controllers
{
	public class EnumerableValidationAttribute : ValidationAttribute
	{
		public readonly IList<string> Values;

		public EnumerableValidationAttribute(Type type)
		{
			var t = type.GetGenericSubclass(typeof(Enumerable<>));
			if (t == null)
				throw new ArgumentException($"{t} does not inherit from {typeof(Enumerable<>)}.");

			Values = t.GetMethod("Values", BindingFlags.Public | BindingFlags.Static).
				Invoke(null, null) as IList<string>;
		}

		protected override ValidationResult IsValid(object value, ValidationContext ctx)
		{
			var s = (string)value;
			if (string.IsNullOrEmpty(s))
				return ValidationResult.Success;

			if (!Values.Contains(s))
				return new ValidationResult($"{s} is not a valid {ctx.DisplayName} value.");

			return ValidationResult.Success;
		}
	}
}
