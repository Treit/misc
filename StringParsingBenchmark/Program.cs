namespace StringParsingBenchmark
{
    using BenchmarkDotNet.Running;
    using System;

    internal class Program
    {
        static void Main(string[] args)
        {
            if (args.Length == 0)
            {
                Console.WriteLine("Provide a file to process.");
                return;
            }
#if DEBUG
            var benchmark = new Benchmark();
            benchmark.GlobalSetup();
            benchmark.Iterations = 1;
            benchmark.TestA();
            benchmark.TestB();
#else
            BenchmarkRunner.Run<Benchmark>();
#endif
        }
    }
}
