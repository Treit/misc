using Microsoft.Extensions.Configuration;

var builder =
    new ConfigurationBuilder()
    .SetBasePath(AppContext.BaseDirectory)
    .AddJsonFile("appsettings.json")
    .AddEnvironmentVariables()
    .AddCommandLine(args);

var config = builder.Build();
var settings = config.GetRequiredSection("Logging").Get<Logging>();

if (settings == null)
{
    Console.WriteLine("Could not find required settings.");
    return;
}

Console.WriteLine(settings.LogLevel.Default);

public sealed class Logging
{
    public LogLevelSetting LogLevel { get; init; } = null!;
}

public sealed class LogLevelSetting
{
    public required string Default { get; set; }
    public required string Microsoft { get; set; }
}