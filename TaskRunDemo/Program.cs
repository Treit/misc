using System.Diagnostics;

await TestA();
Console.WriteLine();

await TestB();
Console.WriteLine();

await TestC();
Console.WriteLine();

await TestD();
Console.WriteLine();

await TestE();
Console.WriteLine();

static async Task TestA()
{
    Console.WriteLine("TestA starting.");
    var sw = Stopwatch.StartNew();
    var resetEvent = new ManualResetEvent(false);

    var tasks = new List<Task<int>>();
    for (int i = 0; i < 20; i++)
    {
        var lambda = async () =>
        {
            // Simulates doing some expensive synchronous work, such as an
            // expensive CPU-bound computation.
            Thread.Sleep(500);

            // Now simulate doing some async work, such as making a network call.
            await Task.Delay(500);

            resetEvent.WaitOne();

            return Random.Shared.Next();
        };

        tasks.Add(lambda());
    }

    Console.WriteLine($"TestA doing some other work now after {sw.ElapsedMilliseconds} ms.");
    await Task.Delay(100);
    resetEvent.Set();
    await Task.WhenAll(tasks);
    Console.WriteLine($"TestA done waiting for all tasks to finish after {sw.ElapsedMilliseconds} total ms.");
}

static async Task TestB()
{
    Console.WriteLine("TestB starting.");
    var sw = Stopwatch.StartNew();
    var resetEvent = new ManualResetEvent(false);

    var tasks = new List<Task<int>>();
    for (int i = 0; i < 20; i++)
    {
        var task = Task.Run(async () =>
        {
            // Simulates doing some expensive synchronous work, such as an
            // expensive CPU-bound computation.
            Thread.Sleep(500);

            // Now simulate doing some async work, such as making a network call.
            await Task.Delay(500);

            resetEvent.WaitOne();

            return Random.Shared.Next();
        });

        tasks.Add(task);
    }

    Console.WriteLine($"TestB doing some other work now after {sw.ElapsedMilliseconds} ms.");
    await Task.Delay(100);
    resetEvent.Set();
    await Task.WhenAll(tasks);
    Console.WriteLine($"TestB done waiting for all tasks to finish after {sw.ElapsedMilliseconds} total ms.");
}

static async Task TestC()
{
    Console.WriteLine("TestC starting.");
    var sw = Stopwatch.StartNew();
    var resetEvent = new ManualResetEvent(false);

    var tasks = new List<Task<int>>();
    for (int i = 0; i < 20; i++)
    {
        var lambda = async () =>
        {
            await Task.Yield();

            // Simulates doing some expensive synchronous work, such as an
            // expensive CPU-bound computation.
            Thread.Sleep(500);

            // Now simulate doing some async work, such as making a network call.
            await Task.Delay(500);

            resetEvent.WaitOne();

            return Random.Shared.Next();
        };

        tasks.Add(lambda());
    }

    Console.WriteLine($"TestC doing some other work now after {sw.ElapsedMilliseconds} ms.");
    await Task.Delay(100);
    resetEvent.Set();
    await Task.WhenAll(tasks);
    Console.WriteLine($"TestC done waiting for all tasks to finish after {sw.ElapsedMilliseconds} total ms.");
}

static async Task TestD()
{
    Console.WriteLine("TestD starting.");
    var sw = Stopwatch.StartNew();
    var resetEvent = new ManualResetEvent(false);

    var tasks = new List<Task<int>>();
    for (int i = 0; i < 20; i++)
    {
        var lambda = async () =>
        {
            await Task.Yield();

            // Now simulate doing some async work, such as making a network call.
            await Task.Delay(500);

            // Simulates doing some expensive synchronous work, such as an
            // expensive CPU-bound computation.
            Thread.Sleep(500);

            resetEvent.WaitOne();

            return Random.Shared.Next();
        };

        tasks.Add(lambda());
    }

    Console.WriteLine($"TestD doing some other work now after {sw.ElapsedMilliseconds} ms.");
    await Task.Delay(100);
    resetEvent.Set();
    await Task.WhenAll(tasks);
    Console.WriteLine($"TestD done waiting for all tasks to finish after {sw.ElapsedMilliseconds} total ms.");
}

static async Task TestE()
{
    ThreadPool.SetMinThreads(100, 100);

    Console.WriteLine("TestE starting.");
    var sw = Stopwatch.StartNew();
    var resetEvent = new ManualResetEvent(false);

    var tasks = new List<Task<int>>();
    for (int i = 0; i < 20; i++)
    {
        var lambda = async () =>
        {
            await Task.Yield();

            // Now simulate doing some async work, such as making a network call.
            await Task.Delay(500);

            // Simulates doing some expensive synchronous work, such as an
            // expensive CPU-bound computation.
            Thread.Sleep(500);

            resetEvent.WaitOne();

            return Random.Shared.Next();
        };

        tasks.Add(lambda());
    }

    Console.WriteLine($"TestE doing some other work now after {sw.ElapsedMilliseconds} ms.");
    await Task.Delay(100);
    resetEvent.Set();
    await Task.WhenAll(tasks);
    Console.WriteLine($"TestE done waiting for all tasks to finish after {sw.ElapsedMilliseconds} total ms.");
}