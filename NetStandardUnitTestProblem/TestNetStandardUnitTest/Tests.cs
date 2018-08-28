namespace TestNetStandardUnitTest
{
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using System.Configuration;

    [TestClass]
    public class Tests
    {
        [TestMethod]
        public void SimpleTest()
        {
            NetStandardTest.Test.DoSomething();
        }
    }
}
