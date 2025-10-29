using System.ComponentModel;

namespace Domain.Enums
{
    public enum ValidationTypeEnum
    {
        [Description("SUM(all)")]
        SumALL = 0,

        [Description("Each item")]
        EachItem = 1,

        [Description("At least")]
        AtLeast = 2,

        [Description("At most")]
        AtMost = 3
    }
}
