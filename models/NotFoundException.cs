using System;

namespace models
{
	public class NotFoundException : Exception
	{
		public Type Type { get; private set; }
		public object Identifier { get; private set; }

		public NotFoundException(Type type, object id) : base($"Could not find {type.Name} with ID {id}.")
		{
			Type = type;
			Identifier = id;
		}

		public NotFoundException() { }
	}
}
