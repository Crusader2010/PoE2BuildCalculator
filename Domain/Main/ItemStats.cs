using Domain.Helpers;
using System.ComponentModel;

namespace Domain.Main
{
    public class ItemStats
    {
        [Description("Maximum Life")]
        [ItemStatsHelper.StatColumn(1)]
        public int MaximumLifeAmount { get; set; }

        [Description("Maximum Life%")]
        [ItemStatsHelper.StatColumn(2)]
        public double MaximumLifePercent { get; set; }

        [Description("Maximum Mana")]
        [ItemStatsHelper.StatColumn(3)]
        public int MaximumManaAmount { get; set; }

        [Description("Energy Shield")]
        [ItemStatsHelper.StatColumn(4)]
        public int EnergyShieldAmount { get; set; }

        [Description("Energy Shield%")]
        [ItemStatsHelper.StatColumn(5)]
        public double EnergyShieldPercent { get; set; }

        [Description("Spirit")]
        [ItemStatsHelper.StatColumn(6)]
        public int SpiritAmount { get; set; }

        [Description("Armour Amount Implicit")]
        [ItemStatsHelper.StatColumn(7)]
        public int ArmourAmountImplicit { get; set; }

        [Description("Armour Amount Explicit")]
        [ItemStatsHelper.StatColumn(8)]
        public int ArmourAmountExplicit { get; set; }

        [Description("Armour%")]
        [ItemStatsHelper.StatColumn(9)]
        public double ArmourPercent { get; set; }

        [Description("Mana Regen%")]
        [ItemStatsHelper.StatColumn(10)]
        public double ManaRegenPercent { get; set; }

        [Description("Mana Regen")]
        [ItemStatsHelper.StatColumn(11)]
        public double ManaRegenAmount { get; set; }

        [Description("Life Regen")]
        [ItemStatsHelper.StatColumn(12)]
        public double LifeRegenAmount { get; set; }

        [Description("Stun Threshold")]
        [ItemStatsHelper.StatColumn(13)]
        public int StunThresholdAmount { get; set; }

        [Description("Block Chance%")]
        [ItemStatsHelper.StatColumn(14)]
        public double BlockChancePercent { get; set; }

        [Description("Physical Damage Reduction%")]
        [ItemStatsHelper.StatColumn(15)]
        public double PhysicalDamageReductionPercent { get; set; }

        [Description("Fire Resistance%")]
        [ItemStatsHelper.StatColumn(20)]
        public double FireResistancePercent { get; set; }

        [Description("Lightning Resistance%")]
        [ItemStatsHelper.StatColumn(21)]
        public double LightningResistancePercent { get; set; }

        [Description("Cold Resistance%")]
        [ItemStatsHelper.StatColumn(22)]
        public double ColdResistancePercent { get; set; }

        [Description("Chaos Resistance%")]
        [ItemStatsHelper.StatColumn(23)]
        public double ChaosResistancePercent { get; set; }

        [Description("All Elemental Resistances%")]
        [ItemStatsHelper.StatColumn(24)]
        public double AllElementalResistancesPercent { get; set; }

        [Description("Fire Damage%")]
        [ItemStatsHelper.StatColumn(30)]
        public double FireDamagePercent { get; set; }

        [Description("Lightning Damage%")]
        [ItemStatsHelper.StatColumn(31)]
        public double LightningDamagePercent { get; set; }

        [Description("Cold Damage%")]
        [ItemStatsHelper.StatColumn(32)]
        public double ColdDamagePercent { get; set; }

        [Description("Chaos Damage%")]
        [ItemStatsHelper.StatColumn(33)]
        public double ChaosDamagePercent { get; set; }

        [Description("All Damage%")]
        [ItemStatsHelper.StatColumn(34)]
        public double AllDamagePercent { get; set; }

        [Description("Physical Damage%")]
        [ItemStatsHelper.StatColumn(35)]
        public double PhysicalDamagePercent { get; set; }

        [Description("Spell Damage%")]
        [ItemStatsHelper.StatColumn(36)]
        public double SpellDamagePercent { get; set; }

        [Description("Physical Thorns Min")]
        [ItemStatsHelper.StatColumn(40)]
        public int PhysicalThornsMinDamageAmount { get; set; }

        [Description("Physical Thorns Max")]
        [ItemStatsHelper.StatColumn(41)]
        public int PhysicalThornsMaxDamageAmount { get; set; }

        [Description("Cold Spell Skills Level")]
        [ItemStatsHelper.StatColumn(50)]
        public int ColdSpellSkillsLevel { get; set; }

        [Description("Lightning Spell Skills Level")]
        [ItemStatsHelper.StatColumn(51)]
        public int LightningSpellSkillsLevel { get; set; }

        [Description("Fire Spell Skills Level")]
        [ItemStatsHelper.StatColumn(52)]
        public int FireSpellSkillsLevel { get; set; }

        [Description("Chaos Spell Skills Level")]
        [ItemStatsHelper.StatColumn(53)]
        public int ChaosSpellSkillsLevel { get; set; }

        [Description("Cast Speed%")]
        [ItemStatsHelper.StatColumn(60)]
        public double CastSpeedPercent { get; set; }

        [Description("Attack Speed%")]
        [ItemStatsHelper.StatColumn(61)]
        public double AttackSpeedPercent { get; set; }

        [Description("Rarity%")]
        [ItemStatsHelper.StatColumn(62)]
        public double RarityPercent { get; set; }

        [Description("Strength")]
        [ItemStatsHelper.StatColumn(70)]
        public int Strength { get; set; }

        [Description("Dexterity")]
        [ItemStatsHelper.StatColumn(71)]
        public int Dexterity { get; set; }

        [Description("Intelligence")]
        [ItemStatsHelper.StatColumn(72)]
        public int Intelligence { get; set; }

        [Description("All Attributes")]
        [ItemStatsHelper.StatColumn(73)]
        public int AllAttributes { get; set; }

        [Description("Corrupted?")]
        [ItemStatsHelper.StatColumn(74)]
        public string Corrupted { get; set; } = "NO";

        [Description("Potential?")]
        [ItemStatsHelper.StatColumn(75)]
        public string Potential { get; set; } = "NO";

        [Description("Enchant")]
        [ItemStatsHelper.StatColumn(80)]
        public string Enchant { get; set; } = string.Empty;
    }
}
