using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Test
{
    class ThreadSafetyDemo
    {
        static readonly ConcurrentDictionary<int, int> _dict = new();

        static void Main()
        {
            Parallel.For(0, 8, idx =>
            {
                for (int i = 0; i < 1000; i++)
                {
                    _dict[i] = i + 1;
                }
            });

            foreach (var kvp in _dict)
            {
                Console.Write($"{kvp} ");
            }

            Console.ReadKey();
        }
    }
}
