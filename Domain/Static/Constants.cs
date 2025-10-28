using System.Collections.Immutable;

namespace Domain.Static
{
	public static class Constants
	{
		public const string ITEM_IS_MINE_TAG = "(MINE)";
		public const string ITEM_HAS_POTENTIAL_TAG = "(POTENTIAL)";
		public const string ITEM_CLASS_TAG = "Item Class:";
		public const string ITEM_RARITY_TAG = "Rarity:";
		public const string ITEM_CORRUPTED_TAG = "Corrupted";
		public const string ITEM_CLASS_RING = "Ring";
		public const string DEFAULT_DESCRIPTION = "NONE";
		public const string TOTAL_TIERS_WEIGHT_SUFFIX = @" %";
		public const string DOUBLE_NUMBER_FORMAT = "0.00";

		public static readonly ImmutableList<string> ITEM_CLASSES =
		[
			"Claw",
			"Dagger",
			"Two Hand Axe",
			"One Hand Axe",
			"Two Hand Mace",
			"One Hand Mace",
			"Two Hand Sword",
			"One Hand Sword",
			"Flail",
			"Staff",
			"Quarterstaff",
			"Sceptre",
			"Spear",
			"Bow",
			"Crossbow",
			"Wand",
			"Gloves",
			"Boots",
			"Belt",
			"Amulet",
			"Shield",
			"Buckler",
			"Body Armour",
			"Helmet",
			"Focus",
			"Quiver",
			"Charm",
			"Flask",
			"Ring"
		];

		//public static readonly ImmutableArray<string> MATH_OPERATORS = ["+", "-", "*", "/"];
		//public static readonly ImmutableArray<string> LOGICAL_OPERATORS = ["AND", "OR", "XOR"];
		//public static readonly ImmutableArray<string> GROUP_VALUES_OPERATORS = [">=", ">", "=", "<", "<="];
		//public static readonly ImmutableArray<string> GROUP_MIN_MAX_LOGICAL_OPERATORS = ["AND", "OR"];

		public const string VALIDATOR_HELP_TEXT = @"=== CUSTOM VALIDATOR INFORMATION ===

---------------------------------------------------------------------------
WITHIN A GROUP (list of item stats with operators):
---------------------------------------------------------------------------

Stats are evaluated LEFT-TO-RIGHT in the order they appear.
Example: If you have:
	• MaxLife (+)
	• Armour% (-)
	• Spirit (*)
	• next_stat

The computation will be: ((MaxLife + Armour%) - Spirit) * next_stat
This is LEFT-ASSOCIATIVE evaluation.

To control order of item stats' evaluation:
	1. Reorder stats using ▲▼ buttons
	2. First stat evaluated first
	3. Each operator applies between result and next stat

---------------------------------------------------------------------------
OPERATIONS BETWEEN GROUPS:
---------------------------------------------------------------------------

Group operations are evaluated in order, top to bottom.
Each operation must have one group selected, to be applied to.
You can have more operations, thus using some of the groups multiple times.

Group-level operators are used to link operations (AND/OR/XOR):
	• AND: Both group operations must pass
	• OR: At least one group operation must pass  
	• XOR: Exactly one group operation must pass

Example with 3 groups:
	Group1 (TRUE) → AND
	Group2 (FALSE) → OR
	Group3 (TRUE)

The group operations' evaluation is also left-associative: (TRUE AND FALSE) OR TRUE = FALSE OR TRUE = TRUE

To control the order of group operations (if you need to swap operations X and Y and don't care to remember their values):
	1. Create a new operation, Z, identical to X.
	2. Change the values and group of X to match Y.
	3. Change Y to match Z.
	4. Delete Z.
	4. Each group-level operator applies between one operation's result and next's.

---------------------------------------------------------------------------
CONSTRAINTS:
---------------------------------------------------------------------------
- Each group must have at least one item stat chosen;
- Each group operation must have a valid group selected;
- Each group operation must have at least the Min or Max checkbox set;
- When 'as percentage' checkbox is set, for the 'At least' and 'At most' validation types, 
  the value for the number of items must be between 0 and 100.
";

		public const string COMBINATIONS_DISPLAY_HELP_TEXT = @"=== COMBINATIONS DISPLAY INFORMATION ===

- Press CTRL+Click on a combination row (upper table) to compare it with other selected ones (lower table).

- Double click an item stat value, in the lower table, to open the breakdown of that stat for that combination.

- Item stats are ordered like this: 
	FIRST: stats that were chosen in the custom tiers, then ordered by their total weight contribution (highest to lowest).
	SECOND: stats chosen in the custom validator
	THIRD: all other stats present on any item in the combination, that are not already in the other two categories.

";
	}
}
