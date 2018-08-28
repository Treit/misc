namespace TestNetStandardUnitTest
{
    using Microsoft.VisualStudio.TestTools.UnitTesting;

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
