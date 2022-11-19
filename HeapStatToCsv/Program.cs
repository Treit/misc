using System;
using System.IO;
using System.Text.RegularExpressions;

namespace HeapStatToCsv
{
    class Program
    {
        static readonly Regex headerRegex = new(@"^\s*MT\s+Count\s+TotalSize\s+Class Name", RegexOptions.Compiled);
        static readonly Regex heapstatRegex = new(@"([0-9a-f]{16})\s+?(\d+?)\s+(\d+?)\s+(\S.+$)", RegexOptions.Compiled);

        static void Main(string[] args)
        {
            if (args.Length < 2)
            {
                PrintUsage();
                return;
            }

            string filepath = args[0];

            if (!long.TryParse(args[1], out long threshold))
            {
                Console.WriteLine("The threshold value was not a valid integer.");
                return;
            }

            ProcessHeapStatFile(filepath, threshold);
        }

        private static void ProcessHeapStatFile(string filepath, long threshold)
        {
            using var sr = new StreamReader(filepath);
            string line;
            bool started = false;

            while ((line = sr.ReadLine()) != null)
            {
                if (!started)
                {
                    if (!headerRegex.IsMatch(line))
                    {
                        continue;
                    }
                    else
                    {
                        Console.WriteLine("MT,Count,Size,Type");
                        started = true;
                        continue;
                    }
                }

                Match m = heapstatRegex.Match(line);

                if (!m.Success)
                {
                    break;
                }

                string mt = m.Result("$1");
                string count = m.Result("$2");
                string sizeStr = m.Result("$3");
                string typeName = m.Result("$4").Replace(",", ".");

                if (long.TryParse(sizeStr, out long size))
                {
                    if (size < threshold)
                    {
                        continue;
                    }
                }

                Console.WriteLine($"{mt},{count},{sizeStr},{typeName}");
            }
        }

        private static void PrintUsage()
        {
            Console.WriteLine("Usage:");
            Console.WriteLine("HeapStatToCsv.exe <file> <threshold>");
        }
    }
}
