using System.Diagnostics;

namespace TestLib;

public class SimpleClass
{
    public void DoSomething(IObserver<DiagnosticListener> observer)
    {
        observer.OnNext(new DiagnosticListener("something"));
    }
}
