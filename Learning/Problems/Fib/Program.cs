foreach (var fib in Fib(20))
{
    Console.Write(fib);
    Console.Write(" ");
}

Console.WriteLine();
Console.WriteLine(FibRec(19));

foreach (var fib in FibWithStack(20))
{
    Console.Write(fib);
    Console.Write(" ");
}

IEnumerable<int> FibWithStack(int max)
{
    yield return 0;
    yield return 1;
    var stack = new Stack<int>();
    stack.Push(0);
    stack.Push(1);

    var iterations = 1;

    while (true)
    {
        if (++iterations >= max)
        {
            break;
        }

        var first = stack.Pop();
        var second = stack.Pop();
        var sum = first + second;
        stack.Push(first);
        stack.Push(sum);
        yield return sum;
    }
}

IEnumerable<int> Fib(int max)
{
    yield return 0;
    yield return 1;

    int first = 0;
    int second = 1;

    for (int i = 2; i < max; i++)
    {
        int sum = first + second;
        first = second;
        second = sum;
        yield return sum;
    }
}

int FibRec(int n)
{
    if (n == 0 || n == 1)
    {
        return n;
    }

    return FibRec(n - 1) + FibRec(n - 2);
}