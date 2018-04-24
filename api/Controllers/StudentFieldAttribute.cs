using System.ComponentModel.DataAnnotations;

using api.Models;

namespace api.Controllers
{
	public class StudentFieldAttribute : ValidationAttribute
	{
		protected override ValidationResult IsValid(object value, ValidationContext validationContext)
		{
			var field = (string)value;

			if (!string.IsNullOrWhiteSpace(field) && !Student.IsValidField(field))
				return new ValidationResult($"The field {validationContext.MemberName} contains an invalid Student property.");

			return ValidationResult.Success;
		}
	}
}
