using System.Management.Automation;
var procs = PowerShell.Create().AddScript("get-process").Invoke();
foreach (var proc in procs)
{
    Console.WriteLine(proc);
}

