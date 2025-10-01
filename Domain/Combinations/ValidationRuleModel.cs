using System.ComponentModel;
using System.Reflection;

namespace Domain.Combinations
{
    public class ValidationRuleModel
    {
        // Data properties
        public string PropertyName { get; set; }
        public bool SumAtLeastEnabled { get; set; }
        public string SumAtLeastValue { get; set; }
        public bool SumAtMostEnabled { get; set; }
        public string SumAtMostValue { get; set; }
        public bool EachAtLeastEnabled { get; set; }
        public string EachAtLeastValue { get; set; }
        public bool EachAtMostEnabled { get; set; }
        public string EachAtMostValue { get; set; }

        // --- Operators ---
        public string Op1 { get; set; } = "AND"; // Operator between Sum>= and Sum<=
        public string Op2 { get; set; } = "AND"; // Operator between Sum<= and Each>=
        public string Op3 { get; set; } = "AND"; // Operator between Each>= and Each<=
        public string RowOperator { get; set; } = "AND"; // Operator for combining with the NEXT row

        // Metadata - not shown in the grid, but crucial for logic
        [Browsable(false)]
        public PropertyInfo PropInfo { get; set; }

        [Browsable(false)]
        public bool IsActive => SumAtLeastEnabled || SumAtMostEnabled || EachAtLeastEnabled || EachAtMostEnabled;
    }
}
