/*
This program is used to add or update existing PackageReference items to use centralized package versioning.
Centralized package versioning allows a single Packages.props file at the root of the project tree to specify
which version of a package to use. This allows for easy updating of packages versions across all projects, and
also helps reduce version conflicts.

The program does the following:
1. Takes a NuGet package name and version as input.
2. Adds or updates an entry in Packages.props in the current folder for that package and version.
3. Finds all csproj files in the current directory and subdirectories and if there is an existing PackageReference
entry for that package, and if VersionOverride is not specified, updates the entry for that package.
*/
using AddCentralizedPackageReference;
using System.Text.RegularExpressions;

if (args.Length < 1)
{
    PrintUsage();
    return;
}

if (!File.Exists("Packages.props"))
{
    Console.WriteLine("Packages.props file not found in current directory.");
    return;
}

var packageName = args[0];
var packageVersion = args.Length > 1 ? args[1] : (await NuGetPackageQuery.GetLatestVersion(packageName))?.Version;

if (packageVersion is null)
{
    Console.WriteLine($"Could not determine the latest package version for the package '{packageName}'.");
    return;
}

Console.WriteLine($"Using package '{packageName}' with version '{packageVersion}.");

try
{
    UpdatePackagesPropsFile("Packages.props", packageName, packageVersion);
}
catch (Exception e)
{
    Console.WriteLine($"Failed updating the props file with:{Environment.NewLine}{e}");
}

try
{
    var csprojFiles = Directory.EnumerateFiles(Directory.GetCurrentDirectory(), "*.csproj", SearchOption.AllDirectories);
    Console.WriteLine($"{csprojFiles.Count()} csproj files found.");
    foreach (var csprojFile in csprojFiles)
    {
        try
        {
            UpdateCsprojFile(csprojFile, packageName);
            Console.WriteLine($"Updated {csprojFile} successfully.");
        }
        catch (Exception e)
        {
            Console.WriteLine($"Failed to update {csprojFile} with error: {e.Message}");
        }
    }
}
catch (Exception e)
{
    Console.WriteLine($"Failed updating the csproj file with:{Environment.NewLine}{e}");
}

static void UpdatePackagesPropsFile(string propsFile, string packageName, string version)
{
    var tmpFile = $"{propsFile}.tmp";

    using (var reader = new StreamReader(propsFile))
    using (var writer = new StreamWriter(tmpFile))
    {
        var state = State.NotStarted;

        while (reader.ReadLine() is string line)
        {
            if (state is not State.Started)
            {
                writer.WriteLine(line);
            }

            if (line.Contains("<ItemGroup>"))
            {
                state = State.Started;
            }

            if (line.Contains("<PackageReference"))
            {
                if (line.Contains($"<PackageReference Update=\"{packageName}\""))
                {
                    continue;
                }

                writer.WriteLine(line);
            }

            if (state is State.Started && line.Contains("</ItemGroup>"))
            {
                state = State.Finished;
                writer.WriteLine($@"    <PackageReference Update=""{packageName}"" Version=""{version}"" />");
                writer.WriteLine(line);
            }
        }
    }

    File.Copy(tmpFile, propsFile, overwrite: true);
    File.Delete(tmpFile);
}

static void UpdateCsprojFile(string csprojFile, string packageName)
{
    var tmpFile = $"{csprojFile}.tmp";

    var lines = File.ReadAllLines(csprojFile);

    var re = new Regex($@"PackageReference.+Include=""{packageName}"" (Version="".+"")");
    var fileMatched = false;

    using (var sw = new StreamWriter(tmpFile))
    {
        foreach (var line in lines)
        {
            var lineToWrite = line;
            var m = re.Match(line);

            if (m.Success)
            {
                fileMatched = true;
                lineToWrite = line.Replace(m.Result("$1"), string.Empty);
                lineToWrite = lineToWrite.Replace("  />", " />");
            }

            sw.WriteLine(lineToWrite);
        }
    }

    if (fileMatched)
    {
        File.Copy(tmpFile, csprojFile, overwrite: true);
    }

    File.Delete(tmpFile);
}

static void PrintUsage()
{
    var programName = AppDomain.CurrentDomain.FriendlyName;
    Console.WriteLine("Usage:");
    Console.WriteLine($"{programName} <PackageName> [<PackageVersion>]");
    Console.WriteLine();
    Console.WriteLine("Example:");
    Console.WriteLine($"{programName} 'System.Text.Json' '7.0.3'");
}

enum State
{
    NotStarted,
    Started,
    Finished
}