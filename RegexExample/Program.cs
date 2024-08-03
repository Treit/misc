using System.Text.RegularExpressions;
using System.IO;

internal partial class Program
{
    [GeneratedRegex(@"for key.+-(\d+)(.+)", RegexOptions.None)]
    private static partial Regex GetRegex();
    private static void Main(string[] args)
    {
        if (args.Length == 0)
        {
            Console.WriteLine("Usage: dotnet run <filename>");
            return;
        }

        using var sr = new StreamReader(args[0]);
        var results = new Dictionary<string, HashSet<string>>();

        while (sr.ReadLine() is string line)
        {
            var match = GetRegex().Match(line);
            if (!match.Success)
            {
                continue;
            }

            var hash = match.Groups[1].Value;
            var id = match.Groups[2].Value;

            if (results.TryGetValue(id, out var set))
            {
                set.Add(hash);
            }
            else
            {
                results.Add(id, new HashSet<string> { hash });
            }
        }

        var finalCount = results.Where(kvp => kvp.Value.Count == 3).Count();

        Console.WriteLine(finalCount);
    }

}