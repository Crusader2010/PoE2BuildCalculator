namespace Domain.Validation
{
    public class GroupStatModel
    {
        public string PropertyName { get; set; }
        public string Operator { get; set; } = "+"; // +, -, *, /
    }
}
