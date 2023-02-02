namespace SharedLib;
public class TestClass
{
    public string TestProp { get; } = "Normal prop";
#if NET5_0_OR_GREATER
    public string NetProp { get; } = ".NET-only prop";
#endif
}
