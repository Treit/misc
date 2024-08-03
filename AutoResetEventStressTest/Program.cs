namespace TaskDemo
{
    using Microsoft.VisualStudio.Threading;
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.IO.MemoryMappedFiles;
    using System.Runtime.Caching;
    using System.Threading;
    using System.Threading.Tasks;
    using AsyncManualResetEventVS = Microsoft.VisualStudio.Threading.AsyncManualResetEvent;

    class Program
    {
        private static int s_minThreads = 1000;
        private static CancellationTokenSource s_cts = new();
        private static MemoryCache s_memCache;

        private static async Task Main(string[] args)
        {
            var count = 1_000_000;

            ThreadPool.SetMinThreads(s_minThreads, s_minThreads);
            s_memCache = new MemoryCache("TestCache");
            var cachePolicy = new CacheItemPolicy
            {
                SlidingExpiration = TimeSpan.FromMinutes(5),
                Priority = CacheItemPriority.Default
            };

            for (var i = 0; i < count; i++)
            {
                var resetEvent = new AsyncAutoResetEvent();
                resetEvent.Set();
                var cacheItem = new CacheItem(i.ToString(), resetEvent);
                s_memCache.Add(cacheItem, cachePolicy);
            }

            await Task.Yield();
            var tasks = new List<Task>();

            for (var i = 0; i < count; i++)
            {
                var item = i;

                var t = Task.Run(async () =>
                {
                    var cacheItem = s_memCache.GetCacheItem(item.ToString());
                    var resetEvent = (AsyncAutoResetEvent)cacheItem.Value;

                    if (resetEvent is null)
                    {
                        return;
                    }

                    try
                    {
                        await resetEvent.WaitAsync();

                        // Simulate some work
                        await Task.Delay(50);
                    }
                    finally
                    {
                        resetEvent.Set();
                    }
                });

                tasks.Add(t);

                t = Task.Run(async () =>
                {
                    var cacheItem = s_memCache.GetCacheItem(item.ToString());
                    var resetEvent = (AsyncAutoResetEvent)cacheItem.Value;

                    if (resetEvent is null)
                    {
                        return;
                    }

                    try
                    {
                        await resetEvent.WaitAsync();

                        // Simulate some work
                        await Task.Delay(50);
                    }
                    finally
                    {
                        resetEvent.Set();
                    }
                });

                tasks.Add(t);
            }

            await Task.WhenAll(tasks);
        }
    }
}
