using System.Diagnostics;

var sw = Stopwatch.StartNew();

for (int i = 0; i < 1000; i++)
{
    Console.WriteLine($"Iteration {i}");
}

Console.WriteLine($"Time elapsed: {sw.ElapsedMilliseconds} ms");
