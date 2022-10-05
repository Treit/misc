using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Test
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length < 1 || !int.TryParse(args[0], out int num))
            {
                Console.WriteLine("Provide a number of predicates to apply.");
                return;
            }

            var sw = Stopwatch.StartNew();
            Console.WriteLine("About to run the test...");
            IEnumerable<int> rows = new int[] { 1234 };
            var predicates = new Func<int, bool>[num];

            for (int i = 0; i < num; i++)
            {
                predicates[i] = x => true;
            }

            foreach (var predicate in predicates)
            {
                rows = rows.AsParallel().Where(predicate);
            }

            var x = rows.ToList();
            Console.WriteLine($"Test finished after {sw.ElapsedMilliseconds} ms.");
        }
    }
}
