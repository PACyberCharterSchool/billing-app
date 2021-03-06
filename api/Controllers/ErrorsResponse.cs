using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.Collections.Generic;
using System;
using System.Linq;

namespace api.Controllers
{
	public struct ErrorsResponse
	{
		public IList<string> Errors { get; }

		public ErrorsResponse(IList<string> errors) => Errors = errors;
		public ErrorsResponse(params string[] errors) => Errors = errors.ToList();
		public ErrorsResponse(Exception e) => Errors = new[] { e.Message };

		private static IList<string> ModelStateToList(ModelStateDictionary modelState)
		{
			var errors = new List<string>();
			foreach (var value in modelState.Values)
				foreach (var error in value.Errors)
				{
					if (!string.IsNullOrWhiteSpace(error.ErrorMessage))
					{
						errors.Add(error.ErrorMessage);
						continue;
					}

					if (error.Exception != null && !string.IsNullOrWhiteSpace(error.Exception.Message))
						errors.Add(error.Exception.Message);
				}

			return errors;
		}

		public ErrorsResponse(ModelStateDictionary modelState) : this(ModelStateToList(modelState)) { }
	}
}
