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
    }
}
