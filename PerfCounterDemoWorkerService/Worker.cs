using System.Diagnostics;

namespace PerfCounterDemoWorkerService;

public class Worker : BackgroundService
{
    private readonly ILogger<Worker> _logger;

    public Worker(ILogger<Worker> logger)
    {
        _logger = logger;
    }

    public override async Task StartAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Worker started at: {time}", DateTimeOffset.Now);
        _ = Task.Run(MonitorCPU);
        await base.StartAsync(stoppingToken);
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            await Task.Delay(1000, stoppingToken);
        }
    }

    void MonitorCPU()
    {
        var cpuCounter = new PerformanceCounter("Processor", "% Processor Time", "_Total");
        while (true)
        {
            Console.WriteLine("CPU Usage: {0}", cpuCounter.NextValue());
            Thread.Sleep(1000);
        }
    }
}
