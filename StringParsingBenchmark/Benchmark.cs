namespace StringParsingBenchmark
{
    using BenchmarkDotNet.Attributes;
    using BenchmarkDotNet.Diagnosers;
    using System;
    using System.Diagnostics;
    using System.IO;

    [MemoryDiagnoser]
    public class Benchmark
    {
        string[] _lines;

        [Params(100, 1000, 10_000)]
        public int Iterations { get; set; }

        [GlobalSetup]
        public void GlobalSetup()
        {
            var inputFile = @"./input.txt";
            _lines = File.ReadAllLines(inputFile);
        }

        [Benchmark]
        public int ParseWithSpan()
        {
            return ParseFileWithSpan(_lines);
        }

        [Benchmark]
        public int ParseWithStringSplit()
        {
            return ParseFileWithSplit(_lines);
        }

        private int ParseFileWithSplit(string[] lines)
        {
            int result = 0;

            for (int i = 0; i < this.Iterations; i++)
            {
                result = 0;

                var sw = Stopwatch.StartNew();

                foreach (var line in lines)
                {
                    string[] tokens = line.Split(' ');
                    string valRange = tokens[0];
                    string[] valTokens = valRange.Split('-');
                    int valA = int.Parse(valTokens[0]);
                    int valB = int.Parse(valTokens[1]);
                    char requiredChar = tokens[1][0];
                    string password = tokens[2];
                    result += valA + valB + requiredChar + password.Length;
                }
            }

            return result;
        }

        private int ParseFileWithSpan(string[] lines)
        {
            int result = 0;

            for (int i = 0; i < this.Iterations; i++)
            {
                result = 0;
                var sw = Stopwatch.StartNew();

                foreach (var line in lines)
                {
                    ReadOnlySpan<char> span = line.AsSpan();
                    int loc = span.IndexOf(' ');
                    ReadOnlySpan<char> password = span.Slice(loc + 4);
                    ReadOnlySpan<char> countRange = span.Slice(0, loc);
                    char requiredChar = span.Slice(loc + 1, 1)[0];
                    loc = countRange.IndexOf('-');
                    ReadOnlySpan<char> valAStr = countRange.Slice(0, loc);
                    ReadOnlySpan<char> valBStr = countRange.Slice(loc + 1);
                    int valA = int.Parse(valAStr);
                    int valB = int.Parse(valBStr);

                    result += valA + valB + requiredChar + password.Length;
                }
            }

            return result;
        }
    }
}
