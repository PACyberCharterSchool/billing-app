using System;

namespace api.Common
{
	public static class TypeExtensions
	{
		public static Type GetGenericSubclass(this Type type, Type generic)
		{
			while (type != typeof(object))
			{
				if (type == null)
					return null;

				var current = type.IsGenericType ? type.GetGenericTypeDefinition() : type;
				if (current == generic)
					return type;

				type = type.BaseType;
			}

			return null;
		}

		public static bool IsSubclassOfGeneric(this Type type, Type generic)
		{
			return type.GetGenericSubclass(generic) != null;
		}
	}
}
