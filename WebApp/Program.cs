var builder = WebApplication.CreateBuilder(args);
builder.WebHost.UseUrls("http://localhost:9001");
var app = builder.Build();

app.MapGet("/", async () => {
    await Task.Delay(1000);
    return "a";
});

app.Run();
