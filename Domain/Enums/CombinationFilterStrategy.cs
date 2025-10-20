namespace Domain.Enums
{
	public enum CombinationFilterStrategy
	{
		Comprehensive = 0,  // Generate all combinations (no filtering)
		Balanced = 1,       // Require at least one item with tiered stat
		Strict = 2          // Only use items that have tiered stats
	}
}
