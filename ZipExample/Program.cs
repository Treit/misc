using System.IO.Compression;

if (args.Length == 0)
{
    Console.WriteLine("Provide a zip file.");
    return;
}

string filename = args[0];

using var archive = ZipFile.Open(filename, ZipArchiveMode.Read);
var currentArchive = archive;

while (true)
{
    var last = currentArchive.Entries.Last();
    if (!last.Name.EndsWith(".zip"))
    {
        break;
    }

    currentArchive = new ZipArchive(currentArchive.Entries.Last().Open());
}

var finalFile = currentArchive.Entries.Last();
Console.WriteLine($"Terminated because we found {finalFile.Name}, which is not a zip file.");

using var fs = finalFile.Open();
using var destfs = new FileStream(finalFile.Name, FileMode.Create);
fs.CopyTo(destfs);

Console.WriteLine($"Wrote {finalFile.Name} to disk.");

