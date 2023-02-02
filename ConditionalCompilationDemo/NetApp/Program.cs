using SharedLib;

var tc = new TestClass();
Console.WriteLine(tc.TestProp);
#if NET6_0_OR_GREATER
Console.WriteLine(tc.NetProp);
#endif