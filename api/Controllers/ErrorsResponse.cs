using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.Collections.Generic;

namespace api.Controllers
{
	public struct ErrorsResponse
	{
		public IList<string> Errors { get; }

		public ErrorsResponse(IList<string> errors)
		{
			Errors = errors;
		}

		private static IList<string> ModelStateToList(ModelStateDictionary modelState)
		{
			var errors = new List<string>();
			foreach (var value in modelState.Values)
				foreach (var error in value.Errors)
					errors.Add(error.ErrorMessage);

			return errors;
		}

		public ErrorsResponse(ModelStateDictionary modelState) : this(ModelStateToList(modelState)) { }
	}
}
