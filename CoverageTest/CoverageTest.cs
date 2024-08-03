namespace CoverageTest;

[TestClass]
public class CoverageTest
{
    [TestMethod]
    public void BasicCoverageTest()
    {
        var test = new Test();
        test.DoIt();
        test.DoIt();
    }
}

class Test
{
    object _lock = new object();
    string? _instance = null;

    public string DoIt()
    {
        if (_instance is not null) return _instance;
        lock (_lock)
        {
            if (_instance is null)
            {
                _instance = CreateInstance();
            }
            else { }
            return _instance;
        }
    }
    string CreateInstance()
    {
        return "foo";
    }

}