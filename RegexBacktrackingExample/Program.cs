using System.Text.RegularExpressions;
using System.IO;
using System.Diagnostics;

internal class Program
{
    private static string s_regexString = @"(x+x+)+y";

    private static void Main(string[] args)
    {
        var sw = Stopwatch.StartNew();
        var someString = "xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx";
        var regex = new Regex(s_regexString, RegexOptions.NonBacktracking);

        try
        {
            var match = regex.Match(someString);

            if (match.Success)
            {
                Console.WriteLine($"Got a match 😎");
            }
            else
            {
                Console.WriteLine("No match found 😔");
            }

            Console.WriteLine($"Time taken: {sw.ElapsedMilliseconds} ms");
        }
        catch (RegexMatchTimeoutException)
        {
            Console.WriteLine("Regex took too long to match! 😲");
        }
    }
}