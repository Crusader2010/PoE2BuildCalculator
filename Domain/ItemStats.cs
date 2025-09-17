using System.ComponentModel;

namespace Domain
{
    public class ItemStats
    {
        [Description("Maximum Life")]
        [PropertyDescriptionHelper.StatColumnAttribute(1)]
        public int MaximumLifeAmount { get; set; }

        [Description("Maximum Life%")]
        [PropertyDescriptionHelper.StatColumnAttribute(2)]
        public double MaximumLifePercent { get; set; }

        [Description("Maximum Mana")]
        [PropertyDescriptionHelper.StatColumnAttribute(3)]
        public int MaximumManaAmount { get; set; }

        [Description("Energy Shield")]
        [PropertyDescriptionHelper.StatColumnAttribute(4)]
        public int EnergyShieldAmount { get; set; }

        [Description("Energy Shield%")]
        [PropertyDescriptionHelper.StatColumnAttribute(5)]
        public double EnergyShieldPercent { get; set; }

        [Description("Spirit")]
        [PropertyDescriptionHelper.StatColumnAttribute(6)]
        public int SpiritAmount { get; set; }

        [Description("Armour Amount Implicit")]
        [PropertyDescriptionHelper.StatColumnAttribute(7)]
        public int ArmourAmountImplicit { get; set; }

        [Description("Armour Amount Explicit")]
        [PropertyDescriptionHelper.StatColumnAttribute(8)]
        public int ArmourAmountExplicit { get; set; }

        [Description("Armour%")]
        [PropertyDescriptionHelper.StatColumnAttribute(9)]
        public double ArmourPercent { get; set; }

        [Description("Mana Regen%")]
        [PropertyDescriptionHelper.StatColumnAttribute(10)]
        public double ManaRegenPercent { get; set; }

        [Description("Mana Regen")]
        [PropertyDescriptionHelper.StatColumnAttribute(11)]
        public double ManaRegenAmount { get; set; }

        [Description("Life Regen")]
        [PropertyDescriptionHelper.StatColumnAttribute(12)]
        public double LifeRegenAmount { get; set; }

        [Description("Stun Threshold")]
        [PropertyDescriptionHelper.StatColumnAttribute(13)]
        public int StunThresholdAmount { get; set; }

        [Description("Block Chance%")]
        [PropertyDescriptionHelper.StatColumnAttribute(14)]
        public double BlockChancePercent { get; set; }

        [Description("Physical Damage Reduction%")]
        [PropertyDescriptionHelper.StatColumnAttribute(15)]
        public double PhysicalDamageReductionPercent { get; set; }

        [Description("Fire Resistance%")]
        [PropertyDescriptionHelper.StatColumnAttribute(20)]
        public double FireResistancePercent { get; set; }

        [Description("Lightning Resistance%")]
        [PropertyDescriptionHelper.StatColumnAttribute(21)]
        public double LightningResistancePercent { get; set; }

        [Description("Cold Resistance%")]
        [PropertyDescriptionHelper.StatColumnAttribute(22)]
        public double ColdResistancePercent { get; set; }

        [Description("Chaos Resistance%")]
        [PropertyDescriptionHelper.StatColumnAttribute(23)]
        public double ChaosResistancePercent { get; set; }

        [Description("All Elemental Resistances%")]
        [PropertyDescriptionHelper.StatColumnAttribute(24)]
        public double AllElementalResistancesPercent { get; set; }

        [Description("Fire Damage%")]
        [PropertyDescriptionHelper.StatColumnAttribute(30)]
        public double FireDamagePercent { get; set; }

        [Description("Lightning Damage%")]
        [PropertyDescriptionHelper.StatColumnAttribute(31)]
        public double LightningDamagePercent { get; set; }

        [Description("Cold Damage%")]
        [PropertyDescriptionHelper.StatColumnAttribute(32)]
        public double ColdDamagePercent { get; set; }

        [Description("Chaos Damage%")]
        [PropertyDescriptionHelper.StatColumnAttribute(33)]
        public double ChaosDamagePercent { get; set; }

        [Description("All Damage%")]
        [PropertyDescriptionHelper.StatColumnAttribute(34)]
        public double AllDamagePercent { get; set; }

        [Description("Physical Damage%")]
        [PropertyDescriptionHelper.StatColumnAttribute(35)]
        public double PhysicalDamagePercent { get; set; }

        [Description("Spell Damage%")]
        [PropertyDescriptionHelper.StatColumnAttribute(36)]
        public double SpellDamagePercent { get; set; }

        [Description("Physical Thorns Min")]
        [PropertyDescriptionHelper.StatColumnAttribute(40)]
        public int PhysicalThornsMinDamageAmount { get; set; }

        [Description("Physical Thorns Max")]
        [PropertyDescriptionHelper.StatColumnAttribute(41)]
        public int PhysicalThornsMaxDamageAmount { get; set; }

        [Description("Cold Spell Skills Level")]
        [PropertyDescriptionHelper.StatColumnAttribute(50)]
        public int ColdSpellSkillsLevel { get; set; }

        [Description("Lightning Spell Skills Level")]
        [PropertyDescriptionHelper.StatColumnAttribute(51)]
        public int LightningSpellSkillsLevel { get; set; }

        [Description("Fire Spell Skills Level")]
        [PropertyDescriptionHelper.StatColumnAttribute(52)]
        public int FireSpellSkillsLevel { get; set; }

        [Description("Chaos Spell Skills Level")]
        [PropertyDescriptionHelper.StatColumnAttribute(53)]
        public int ChaosSpellSkillsLevel { get; set; }

        [Description("Cast Speed%")]
        [PropertyDescriptionHelper.StatColumnAttribute(60)]
        public double CastSpeedPercent { get; set; }

        [Description("Attack Speed%")]
        [PropertyDescriptionHelper.StatColumnAttribute(61)]
        public double AttackSpeedPercent { get; set; }

        [Description("Rarity%")]
        [PropertyDescriptionHelper.StatColumnAttribute(62)]
        public double RarityPercent { get; set; }

        [Description("Strength")]
        [PropertyDescriptionHelper.StatColumnAttribute(70)]
        public int Strength { get; set; }

        [Description("Dexterity")]
        [PropertyDescriptionHelper.StatColumnAttribute(71)]
        public int Dexterity { get; set; }

        [Description("Intelligence")]
        [PropertyDescriptionHelper.StatColumnAttribute(72)]
        public int Intelligence { get; set; }

        [Description("All Attributes")]
        [PropertyDescriptionHelper.StatColumnAttribute(73)]
        public int AllAttributes { get; set; }

        [Description("Enchant")]
        [PropertyDescriptionHelper.StatColumnAttribute(80)]
        public string Enchant { get; set; } = string.Empty;
    }
}
