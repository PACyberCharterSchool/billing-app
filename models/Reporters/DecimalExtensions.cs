using System;

namespace models.Reporters
{
	public static class DecimalExtensions
	{
		public static decimal Round(this decimal d, int precision = 2) =>
			Math.Round(d, precision, MidpointRounding.AwayFromZero);
	}
}
