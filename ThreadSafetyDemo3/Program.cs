using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Test
{
    class ThreadSafetyDemo
    {
        static int _totalProcessed = 0;        
        static void Main()
        {
            int numTasks = 1000;
            var tasks = new Task[numTasks];

            for (int i = 0; i < numTasks; i++)
            {
                var task = Task.Run(() =>
                {
                    Interlocked.Increment(ref _totalProcessed);
                });

                tasks[i] = task;
            }

            Task.WaitAll(tasks);

            Console.WriteLine($"totalProcessed: {_totalProcessed}.");
        }
    }
}
