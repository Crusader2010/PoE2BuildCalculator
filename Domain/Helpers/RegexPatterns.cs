using System.Text.RegularExpressions;

namespace Domain
{
	public static partial class RegexPatterns
	{
		[GeneratedRegex(@"^\s*Item\s*class\s*:\s*(.+?)\s*$", RegexOptions.IgnoreCase, "en-US")]
		public static partial Regex ItemClassPattern();

		[GeneratedRegex(@"^\s*Armour\s*:\s*(\d+)\s*$", RegexOptions.IgnoreCase, "en-US")]
		public static partial Regex ArmourAmountImplicitPattern();

		[GeneratedRegex(@"^\s*Energy\s*shield\s*:\s*(\d+)\s*$", RegexOptions.IgnoreCase, "en-US")]
		public static partial Regex EnergyShieldAmountImplicitPattern();

		[GeneratedRegex(@"^\s*\+(\d+)\s*to\s*maximum\s*Life\s*$", RegexOptions.IgnoreCase, "en-US")]
		public static partial Regex MaximumLifeAmountPattern();

		[GeneratedRegex(@"^\s*\+(\d+)\s*to\s*maximum\s*Mana\s*$", RegexOptions.IgnoreCase, "en-US")]
		public static partial Regex MaximumManaAmountPattern();

		[GeneratedRegex(@"^\s*\+(\d+)\s*to\s*Spirit\s*$", RegexOptions.IgnoreCase, "en-US")]
		public static partial Regex SpiritAmountPattern();

		// Match "+X to Armour"
		[GeneratedRegex(@"^\s*\+(\d+)\s*to\s*Armour\s*$", RegexOptions.IgnoreCase, "en-US")]
		public static partial Regex ArmourAmountExplicitPattern();

		[GeneratedRegex(@"^\s*\+(\d+)\s*to\s*Stun\s*threshold\s*$", RegexOptions.IgnoreCase, "en-US")]
		public static partial Regex StunThresholdAmountPattern();

		[GeneratedRegex(@"^\s*(\d+)\s*to\s*(\d+)\s*Physical\s*Thorns\s*damage\s*$", RegexOptions.IgnoreCase, "en-US")]
		public static partial Regex PhysicalThornsRangePattern();

		[GeneratedRegex(@"^\s*\+(\d+)\s*to\s*Level\s*of\s*all\s*Cold\s*Spell\s*Skills\s*$", RegexOptions.IgnoreCase, "en-US")]
		public static partial Regex ColdSpellSkillsLevelPattern();

		[GeneratedRegex(@"^\s*\+(\d+)\s*to\s*Level\s*of\s*all\s*Fire\s*Spell\s*Skills\s*$", RegexOptions.IgnoreCase, "en-US")]
		public static partial Regex FireSpellSkillsLevelPattern();

		[GeneratedRegex(@"^\s*\+(\d+)\s*to\s*Level\s*of\s*all\s*Lightning\s*Spell\s*Skills\s*$", RegexOptions.IgnoreCase, "en-US")]
		public static partial Regex LightningSpellSkillsLevelPattern();

		[GeneratedRegex(@"^\s*\+(\d+)\s*to\s*Level\s*of\s*all\s*Chaos\s*Spell\s*Skills\s*$", RegexOptions.IgnoreCase, "en-US")]
		public static partial Regex ChaosSpellSkillsLevelPattern();

		[GeneratedRegex(@"^\s*\+(\d+)\s*to\s*Strength\s*$", RegexOptions.IgnoreCase, "en-US")]
		public static partial Regex StrengthAmountPattern();

		[GeneratedRegex(@"^\s*\+(\d+)\s*to\s*Dexterity\s*$", RegexOptions.IgnoreCase, "en-US")]
		public static partial Regex DexterityAmountPattern();

		[GeneratedRegex(@"^\s*\+(\d+)\s*to\s*Intelligence\s*$", RegexOptions.IgnoreCase, "en-US")]
		public static partial Regex IntelligenceAmountPattern();

		[GeneratedRegex(@"^\s*\+(\d+)\s*to\s*all\s*attributes\s*$", RegexOptions.IgnoreCase, "en-US")]
		public static partial Regex AllAttributesAmountPattern();


		// Percent forms using "X% increased <stat>"
		[GeneratedRegex(@"^\s*(\d+(?:\.\d+)?)%\s*increased\s*maximum\s*Life\s*$", RegexOptions.IgnoreCase, "en-US")]
		public static partial Regex MaximumLifePercentPattern();

		[GeneratedRegex(@"^\s*(\d+(?:\.\d+)?)%\s*increased\s*Energy\s*shield\s*$", RegexOptions.IgnoreCase, "en-US")]
		public static partial Regex EnergyShieldPercentPattern();

		[GeneratedRegex(@"^\s*(\d+(?:\.\d+)?)%\s*increased\s*Armour\s*$", RegexOptions.IgnoreCase, "en-US")]
		public static partial Regex ArmourPercentPattern();

		[GeneratedRegex(@"^\s*(\d+(?:\.\d+)?)%\s*increased\s*Mana\s*Regeneration\s*rate\s*$", RegexOptions.IgnoreCase, "en-US")]
		public static partial Regex ManaRegenPercentPattern();

