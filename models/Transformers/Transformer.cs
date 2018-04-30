using System;
using System.Collections.Generic;

using models;

namespace models.Transformers
{
	public interface ITransformer
	{
		IEnumerable<object> Transform(IEnumerable<object> input);
	}

	public abstract class Transformer<T, U> : ITransformer
	{
		protected abstract IEnumerable<U> Transform(IEnumerable<T> input);

		public IEnumerable<object> Transform(IEnumerable<object> input)
		{
			if (!(input is IEnumerable<T>))
				throw new ArgumentException($"Expected type {typeof(T)}; got type {typeof(object)}.");

			return (IEnumerable<object>)Transform((IEnumerable<T>)input);
		}
	}

	// public class TransformerChain : List<ITransformer>, ITransformer
	// {
	// 	public IEnumerable<object> Transform(IEnumerable<object> input)
	// 	{
	// 		foreach (var t in this)
	// 			input = t.Transform(input);

	// 		return input;
	// 	}
	// }

	// public class TransformerJunction : List<ITransformer>, ITransformer
	// {
	// 	public IEnumerable<object> Transform(IEnumerable<object> input)
	// 	{
	// 		foreach (var t in this)
	// 			t.Transform(input);

	// 		return input;
	// 	}
	// }
}
