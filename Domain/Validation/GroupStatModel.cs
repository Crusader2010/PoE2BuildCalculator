using System.Reflection;

using Domain.Enums;

namespace Domain.Validation
{
	public class GroupStatModel
	{
		public string PropertyName { get; set; }
		public PropertyInfo PropInfo { get; set; }
		public ArithmeticOperationsEnum? Operator { get; set; }
	}
}
