var thread = new Thread(() =>
{
    throw new InvalidOperationException("OH NO!");
});

try
{
    thread.Start();
    thread.Join();
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