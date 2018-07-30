using Microsoft.Extensions.Logging;
using System;

namespace models.Tests.Util
{
	// See: https://alastaircrabtree.com/using-logging-in-unit-tests-in-net-core/
	public class TestLogger<T> : ILogger<T>, IDisposable
	{
		private readonly Action<string> output = Console.WriteLine;

		public void Dispose()
		{
		}

		public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception,
				Func<TState, Exception, string> formatter) => output(formatter(state, exception));

		public bool IsEnabled(LogLevel logLevel) => true;

		public IDisposable BeginScope<TState>(TState state) => this;
	}
}
