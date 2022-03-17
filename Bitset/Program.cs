using Test;
var bset = new BitSet(128);
bset[1] = true;

Console.WriteLine(bset[0]);
Console.WriteLine(bset[1]);
Console.WriteLine(bset[15]);
Console.WriteLine(bset[110]);

Console.WriteLine();

bset[15] = true;
Console.WriteLine(bset[0]);
Console.WriteLine(bset[1]);
Console.WriteLine(bset[15]);
Console.WriteLine(bset[110]);

bset[110] = true;

Console.WriteLine();

Console.WriteLine(bset[0]);
Console.WriteLine(bset[1]);
Console.WriteLine(bset[15]);
Console.WriteLine(bset[110]);

bset[110] = false;

Console.WriteLine();

Console.WriteLine(bset[0]);
Console.WriteLine(bset[1]);
Console.WriteLine(bset[15]);
Console.WriteLine(bset[110]);

Console.WriteLine();
bset = new BitSet(1);
Console.WriteLine(bset[0]);
bset[0] = true;
Console.WriteLine(bset[0]);
bset[0] = false;
Console.WriteLine(bset[0]);

try
{
    bset[1] = true;
}
catch (ArgumentOutOfRangeException)
{
    Console.WriteLine("Got expected exception on write.");
}

try
{
    var _ = bset[1];
}
catch (ArgumentOutOfRangeException)
{
    Console.WriteLine("Got expected exception on write.");
}