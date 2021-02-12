namespace Test
{
    using BenchmarkDotNet.Attributes;
    using BenchmarkDotNet.Diagnosers;
    using System.Collections.Concurrent;
    using System.Collections.Generic;

    public class SomeObject
    {
        public SomeObject(int val)
        {
            this.Val = val;
        }

        public int Val { get; }
    }

    [MemoryDiagnoser]
    public class Benchmark
    {
        BlockingCollection<string> _logbuffer = new BlockingCollection<string>();

        [Params(100, 10_000, 100_000, 1_000_000)]
        public int Iterations { get; set; }

        [GlobalSetup]
        public void GlobalSetup()
        {
        }

        [Benchmark]
        public long WithList()
        {
            List<SomeObject> list = new List<SomeObject>() { new SomeObject(1), new SomeObject(1), new SomeObject(1) };
            long result = 0;

            for (int i = 0; i < this.Iterations; i++)
            {
                foreach (SomeObject so in list)
                {
                    result += so.ToString().GetHashCode();
                }
            }

            return result;
        }

        [Benchmark]
        public long WithDict()
        {
            Dictionary<string, SomeObject> dict = new Dictionary<string, SomeObject>();

            dict["1"] = new SomeObject(1);
            dict["2"] = new SomeObject(2);
            dict["3"] = new SomeObject(3);

            long result = 0;

            for (int i = 0; i < this.Iterations; i++)
            {
                foreach (SomeObject so in dict.Values)
                {
                    result += so.ToString().GetHashCode();
                }
            }

            return result;
        }
    }
}
