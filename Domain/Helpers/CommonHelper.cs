using System.Numerics;

namespace Domain.Helpers
{
	public static class CommonHelper
	{
		public static string GetBigIntegerApproximation(BigInteger number)
		{
			if (number.IsZero) return "0";

			string s = BigInteger.Abs(number).ToString();
			int exponent = s.Length - 1;

			// Take first 3 digits and insert decimal point
			string mantissaStr = s.Length == 1 ? s : s.Insert(1, ".");
			if (mantissaStr.Length > 4) mantissaStr = mantissaStr[..4]; // keep 3 significant digits

			string sign = number.Sign < 0 ? "-" : "";
			return $"{sign}{mantissaStr} × 10^{exponent}";
		}

		public static IEnumerable<T> TakeLong<T>(this IEnumerable<T> source, long count)
		{
			if (source == null) ArgumentNullException.ThrowIfNull(source, nameof(source));
			if (count <= 0)
				yield break;

			long taken = 0;
			foreach (var item in source)
			{
				yield return item;
				taken++;
				if (taken >= count)
					yield break;
			}
		}
	}
}
