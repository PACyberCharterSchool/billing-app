using Microsoft.EntityFrameworkCore;
using System.Data;

namespace models.Reporters
{
	// Marker interface: should not be used directly.
	public interface IReporter { }

	public interface IReporter<T, U> : IReporter
	{
		T GenerateReport(U config);
	}
}
