using CoreWCF;
using System.Runtime.Serialization;

namespace RedirectCertIssue
{
    [ServiceContract]
    public interface ITestService
    {
        [OperationContract]
        string Test(TestMessage text);
    }

    [DataContract]
    public class TestMessage
    {
        [DataMember]
        public string Data { get; set; }
    }
}
