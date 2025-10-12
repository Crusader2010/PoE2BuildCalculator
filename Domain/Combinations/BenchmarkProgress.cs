namespace Domain.Combinations
{
    public class BenchmarkProgress
    {
        public ulong CurrentIteration { get; set; }
        public ulong TotalIterations { get; set; }
        public double PercentComplete { get; set; }
        public string StatusMessage { get; set; }
    }
}
