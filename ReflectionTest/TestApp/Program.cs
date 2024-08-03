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
            var stuff = new string[] { "a", "b", "c", "d", "e" };
            var x = stuff switch
            {
                [_, "b", .., var result] => result.ToUpper(),
                _ => ""
            };

            Console.WriteLine(x);
        }

        private static IEnumerable<string[]> ReadRecords()
        {
            yield return new string[] { "a", "DEPOSIT", "b", "100" };
        }
    }
}