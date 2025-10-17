using System.ComponentModel;

namespace Domain.Enums
{
    public enum ArithmeticOperationsEnum
    {
        [Description("+")]
        Sum = 0,

        [Description("-")]
        Diff = 1,

        [Description("*")]
        Mult = 2,

        [Description("/")]
        Div = 3
    }
}
