using System.Diagnostics;
using TestLib;

namespace Test
{
    class ReflectionTest : IObserver<DiagnosticListener>
    {
        public void DoTest()
        {
            var t = typeof(SimpleClass);
            var method = t.GetMethods().Where(x => x.Name.Contains("DoSomething")).First();
            var obj = Activator.CreateInstance(t);
            method.Invoke(obj, new object?[] { this });
        }

        public void OnCompleted()
        {
            Console.WriteLine("OnCompleted");
        }

        public void OnError(Exception error)
        {
            Console.WriteLine("OnError");
        }

        public void OnNext(DiagnosticListener value)
        {
            Console.WriteLine($"OnNext: {value.Name}");
        }
    }

    class Program
    {
        static void Main()
        {
            var test = new ReflectionTest();
            test.DoTest();
        }
    }
}