using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Test
{
    class ThreadSafetyDemo
    {
        static async Task Main(string[] args)
        {
            if (args.Length < 4)
            {
                PrintUsage();
                return;
            }

            var startFolder = args[0];
            var wildCard = args[1];
            var fileNum = int.Parse(args[2]);
            var lineNum = int.Parse(args[3]);
            var sw = Stopwatch.StartNew();

            var task = GetLineInFile(startFolder, wildCard, fileNum, lineNum);
            Console.WriteLine($"Got task back after {sw.ElapsedMilliseconds} ms.");

            // Other work goes here.

            // Now we are ready to get the result of the async task.
            var nthLine = await task;
            Console.WriteLine($"Task completed after {sw.ElapsedMilliseconds} ms. total.");
            Console.WriteLine($"Line {lineNum} of file number {fileNum} is:");
            Console.WriteLine($"{nthLine}");
        }

        private static async Task<string> GetLineInFile(
            string startFolder,
            string wildcard,
            int fileNumber,
            int lineNumber)
        {
            var files = Directory.EnumerateFiles(
                startFolder,
                wildcard,
                new EnumerationOptions { RecurseSubdirectories = true });

            var nthfile = files.Skip(fileNumber - 1).Take(1).FirstOrDefault();

            using var sr = new StreamReader(nthfile);

            var line = string.Empty;

            for (int i = 0; i < lineNumber; i++)
            {
                line = await sr.ReadLineAsync();

                if (line == null)
                {
                    break;
                }
            }

            return line;
        }

        private static void PrintUsage()
        {
            Console.WriteLine("Program.exe <startFolder> <wildCard> <fileNumber> <lineNumber>");
        }
    }
}
