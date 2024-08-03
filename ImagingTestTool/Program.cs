using Test;

if (args.Length < 4)
{
    Console.WriteLine("Usage:");
    Console.WriteLine("app.exe <uri> <outPath> <width> <height>");
    return;
}

var uri = args[0];
var outPath = args[1];

if (!int.TryParse(args[2], out var width))
{
    Console.WriteLine("Failed to parse width value.");
    return;
}

if (!int.TryParse(args[3], out var height))
{
    Console.WriteLine("Failed to parse height value.");
    return;
}

//var imageProvider = new ImageProvider();
var imageSharpImageProvider = new ImageSharpImageProvider();
//await DoTest(imageProvider, uri, outPath, height, width);
await DoTest(imageSharpImageProvider, uri, $"{outPath}.imagesharp.jpg", height, width);

async Task DoTest(
    IImageProvider imageProvider,
    string uri,
    string outPath,
    int width,
    int height)
{
    var data = await imageProvider.GetImageAsync(new Uri(uri), width, height);
    File.WriteAllBytes(outPath, data.ImageBuffer);
    Console.WriteLine($"Wrote {outPath}.");
}