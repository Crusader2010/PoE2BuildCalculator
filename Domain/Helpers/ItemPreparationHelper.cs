using Domain.Main;
using Domain.Static;

using System.Collections.Immutable;

namespace Domain.Helpers
{
	/// <summary>
	/// Helper for preparing and validating item collections for combination generation.
	/// </summary>
	public static class ItemPreparationHelper
	{
		public record PreparedItems(
			List<List<Item>> ItemsWithoutRings,
			List<Item> Rings,
			bool HasItems,
			bool HasRings
		);

		/// <summary>
		/// Prepares items by grouping by class and extracting rings.
		/// </summary>
		public static PreparedItems PrepareItemsForCombinations(ImmutableList<Item> parsedItems)
		{
			ArgumentNullException.ThrowIfNull(parsedItems);

			if (parsedItems == null || parsedItems.Count == 0)
			{
				return new PreparedItems([], null, false, false);
			}

			var itemsForClasses = parsedItems
				.Where(x => Constants.ITEM_CLASSES.Contains(x.Class, StringComparer.OrdinalIgnoreCase))
				.GroupBy(x => x.Class, StringComparer.OrdinalIgnoreCase)
				.ToDictionary(x => x.Key, x => x.ToList(), StringComparer.OrdinalIgnoreCase);

			itemsForClasses.TryGetValue(Constants.ITEM_CLASS_RING, out var rings);
			itemsForClasses.Remove(Constants.ITEM_CLASS_RING);

			var itemsWithoutRings = itemsForClasses.Values.ToList();

			bool hasItems = itemsWithoutRings.Any(list => list.Count > 0);
			bool hasRings = rings?.Count > 0;

			return new PreparedItems(itemsWithoutRings, rings, hasItems, hasRings);
		}

		/// <summary>
		/// Creates sampled versions of item lists for benchmarking.
		/// </summary>
		public static (List<List<Item>> SampledItems, List<Item> SampledRings) CreateSamples(
			List<List<Item>> itemsWithoutRings,
			List<Item> rings,
			int maxSampleSize)
		{
			var sampledItems = itemsWithoutRings
				.Select(list => list.Take(maxSampleSize).ToList())
				.ToList();

			var sampledRings = rings?.Take(maxSampleSize).ToList();

			return (sampledItems, sampledRings);
		}
	}
}
