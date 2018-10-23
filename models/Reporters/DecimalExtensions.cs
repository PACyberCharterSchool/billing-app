using System;

namespace models.Reporters
{
	public static class DecimalExtensions
	{
		public static double Round(this double d, int precision = 2) =>
			Math.Round(d, precision, MidpointRounding.AwayFromZero);

		public static decimal Round(this decimal d, int precision = 2) =>
			Math.Round(d, precision, MidpointRounding.AwayFromZero);

		public static decimal? Round(this decimal? d, bool nullZero = false, int precision = 2)
		{
			var result = d.HasValue ? (decimal?)d.Value.Round() : null;
			if (nullZero && (result.HasValue && result.Value == 0))
				return null;
			return result;
		}

		public static double? Round(this double? d, bool nullZero = false, int precision = 2)
		{
			var result = d.HasValue ? (double?)d.Value.Round() : null;
			if (nullZero && (result.HasValue && result.Value == 0))
				return null;
			return result;
		}
	}
}
