namespace Domain
{
    public class ItemStats
    {
        public int MaximumLifeAmount { get; set; }
        public double MaximumLifePercent { get; set; }
        public int MaximumMana { get; set; }
        public int EnergyShieldAmount { get; set; }
        public double EnergyShieldPercent { get; set; }
        public int SpiritAmount { get; set; }
        public int ArmourAmount { get; set; }
        public double ArmourPercent { get; set; }
        public double ManaRegenPercent { get; set; }
        public double ManaRegenAmount { get; set; }
        public double LifeRegenAmount{ get; set; }
        public int StunThresholdAmount { get; set; }
        public double BlockChancePercent { get; set; }
        public double PhysicalDamageReductionPercent { get; set; }


        public double FireResistancePercent { get; set; }
        public double LightningResistancePercent { get; set; }
        public double ColdResistancePercent { get; set; }
        public double ChaosResistancePercent { get; set; }
        public double AllElementalResistancesPercent { get; set; }


        public double FireDamagePercent { get; set; }
        public double LightningDamagePercent { get; set; }
        public double ColdDamagePercent { get; set; }
        public double ChaosDamagePercent { get; set; }
        public double AllDamagePercent { get; set; }
        public double PhysicalDamagePercent { get; set; }
        public double SpellDamagePercent { get; set; }
        public int PhysicalThornsMinDamageAmount { get; set; }
        public int PhysicalThornsMaxDamageAmount { get; set; }

        public int ColdSpellSkillsLevel { get; set; }
        public int LightningSpellSkillsLevel { get; set; }
        public int FireSpellSkillsLevel { get; set; }
        public int ChaosSpellSkillsLevel { get; set; }

        public double CastSpeedPercent { get; set; }
        public double AttackSpeedPercent { get; set; }
        public double RarityPercent { get; set; }

        public int Strength { get; set; }
        public int Dexterity { get; set; }
        public int Intelligence { get; set; }
        public int AllAttributes { get; set; }

        public string Enchant { get; set; } = string.Empty;
    }
}
