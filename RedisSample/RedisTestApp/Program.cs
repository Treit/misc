using StackExchange.Redis;
using System.Diagnostics;

var redisConnString = Environment.GetEnvironmentVariable("REDIS_CONN_STRING");

if (string.IsNullOrWhiteSpace(redisConnString))
{
    Console.WriteLine("The REDIS_CONN_STRING environment variable is not set.");
    return;
}

var redis = ConnectionMultiplexer.Connect(redisConnString);
redis.ErrorMessage += OnRedisErrorMessage;

void OnRedisErrorMessage(object? sender, RedisErrorEventArgs e)
{
    Console.WriteLine($"OnRedisErrorMessage: {e.Message}");
}

if (!redis.IsConnected)
{
    Console.WriteLine($"Failed to connect to redis cache.");
    return;
}

Console.WriteLine($"Connected to redis cache.");

var db = redis.GetDatabase();
var batch = db.CreateBatch();
var strings = new List<KeyValuePair<string, string>>();

int batchCount = 0;
int batchSize = 100_000;
var tasks = new List<Task>();
var expiration = TimeSpan.FromMinutes(5);
var sw = Stopwatch.StartNew();

for (int i = 0; i < 100_000; i++)
{
    var task = batch.StringSetAsync(i.ToString(), $"{i} value", expiration);
    tasks.Add(task);

    if (batchCount++ == batchSize)
    {
        batch.Execute();
        await Task.WhenAll(tasks);
        tasks.Clear();
        batchCount = 0;
        batch = db.CreateBatch();
    }
}

if (tasks.Count > 0)
{
    batch.Execute();
    await Task.WhenAll(tasks);
}

Console.WriteLine($"All batches finished after {sw.ElapsedMilliseconds} ms.");

var val = await db.StringGetAsync("595");
Console.WriteLine(val);