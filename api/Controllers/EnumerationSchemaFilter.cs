using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

using Swashbuckle.AspNetCore.Swagger;
using Swashbuckle.AspNetCore.SwaggerGen;

using api.Common;
using models;

namespace api.Controllers
{
	public class EnumerationSchemaFilter : ISchemaFilter
	{
		public void Apply(Schema model, SchemaFilterContext context)
		{
			var info = context.SystemType.GetTypeInfo();
			if (info.IsSubclassOfGeneric(typeof(Enumeration<>)))
			{
				var type = info.GetGenericSubclass(typeof(Enumeration<>));
				var values = type.GetMethod("Values").Invoke(null, null) as IList<string>;
				model.Type = "string";
				model.Enum = values.Select(v => (object)v).ToList();
				return;
			}

			var props = info.GetProperties();
			foreach (var prop in props)
			{
				var att = prop.GetCustomAttribute(typeof(EnumerationValidationAttribute)) as EnumerationValidationAttribute;
				if (att == null)
					return;

				var values = att.Values;
				foreach (var p in model.Properties)
				{
					if (p.Key.ToLower() == prop.Name.ToLower())
						model.Properties[p.Key].Enum = values.Select(v => (object)v).ToList();
				}
			}
		}
	}
}
