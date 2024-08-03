namespace FileMangler
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Text;
    using System.Text.RegularExpressions;

    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length < 2)
            {
                Console.WriteLine("Provide a path and a glob.");
                return;
            }

            string path = args[0];
            string glob = args[1];

            var files = Directory.EnumerateFileSystemEntries(path, glob, SearchOption.TopDirectoryOnly);

            foreach (var file in files)
            {
                Mangle(file);
            }
        }

        private static void Mangle(string path)
        {
            var fi = new FileInfo(path);
            var idx = fi.Name.IndexOf(".md");
            var rule = fi.Name.Substring(0, idx).ToUpperInvariant();
            string originalPath = path;
            using StreamReader sr = new StreamReader(originalPath);
            var found = false;
            while (sr.ReadLine() is string line)
            {
                if (line.StartsWith("#"))
                {
                    found = true;
                    continue;
                }

                if (!found || string.IsNullOrWhiteSpace(line))
                {
                    continue;
                }

                var str = $"""
                    "{rule}"="{line}"
                """;
                Console.WriteLine(str);
                break;
            }
        }
    }
}
