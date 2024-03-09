using System.Text.RegularExpressions;

var pattern = args is ["--dirs", ..] ? @"dirs\.proj$" : @".+\.csproj$|dirs\.proj$";
var targetFile = "dirs.proj";

if (File.Exists(targetFile))
{
    File.Delete(targetFile);
}

var files =
    Directory.EnumerateFiles(".", "*.*", new EnumerationOptions { RecurseSubdirectories = true })
    .Where(f => Regex.IsMatch(f, pattern));

using var sw = new StreamWriter(targetFile);
sw.WriteLine("""<Project Sdk="Microsoft.Build.Traversal">""");
sw.WriteLine("  <ItemGroup>");

foreach (var file in files)
{
    var path = Path.GetRelativePath(".", file);
    if (path != "dirs.proj" && (pattern.Contains("csproj") || path.Count(c => c == '\\') is 1))
    {
        sw.WriteLine($"""    <ProjectFile Include="{path}" />""");
    }
}

sw.WriteLine("  </ItemGroup>");
sw.WriteLine("</Project>");
