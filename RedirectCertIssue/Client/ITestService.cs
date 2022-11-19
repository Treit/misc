using System.Runtime.Serialization;
using System.ServiceModel;

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
