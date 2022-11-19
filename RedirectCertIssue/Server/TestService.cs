namespace RedirectCertIssue
{
    public class TestService : ITestService
    {
        public string Test(TestMessage message)
        {
            return message.Data.ToUpperInvariant();
        }
    }
}