using System;
using System.Globalization;

static T Add<T>(T left, T right)
    where T : INumber<T>
{
    return left + right;
}

static T ParseInvariant<T>(string s)
    where T : IParseable<T>
{
    return T.Parse(s, CultureInfo.InvariantCulture);
}

Console.Write("First number: ");
var left = ParseInvariant<float>(Console.ReadLine());

Console.Write("Second number: ");
var right = ParseInvariant<float>(Console.ReadLine());

Console.WriteLine($"Result: {Add(left, right)}");
