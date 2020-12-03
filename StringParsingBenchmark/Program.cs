namespace StringParsingBenchmark
{
    using BenchmarkDotNet.Running;
    using System;

    internal class Program
    {
        static void Main(string[] args)
        {
#if DEBUG
            var benchmark = new Benchmark();
            benchmark.GlobalSetup();
            benchmark.Iterations = 1;
            benchmark.ParseWithSpan();
            benchmark.ParseWithStringSplit();
#else
            BenchmarkRunner.Run<Benchmark>();
#endif
        }
    }
}
