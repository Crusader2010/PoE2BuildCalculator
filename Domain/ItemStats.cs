using System.ComponentModel;

namespace Domain
{
    public class ItemStats
    {
        [Description("Maximum Life")]
        [PropertyDescriptionHelper.StatColumn(1)]
        public int MaximumLifeAmount { get; set; }

        [Description("Maximum Life%")]
        [PropertyDescriptionHelper.StatColumn(2)]
        public double MaximumLifePercent { get; set; }

        [Description("Maximum Mana")]
        [PropertyDescriptionHelper.StatColumn(3)]
        public int MaximumManaAmount { get; set; }

        [Description("Energy Shield")]
        [PropertyDescriptionHelper.StatColumn(4)]
        public int EnergyShieldAmount { get; set; }

        [Description("Energy Shield%")]
        [PropertyDescriptionHelper.StatColumn(5)]
        public double EnergyShieldPercent { get; set; }

        [Description("Spirit")]
        [PropertyDescriptionHelper.StatColumn(6)]
        public int SpiritAmount { get; set; }

        [Description("Armour Amount Implicit")]
        [PropertyDescriptionHelper.StatColumn(7)]
        public int ArmourAmountImplicit { get; set; }

        [Description("Armour Amount Explicit")]
        [PropertyDescriptionHelper.StatColumn(8)]
        public int ArmourAmountExplicit { get; set; }

        [Description("Armour%")]
        [PropertyDescriptionHelper.StatColumn(9)]
        public double ArmourPercent { get; set; }

        [Description("Mana Regen%")]
        [PropertyDescriptionHelper.StatColumn(10)]
        public double ManaRegenPercent { get; set; }

        [Description("Mana Regen")]
        [PropertyDescriptionHelper.StatColumn(11)]
        public double ManaRegenAmount { get; set; }

        [Description("Life Regen")]
        [PropertyDescriptionHelper.StatColumn(12)]
        public double LifeRegenAmount { get; set; }

        [Description("Stun Threshold")]
        [PropertyDescriptionHelper.StatColumn(13)]
        public int StunThresholdAmount { get; set; }

        [Description("Block Chance%")]
        [PropertyDescriptionHelper.StatColumn(14)]
        public double BlockChancePercent { get; set; }

        [Description("Physical Damage Reduction%")]
        [PropertyDescriptionHelper.StatColumn(15)]
        public double PhysicalDamageReductionPercent { get; set; }

        [Description("Fire Resistance%")]
        [PropertyDescriptionHelper.StatColumn(20)]
        public double FireResistancePercent { get; set; }

        [Description("Lightning Resistance%")]
        [PropertyDescriptionHelper.StatColumn(21)]
        public double LightningResistancePercent { get; set; }

        [Description("Cold Resistance%")]
        [PropertyDescriptionHelper.StatColumn(22)]
        public double ColdResistancePercent { get; set; }

        [Description("Chaos Resistance%")]
        [PropertyDescriptionHelper.StatColumn(23)]
        public double ChaosResistancePercent { get; set; }

        [Description("All Elemental Resistances%")]
        [PropertyDescriptionHelper.StatColumn(24)]
        public double AllElementalResistancesPercent { get; set; }

        [Description("Fire Damage%")]
        [PropertyDescriptionHelper.StatColumn(30)]
        public double FireDamagePercent { get; set; }

        [Description("Lightning Damage%")]
        [PropertyDescriptionHelper.StatColumn(31)]
        public double LightningDamagePercent { get; set; }

        [Description("Cold Damage%")]
        [PropertyDescriptionHelper.StatColumn(32)]
        public double ColdDamagePercent { get; set; }

        [Description("Chaos Damage%")]
        [PropertyDescriptionHelper.StatColumn(33)]
        public double ChaosDamagePercent { get; set; }

        [Description("All Damage%")]
        [PropertyDescriptionHelper.StatColumn(34)]
        public double AllDamagePercent { get; set; }

        [Description("Physical Damage%")]
        [PropertyDescriptionHelper.StatColumn(35)]
        public double PhysicalDamagePercent { get; set; }

        [Description("Spell Damage%")]
        [PropertyDescriptionHelper.StatColumn(36)]
        public double SpellDamagePercent { get; set; }

        [Description("Physical Thorns Min")]
        [PropertyDescriptionHelper.StatColumn(40)]
        public int PhysicalThornsMinDamageAmount { get; set; }

        [Description("Physical Thorns Max")]
        [PropertyDescriptionHelper.StatColumn(41)]
        public int PhysicalThornsMaxDamageAmount { get; set; }

        [Description("Cold Spell Skills Level")]
        [PropertyDescriptionHelper.StatColumn(50)]
        public int ColdSpellSkillsLevel { get; set; }

        [Description("Lightning Spell Skills Level")]
        [PropertyDescriptionHelper.StatColumn(51)]
        public int LightningSpellSkillsLevel { get; set; }

        [Description("Fire Spell Skills Level")]
        [PropertyDescriptionHelper.StatColumn(52)]
        public int FireSpellSkillsLevel { get; set; }

        [Description("Chaos Spell Skills Level")]
        [PropertyDescriptionHelper.StatColumn(53)]
        public int ChaosSpellSkillsLevel { get; set; }

        [Description("Cast Speed%")]
        [PropertyDescriptionHelper.StatColumn(60)]
        public double CastSpeedPercent { get; set; }

        [Description("Attack Speed%")]
        [PropertyDescriptionHelper.StatColumn(61)]
        public double AttackSpeedPercent { get; set; }

        [Description("Rarity%")]
        [PropertyDescriptionHelper.StatColumn(62)]
        public double RarityPercent { get; set; }

        [Description("Strength")]
        [PropertyDescriptionHelper.StatColumn(70)]
        public int Strength { get; set; }

        [Description("Dexterity")]
        [PropertyDescriptionHelper.StatColumn(71)]
        public int Dexterity { get; set; }

        [Description("Intelligence")]
        [PropertyDescriptionHelper.StatColumn(72)]
        public int Intelligence { get; set; }

        [Description("All Attributes")]
        [PropertyDescriptionHelper.StatColumn(73)]
        public int AllAttributes { get; set; }

        [Description("Enchant")]
        [PropertyDescriptionHelper.StatColumn(80)]
        public string Enchant { get; set; } = string.Empty;
    }
}
