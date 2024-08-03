if (args.Length == 0)
{
    Console.WriteLine($"Provide an input string.");
    return;
}

var str = args[0];

if (str.Split('-').ToList().Count == 2)
{
    Console.WriteLine($"Input matched.");
}
else
{
    Console.WriteLine($"Input did not match.");
}

if (str.Split('-').ToArray().Length == 2)
{
    Console.WriteLine($"Input matched.");
}
else
{
    Console.WriteLine($"Input did not match.");
}