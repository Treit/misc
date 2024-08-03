using System;
using System.Threading;
using System.Threading.Tasks;

namespace Test
{
    class ThreadSafetyDemo2
    {
        static readonly object _syncobj = new();
        static Random _random = new Random();

        static void Main2()
        {
            long iters = 0L;

            Parallel.For(0, 8, idx =>
            {
                while (true)
                {
                    int r;
                    lock (_syncobj)
                    {
                        r = _random.Next(1000);
                    }

                    if (iters++ % 2000 == 0)
                    {
                        Console.Write($"{r}");
                        Thread.Sleep(50);
                    }
                }
            });

            Console.ReadKey();
        }
    }
}
