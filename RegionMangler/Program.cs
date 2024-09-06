namespace FileMangler
{
    using System;
    using System.IO;
    using System.Text;

    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length < 2)
            {
                Console.WriteLine("Provide a path and a glob.");
                return;
            }

            var path = args[0];
            var glob = args[1];
            var mangled = 0;
            var total = 0;

            var files = Directory.EnumerateFileSystemEntries(path, glob, SearchOption.AllDirectories);

            foreach (var file in files)
            {
                total++;

                var result = Mangle(file);
                if (result)
                {
                    mangled++;
                    Console.WriteLine($"😈 Mangled {file}!");
                }
            }

            Console.WriteLine($"😃 mangled {mangled} out of {total} files.");
        }

        private static bool Mangle(string path)
        {
            var temp = path + ".05750be8-4b63-411a-932b-eaf4035b1da9";
            var found = false;

            using (var sr = new StreamReader(path))
            using (var fs = new FileStream(temp, FileMode.Create))
            using (var sw = new StreamWriter(fs, Encoding.UTF8))
            {
                while (sr.ReadLine() is string line)
                {
                    var trimmed = line.Trim();

                    if (trimmed.StartsWith("#region") || trimmed.StartsWith("#endregion"))
                    {
                        found = true;
                        continue;
                    }

                    sw.WriteLine(line);
                }
            }

            if (found)
            {
                File.Copy(temp, path, true);
            }

            File.Delete(temp);

            return found;
        }
    }
}
