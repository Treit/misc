int count = 4;
var numbers = new int[count];
var tasks = new Task[count];

for (int i = 0; i < count; i++)
{
    var task = Task.Run(() =>
    {
        Console.WriteLine($"Processing {i}.");

        // Code to write to numbers[i] goes here.
    });

    tasks[i] = task;
}

Task.WaitAll(tasks);