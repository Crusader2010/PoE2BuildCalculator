using System.ComponentModel;

namespace Domain.Enums
{
    public enum GroupLevelOperatorsEnum
    {
        [Description("AND")]
        AND = 0,

        [Description("OR")]
        OR = 1,

        [Description("XOR")]
        XOR = 2,
    }
}
