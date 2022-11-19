using RedirectCertIssue;
using System;
using System.ServiceModel;

namespace TestClient
{
    class Program
    {
        private readonly static string _baseHttpAddress = @"localhost:802";
        private readonly static string _baseHttpsAddress = @"localhost:8443";

        static void Main(string[] args)
        {
            var basicHttpAddress = $"http://{_baseHttpAddress}/basichttp";
            var basicHttpsAddress = $"https://{_baseHttpsAddress}/basichttp";
            var wsHttpAddress = $"http://{_baseHttpAddress}/wsHttp.svc";
            var wsHttpsAddress = $"https://{_baseHttpsAddress}/wsHttp.svc";

            var httpsBinding = new BasicHttpsBinding();
            var httpsChannel = ChannelFactory<ITestService>.CreateChannel(httpsBinding, new EndpointAddress(basicHttpsAddress));

            var httpBinding = new BasicHttpBinding();
            var httpChannel = ChannelFactory<ITestService>.CreateChannel(httpBinding, new EndpointAddress(basicHttpAddress));

            var wsHttpBinding = new WSHttpBinding(SecurityMode.None);
            wsHttpBinding.Security.Message.ClientCredentialType = MessageCredentialType.None;
            var wsHttpChannel = ChannelFactory<ITestService>.CreateChannel(wsHttpBinding, new EndpointAddress(wsHttpAddress));

            var wsHttpsBinding = new WSHttpBinding(SecurityMode.Transport);
            wsHttpsBinding.Security.Message.ClientCredentialType = MessageCredentialType.None;
            var wsHttpsChannel = ChannelFactory<ITestService>.CreateChannel(wsHttpsBinding, new EndpointAddress(wsHttpsAddress));

            TestMessage msg = new TestMessage();
            msg.Data = "foobar";

            ColorWrite("BasicHttp:", ConsoleColor.Cyan);
            var result = httpChannel.Test(msg);
            Console.WriteLine(result);

            ColorWrite("BasicHttps:", ConsoleColor.Cyan);
            result = httpsChannel.Test(msg);
            Console.WriteLine(result);

            ColorWrite("WsHttp:", ConsoleColor.Cyan);
            result = wsHttpChannel.Test(msg);
            Console.WriteLine(result);

            ColorWrite("WsHttps:", ConsoleColor.Cyan);
            result = wsHttpsChannel.Test(msg);
            Console.WriteLine(result);

            void ColorWrite(string message, ConsoleColor color)
            {
                Console.ForegroundColor = color;
                Console.WriteLine(message);
                Console.ResetColor();
            }
        }
    }
}
