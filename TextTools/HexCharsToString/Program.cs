using System.Text;

if (args.Length == 0)
{
    Console.WriteLine("Usage:");
    Console.WriteLine("Program.exe <inputString> [encoding]");
    return 1;
}

try
{
    var input = args[0];
    var encodingStr = args.Length > 1 ? args[1] : "UTF8";

    #pragma warning disable SYSLIB0001 // Type or member is obsolete
    var encoding = encodingStr.ToUpperInvariant() switch 
    {
        "ASCII" or "ANSI" => Encoding.ASCII,
        "UNICODE" or "UTF16" or "UTF16-LE" => Encoding.Unicode,
        "UTF8" => Encoding.UTF8,
        "UTF7" => Encoding.UTF7,
        "UTF32" => Encoding.UTF32,
        "LATIN1" or "ISO8859-1" => Encoding.Latin1,
        "BIGENDIANUNICODE" or "UTF16-BE" => Encoding.BigEndianUnicode,
        _ => throw new InvalidOperationException($"Unknown encoding '{encodingStr}'")
    };
    #pragma warning restore SYSLIB0001 // Type or member is obsolete

    var tmp = input.Replace("0x", string.Empty);
    tmp = input.Replace(" ", string.Empty);
    Console.WriteLine($"Encoding given bytes using {encoding.EncodingName}");
    var bytes = Convert.FromHexString(tmp);
    var outputStr = encoding.GetString(bytes);
    var bytesAfterEncoding = encoding.GetBytes(outputStr);
    Console.WriteLine("-- Input bytes --");
    PrintBytes(bytes);
    Console.WriteLine("-- Ouput bytes --");
    PrintBytes(bytesAfterEncoding);
    Console.WriteLine("-- Ouput string --");
    Console.WriteLine(outputStr);

    return 0;
}
catch (Exception e)
{
    Console.WriteLine(e);
    return 2;
}

void PrintBytes(byte[] bytes)
{
    var outputStr = string.Join(" ", bytes.Select(x => x.ToString("X")));
    Console.WriteLine(outputStr);
}