using Domain.Main;

namespace Domain.Combinations
{
	public static class ItemValidator
	{
		public static bool ValidateListOfItems(List<Item> items)
		{
			if (items == null || items.Count == 0)
			{
				return false;
			}

			var sumFireRes = items.Sum(item => item.ItemStats?.FireResistancePercent ?? 0);
			var sumColdRes = items.Sum(item => item.ItemStats?.ColdResistancePercent ?? 0);
			var sumLightningRes = items.Sum(item => item.ItemStats?.LightningResistancePercent ?? 0);
			var sumChaosRes = items.Sum(item => item.ItemStats?.ChaosResistancePercent ?? 0);
			var sumAllRes = items.Sum(item => item.ItemStats?.AllElementalResistancesPercent ?? 0);

			sumFireRes += sumAllRes;
			sumLightningRes += sumAllRes;
			sumColdRes += sumAllRes;

			return sumFireRes > 75.0d && sumFireRes < 100.0d &&
				   sumColdRes > 75.0d && sumColdRes < 100.0d &&
				   sumLightningRes > 75.0d && sumLightningRes < 100.0d &&
				   sumChaosRes > 40.0d;
		}
	}
}
