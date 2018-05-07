using System.Linq;

using Swashbuckle.AspNetCore.Swagger;
using Swashbuckle.AspNetCore.SwaggerGen;

using models;

namespace api.Controllers
{
	public class StudentFieldOperationFilter : IOperationFilter
	{
		public void Apply(Operation operation, OperationFilterContext context)
		{
			if (operation.Parameters == null)
				return;

			foreach (var param in context.ApiDescription.ParameterDescriptions)
			{
				var type = param.ParameterDescriptor.ParameterType;
				if (type.Name == param.Type.Name)
					continue;

				var prop = type.GetProperty(param.Name);
				if (prop == null)
					continue;

				var att = prop.GetCustomAttributes(typeof(StudentFieldAttribute), true).FirstOrDefault();
				if (att == null)
					continue;

				var fields = typeof(Student).GetProperties().Select(p => p.Name);
				var def = operation.Parameters.SingleOrDefault(p => p.Name == param.Name);
				if (def == null)
					continue;

				def.Extensions.Add("enum", fields);
			}
		}
	}
}
