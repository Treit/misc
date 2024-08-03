using System;
using System.Threading;
using System.Threading.Tasks;

namespace Test
{
    class ThreadSafetyDemo
    {
        static Random _random = new Random();

        static void Main()
        {
            long iters = 0L;

            Parallel.For(0, 8, idx =>
            {
                while (true)
                {
                    int r = _random.Next(1000);

                    if (iters++ % 2000 == 0)
                    {
                        Console.Write($"{r} ");
                        Thread.Sleep(50);
                    }
                }
            });

            Console.ReadKey();
        }
    }
}
