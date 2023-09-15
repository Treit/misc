namespace UnitTestExitCode;

[TestClass]
public class UnitTest1
{
    [TestMethod]
    public void TestMethod1()
    {
        Assert.IsTrue(1 == 0, "Test failed."); ;
    }
}