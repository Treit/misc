using Benchmark;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace Test
{
    public record TestResponse(string Message);
    public record TestRequest(string Input);

    class Test { }

    [ApiController]
    [Route("[controller]")]
    public class TestController : ControllerBase
    {
        private readonly ILogger<TestController> _logger;
        private readonly IServiceCollection _serviceCollection;
        private readonly IDbContextFactory<AdventureWorks2019Context> _contextFactory;

        public TestController(
            ILogger<TestController> logger,
            IServiceCollection collection,
            IDbContextFactory<AdventureWorks2019Context> contextFactory)
        {
            _logger = logger;
            _serviceCollection = collection;
            _serviceCollection.AddSingleton<Test>(new Test());
            _contextFactory = contextFactory;
        }

        [HttpPost]
        public async Task<TestResponse> Post(TestRequest request)
        {
            await Test();
            return new TestResponse(request.Input.ToUpperInvariant());
        }

        async Task Test()
        {
            AsyncVoidMethod();

            await using (var databaseContext = await _contextFactory.CreateDbContextAsync())
            {
                var items = databaseContext.SalesOrderDetails.Select(x => x.LineTotal);
                foreach (var item in items.Take(10))
                {
                    Console.WriteLine(item);
                }
            }

            var headerName = "SomeHeader";

            if (HttpContext.Request.Headers.TryGetValue(headerName, out var headerValue))
            {
                PrintIt(headerValue);
            }

            static void PrintIt(string? message)
            {
                Console.WriteLine(message);
            }
        }

        async void AsyncVoidMethod()
        {
            await Task.Yield();
            throw new InvalidOperationException("😨");
        }
    }
}
