using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Reflection.Emit;

namespace api.Common
{
	public static class Projection
	{
		private static Type CreateAnonymousType(Type type, IList<(string Name, Type Type)> fields)
		{
			var module = AssemblyBuilder.
				DefineDynamicAssembly(new AssemblyName("TempAssembly"), AssemblyBuilderAccess.Run).
				DefineDynamicModule("TempAssembly");

			var anon = module.DefineType("_" + type.Name, TypeAttributes.Public);
			foreach (var field in fields)
				anon.DefineField(field.Name, field.Type, FieldAttributes.Public);

			return anon.CreateType();
		}

		private static Dictionary<string, List<string>> ExtractSublists(IList<string> list)
		{
			return list.Where(i => i.Contains(".")).
				GroupBy(i => i.Substring(0, i.IndexOf("."))).
				ToDictionary(i => i.Key, i => i.Select(ii => ii.Substring(ii.IndexOf(".") + 1)).ToList());
		}

		private static Dictionary<string, List<string>> ExtractPropertyLists(IList<string> list)
		{
			var result = new Dictionary<string, List<string>>();
			if (list == null)
				return result;

			foreach (var s in list)
			{
				// var parts = s.Split(".");
				// if (!result.ContainsKey(""))
				// 	result.Add("", new List<string>());

				// result[""].Add(parts[0]);

				// if (parts.Length > 1)
				// {
				// 	if (!result.ContainsKey(parts[0]))
				// 		result.Add(parts[0], new List<string>());

				// 	result[parts[0]].Add(string.Join(".", parts.Skip(1)));
				// }

				var key = "";
				var value = s;
				if (value.Contains("."))
				{
					var i = value.IndexOf(".");
					key = value.Substring(0, i);
					value = value.Substring(i + 1);
				}

				if (!result.ContainsKey(key))
					result.Add(key, new List<string>());

				result[key].Add(value);
			}

			return result;
		}

		private static bool ShouldInclude(Dictionary<string, List<string>> list, string name)
		{
			if (list.Count == 0)
				return true;

			return list.ContainsKey(name) || (list.ContainsKey("") && list[""].Contains(name));
		}

		private static MemberInitExpression Project(
			Type type,
			Expression param,
			IList<string> includeList,
			IList<string> excludeList)
		{
			var includes = ExtractPropertyLists(includeList);
			var excludes = ExtractPropertyLists(excludeList);

			var props = type.GetProperties().
				Where(p => ShouldInclude(includes, p.Name)).ToArray();

			if (excludes.ContainsKey(""))
				props = props.Where(p => !excludes[""].Contains(p.Name)).ToArray();

			var anon = CreateAnonymousType(type, props.Select(p =>
			{
				if (includes.ContainsKey(p.Name) || excludes.ContainsKey(p.Name))
					return (p.Name, typeof(object));
				else
					return (p.Name, p.PropertyType);
			}).ToList());
			var binders = props.Select(p =>
			{
				var prop = Expression.Property(param, p.Name);
				if (includes.ContainsKey(p.Name) || excludes.ContainsKey(p.Name))
				{
					var nestedInit = Project(p.PropertyType, prop,
						includes.GetValueOrDefault(p.Name),
						excludes.GetValueOrDefault(p.Name));
					return Expression.Bind(anon.GetField(p.Name), nestedInit);
				}
				else
					return Expression.Bind(anon.GetField(p.Name), prop);
			});

			var ctor = Expression.New(anon.GetConstructor(Type.EmptyTypes));
			return Expression.MemberInit(ctor, binders);
		}

		public static object Project<T>(T obj, IList<string> includes = null, IList<string> excludes = null)
		{
			var param = Expression.Parameter(typeof(T), "x");
			var init = Project(typeof(T), param, includes, excludes);
			var lambda = Expression.Lambda(init, param);

			return lambda.Compile().DynamicInvoke(obj);
		}
	}
}
