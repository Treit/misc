using System.IO;
using System.Text;

if (args.Length < 2)
{
    Console.WriteLine("Usage:");
    Console.WriteLine("Program.exe <file> <encoding> [includeBOM]");
    return 1;
}

try
{
    var inputFile = args[0];

    if (!File.Exists(inputFile))
    {
        Console.WriteLine($"Could not find file '{inputFile}'.");
        return 2;
    }

    var encodingStr = args.Length > 1 ? args[1] : "UTF8";
    var includeBOM = false;
    if (args.Length > 2)
    {
        if (args[2].ToUpperInvariant() is "TRUE" or "YES")
        {
            includeBOM = true;
        }
    }

#pragma warning disable SYSLIB0001 // Type or member is obsolete
    var encoding = encodingStr.ToUpperInvariant() switch
    {
        "ASCII" or "ANSI" => Encoding.ASCII,
        "UNICODE" or "UTF16" or "UTF16-LE" => new UnicodeEncoding(false, includeBOM, true),
        "UTF8" => new UTF8Encoding(includeBOM, true),
        "UTF7" => Encoding.UTF7,
        "UTF32" => new UTF32Encoding(false, includeBOM, true),
        "LATIN1" or "ISO8859-1" => Encoding.Latin1,
        "BIGENDIANUNICODE" or "UTF16-BE" => new UnicodeEncoding(true, includeBOM, true),
        _ => throw new InvalidOperationException($"Unknown encoding '{encodingStr}'")
    };
#pragma warning restore SYSLIB0001 // Type or member is obsolete

    var tmpFile = Path.GetTempFileName();
    using (var sr = new StreamReader(inputFile, Encoding.UTF8, true))
    using (var sw = new StreamWriter(tmpFile, false, encoding))
    {
        while (sr.ReadLine() is string line)
        {
            sw.WriteLine(line);
        }
    }

    File.Copy(inputFile, $"{inputFile}.bak", true);
    File.Copy(tmpFile, inputFile, true);
    File.Delete(tmpFile);

    return 0;
}
catch (Exception e)
{
    Console.WriteLine(e);
    return 99;
}