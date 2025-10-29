using System.ComponentModel;

namespace Domain.Enums
{
    public enum MinMaxOperatorsEnum
    {
        [Description(">=")]
        GreaterEqual = 0,

        [Description(">")]
        Greater = 1,

        [Description("=")]
        Equal = 2,

        [Description("<")]
        Less = 3,

        [Description("<=")]
        LessEqual = 4,
    }
}
