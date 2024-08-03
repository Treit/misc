var task = Task.Run(() =>
{
    throw new InvalidOperationException("OH NO!");
});

try
{
    Task.WaitAll(task);
}
catch (Exception e)
{
    Console.WriteLine($"Hit an exception: {e.Message}.");
}
finally
{
    Console.WriteLine("All done.");
}

Console.ReadKey();