		[GeneratedRegex(@"^\s*(\d+(?:\.\d+)?)\s*Life\s*Regeneration\s*per\s*second\s*$", RegexOptions.IgnoreCase, "en-US")]
		public static partial Regex LifeRegenAmountPattern();

		[GeneratedRegex(@"^\s*(\d+(?:\.\d+)?)%\s*increased\s*Block\s*Chance\s*$", RegexOptions.IgnoreCase, "en-US")]
		public static partial Regex BlockChancePercentPattern();

		[GeneratedRegex(@"^\s*(\d+(?:\.\d+)?)%\s*additional\s*Physical\s*Damage\s*Reduction\s*$", RegexOptions.IgnoreCase, "en-US")]
		public static partial Regex PhysicalDamageReductionPercentPattern();

		// Resistances use "+X% to <stat>"
		[GeneratedRegex(@"^\s*\+(\d+(?:\.\d+)?)%\s*to\s*Fire\s*Resistance\s*$", RegexOptions.IgnoreCase, "en-US")]
		public static partial Regex FireResistancePercentPattern();

		[GeneratedRegex(@"^\s*\+(\d+(?:\.\d+)?)%\s*to\s*Lightning\s*Resistance\s*$", RegexOptions.IgnoreCase, "en-US")]
		public static partial Regex LightningResistancePercentPattern();

		[GeneratedRegex(@"^\s*\+(\d+(?:\.\d+)?)%\s*to\s*Cold\s*Resistance\s*$", RegexOptions.IgnoreCase, "en-US")]
		public static partial Regex ColdResistancePercentPattern();

		[GeneratedRegex(@"^\s*\+(\d+(?:\.\d+)?)%\s*to\s*Chaos\s*Resistance\s*$", RegexOptions.IgnoreCase, "en-US")]
		public static partial Regex ChaosResistancePercentPattern();

		[GeneratedRegex(@"^\s*\+(\d+(?:\.\d+)?)%\s*to\s*all\s*elemental\s*resistances\s*$", RegexOptions.IgnoreCase, "en-US")]
		public static partial Regex AllElementalResistancesPercentPattern();

		// Damage / % increases use "X% increased <stat>"
		[GeneratedRegex(@"^\s*(\d+(?:\.\d+)?)%\s*increased\s*Fire\s*Damage\s*$", RegexOptions.IgnoreCase, "en-US")]
		public static partial Regex FireDamagePercentPattern();

		[GeneratedRegex(@"^\s*(\d+(?:\.\d+)?)%\s*increased\s*Lightning\s*Damage\s*$", RegexOptions.IgnoreCase, "en-US")]
		public static partial Regex LightningDamagePercentPattern();

		[GeneratedRegex(@"^\s*(\d+(?:\.\d+)?)%\s*increased\s*Cold\s*Damage\s*$", RegexOptions.IgnoreCase, "en-US")]
		public static partial Regex ColdDamagePercentPattern();

		[GeneratedRegex(@"^\s*(\d+(?:\.\d+)?)%\s*increased\s*Chaos\s*Damage\s*$", RegexOptions.IgnoreCase, "en-US")]
		public static partial Regex ChaosDamagePercentPattern();

		[GeneratedRegex(@"^\s*(\d+(?:\.\d+)?)%\s*increased\s*Damage\s*$", RegexOptions.IgnoreCase, "en-US")]
		public static partial Regex AllDamagePercentPattern();

		[GeneratedRegex(@"^\s*(\d+(?:\.\d+)?)%\s*increased\s*Physical\s*Damage\s*$", RegexOptions.IgnoreCase, "en-US")]
		public static partial Regex PhysicalDamagePercentPattern();

		[GeneratedRegex(@"^\s*(\d+(?:\.\d+)?)%\s*increased\s*Spell\s*Damage\s*$", RegexOptions.IgnoreCase, "en-US")]
		public static partial Regex SpellDamagePercentPattern();

		// Speed / Rarity as "X% increased <stat>"
		[GeneratedRegex(@"^\s*(\d+(?:\.\d+)?)%\s*increased\s*Cast\s*Speed\s*$", RegexOptions.IgnoreCase, "en-US")]
		public static partial Regex CastSpeedPercentPattern();

		[GeneratedRegex(@"^\s*(\d+(?:\.\d+)?)%\s*increased\s*Attack\s*Speed\s*$", RegexOptions.IgnoreCase, "en-US")]
		public static partial Regex AttackSpeedPercentPattern();

		[GeneratedRegex(@"^\s*(\d+(?:\.\d+)?)%\s*increased\s*Rarity\s*of\s*items\s*found\s*$", RegexOptions.IgnoreCase, "en-US")]
		public static partial Regex RarityPercentPattern();

		// Enchant: capture text immediately before "(enchant)" on a line that ends with it.
		[GeneratedRegex(@"^\s*(.+?)\s*\(enchant\)\s*$", RegexOptions.IgnoreCase, "en-US")]
		public static partial Regex EnchantPattern();
	}
}
