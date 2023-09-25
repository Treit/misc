using System;
using System.Linq;

var iters = 0UL;

if (args.Length == 0)
{
    Console.WriteLine("Give us a comma-separated list of values to Bogosort.");
    return;
}

var input = args[0];
var vals = input.Split(',').Select(x => int.Parse(x)).ToArray();

Console.WriteLine("YOLO - sorting!");

while (!IsSorted(vals))
{
    iters++;
    FisherYates(vals);
}

Console.WriteLine($"HELL YEAH! Sorted {vals.Length} values in {iters} iterations!");
Console.WriteLine(string.Join(',', vals));

bool IsSorted<T>(T[] input) where T : IComparable<T>
{
    T last = input[0];

    for (int i = 1; i < input.Length; i++)
    {
        if (input[i].CompareTo(last) < 0)
        {
            return false;
        }
        last = input[i];
    }

    return true;
}

void FisherYates<T>(T[] input)
{
    for (int i = input.Length - 1; i > 0; i--)
    {
        var idx = Random.Shared.Next(0, i + 1);
        var tmp = input[idx];
        input[idx] = input[i];
        input[i] = tmp;
    }
}