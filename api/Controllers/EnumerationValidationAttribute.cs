using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Reflection;

using api.Common;
using models;

namespace api.Controllers
{
	// For use with query params, because a JsonConverter will not work.
	public class EnumerationValidationAttribute : ValidationAttribute
	{
		Type _type;
		public readonly IList<string> Values;

		public EnumerationValidationAttribute(Type type)
		{
			_type = type;
			var t = type.GetGenericSubclass(typeof(Enumeration<>));
			if (t == null)
				throw new ArgumentException($"{t} does not inherit from {typeof(Enumeration<>)}.");

			Values = t.GetMethod("Values", BindingFlags.Public | BindingFlags.Static).
				Invoke(null, null) as IList<string>;
		}

		protected override ValidationResult IsValid(object value, ValidationContext ctx)
		{
			var s = (string)value;
			if (string.IsNullOrEmpty(s))
				return ValidationResult.Success;

			if (!Values.Contains(s))
				return new ValidationResult($"{s} is not a valid {_type.Name} value.");

			return ValidationResult.Success;
		}
	}
}
