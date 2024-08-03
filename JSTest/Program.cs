using System.Diagnostics;
using System.Text;

var client = new HttpClient();

var sw = Stopwatch.StartNew();
var syncobj = new object();

ThreadPool.SetMinThreads(8, 8);

var tasks = new List<Task>();
int total = 0;

for (int i = 0; i < 1000; i++)
{
    var task = Task.Run(async () =>
    {
        var resp = await client.GetAsync(@"http://localhost:9001");
        var text = await resp.Content.ReadAsStringAsync();
        Interlocked.Add(ref total, text.Length);
    });

    tasks.Add(task);
}

await Task.WhenAll(tasks);

Console.WriteLine(total);
Console.WriteLine(sw.ElapsedMilliseconds);