using System.Text;

if (args.Length == 0 || !int.TryParse(args[0], out var length))
{
    Console.WriteLine("Usage:");
    Console.WriteLine("Program.exe <length>");
    return;
}

var str = GetRandomString(Random.Shared, length);
Console.WriteLine(str);

static string GetRandomString(Random r, int length)
{
    var alphabet = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz-_";
    var sb = new StringBuilder(length);

    for (int i = 0; i < length; i++)
    {
        sb.Append(alphabet[r.Next(alphabet.Length)]);
    }

    return sb.ToString();
}