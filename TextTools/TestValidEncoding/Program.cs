using System.IO;
using System.Text;

if (args.Length < 2)
{
    Console.WriteLine("Usage:");
    Console.WriteLine("Program.exe <file> <encoding>");
    return 1;
}

var inputFile = args[0];

try
{
    if (!File.Exists(inputFile))
    {
        Console.WriteLine($"Could not find file '{inputFile}'.");
        return 2;
    }

    var encodingStr = args.Length > 1 ? args[1] : "UTF8";


#pragma warning disable SYSLIB0001 // Type or member is obsolete
    var encoding = encodingStr.ToUpperInvariant() switch
    {
        "ASCII" or "ANSI" => Encoding.ASCII,
        "UNICODE" or "UTF16" or "UTF16-LE" => new UnicodeEncoding(false, false, true),
        "UTF8" => new UTF8Encoding(false, true),
        "UTF7" => Encoding.UTF7,
        "UTF32" => new UTF32Encoding(false, false, true),
        "LATIN1" or "ISO8859-1" => Encoding.Latin1,
        "BIGENDIANUNICODE" or "UTF16-BE" => new UnicodeEncoding(true, false, true),
        _ => throw new InvalidOperationException($"Unknown encoding '{encodingStr}'")
    };
#pragma warning restore SYSLIB0001 // Type or member is obsolete

    using (var sr = new StreamReader(inputFile, encoding, true))
    {
        while (sr.ReadLine() is string line)
        {
        }
    }

    Console.WriteLine($"<valid>,{new FileInfo(inputFile).FullName}");

    return 0;
}
catch (Exception e)
{
    Console.WriteLine($"<invalid>,{new FileInfo(inputFile).FullName}");
    return 99;
}