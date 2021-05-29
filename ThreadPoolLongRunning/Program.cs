using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace ThreadPoolLongRunning
{
    class Program
    {
        private static int Running;

        static void Main(string[] args)
        {
            Test1();
            Test2();
        }

        static void Test1()
        {
            Stopwatch sw = Stopwatch.StartNew();

            List<Task> tasks = new(100);

            for (int i = 0; i < 100; i++)
            {
                Task t = Task.Factory.StartNew(() =>
                {
                    Interlocked.Increment(ref Running);
                    if (Running >= 100)
                    {
                        return;
                    }

                    while (true)
                    {
                        Thread.Sleep(TimeSpan.FromSeconds(5));
                    }
                }, TaskCreationOptions.None);

                tasks.Add(t);
            }

            while (true)
            {
                Thread.Sleep(500);
                if (Running >= 100)
                {
                    Console.WriteLine($"It took {sw.ElapsedMilliseconds} ms. to get all of the tasks running WITHOUT LongRunning.");
                    Running = 0;
                    return;
                }
                Console.WriteLine($"Running total threads: {Process.GetCurrentProcess().Threads.Count}");
                Console.WriteLine($"Running thread pool threads: {GetRunningThreadPoolThreads()}");
            }
        }

        static void Test2()
        {
            Stopwatch sw = Stopwatch.StartNew();

            List<Task> tasks = new(100);

            for (int i = 0; i < 100; i++)
            {
                Task t = Task.Factory.StartNew(() =>
                {
                    Interlocked.Increment(ref Running);
                    if (Running >= 100)
                    {
                        return;
                    }

                    while (true)
                    {
                        Thread.Sleep(TimeSpan.FromSeconds(5));
                    }
                }, TaskCreationOptions.LongRunning);

                tasks.Add(t);
            }

            while (true)
            {
                Thread.Sleep(500);
                if (Running >= 100)
                {
                    Running = 0;
                    Console.WriteLine($"It took {sw.ElapsedMilliseconds} ms. to get all of the tasks running WITH LongRunning.");
                    return;
                }
                Console.WriteLine($"Running total threads: {Process.GetCurrentProcess().Threads.Count}");
                Console.WriteLine($"Running thread pool threads: {GetRunningThreadPoolThreads()}");
            }
        }

        /// <summary>
        /// Gets how many thread pool threads are currently running.
        /// </summary>
        /// <returns></returns>
        public static int GetRunningThreadPoolThreads()
        {
            ThreadPool.GetMaxThreads(out int max, out int _);
            ThreadPool.GetAvailableThreads(out int available, out _);
            int running = max - available;
            return running;
        }
    }
}
