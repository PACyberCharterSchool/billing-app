using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Swashbuckle.AspNetCore.Swagger;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace api.Controllers
{
	public class AuthorizeOperationFilter : IOperationFilter
	{
		public void Apply(Operation operation, OperationFilterContext context)
		{
			var attributes = context.ApiDescription.ActionAttributes();
			var auth = attributes.FirstOrDefault(a => a.GetType() == typeof(AuthorizeAttribute)) as AuthorizeAttribute;
			if (auth == null)
				return;

			var sb = new StringBuilder();
			sb.AppendLine($"Authorization Policy: <code>{auth.Policy}</code>");
			sb.AppendLine(operation.Description);

			operation.Description = sb.ToString();
		}
	}
}
