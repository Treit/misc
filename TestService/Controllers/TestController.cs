using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Test
{
    public record TestResponse(string Message);
    public record TestRequest(string Input);

    [ApiController]
    [Route("[controller]")]
    public class TestController : ControllerBase
    {
        private readonly ILogger<TestController> _logger;

        public TestController(
            ILogger<TestController> logger)
        {
            _logger = logger;
        }

        [HttpPost]
        public TestResponse Post(TestRequest request)
        {
            return new TestResponse(request.Input.ToUpperInvariant());
        }
    }
}
