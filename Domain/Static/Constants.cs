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
        public readonly static ImmutableList<string> ITEM_CLASSES =
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

        public static readonly ImmutableArray<string> MATH_OPERATORS = ["+", "-", "*", "/"];
        public static readonly ImmutableArray<string> LOGICAL_OPERATORS = ["AND", "OR", "XOR"];

        public const string VALIDATOR_HELP_TEXT = @"=== ORDER OF OPERATIONS ===

WITHIN A GROUP (Stats):
Stats are evaluated LEFT-TO-RIGHT in the order they appear.
Example: If you have:
  • MaxLife (+)
  • Armour% (-)
  • Spirit (*)
  • next_stat

Calculation: ((MaxLife + Armour%) - Spirit) * next_stat
This is LEFT-ASSOCIATIVE evaluation.

To control order:
1. Reorder stats using ▲▼ buttons
2. First stat evaluated first
3. Each operator applies between result and next stat

BETWEEN GROUPS:
Groups evaluated in grid order (left→right, top→bottom).
Each group produces TRUE/FALSE based on Min/Max constraints.

Results combined using group operators (AND/OR/XOR):
  • AND: Both groups must pass
  • OR: At least one group must pass  
  • XOR: Exactly one group must pass

Example with 3 groups:
  Group1 (TRUE) → AND
  Group2 (FALSE) → OR
  Group3 (TRUE)

Evaluation: (TRUE AND FALSE) OR TRUE = FALSE OR TRUE = TRUE

CONSTRAINTS:
Each group sums all item stats per its expression,
then checks if sum is within Min/Max bounds.

Min/Max can be 0 or negative.
At least one constraint (Min OR Max) must be enabled.";
    }
}
