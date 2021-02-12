namespace Test
{
    using BenchmarkDotNet.Running;
    using Microsoft.Diagnostics.Tracing.Parsers.FrameworkEventSource;
    using System;
    using System.Collections.Generic;

    internal class Program
    {
        static void Main(string[] args)
        {
            BenchmarkRunner.Run<Benchmark>();
        }
    }
}
