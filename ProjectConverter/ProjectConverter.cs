namespace ProjectConverter
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.IO;
    using System.Text;
    using System.Text.RegularExpressions;
    using System.Xml;

    internal class ProjectConverter
    {
        private readonly string filePath;
        private static readonly Regex packageRegex = new Regex(@"Include=""(.+?)""");
        private static readonly Regex numberPartRegex = new Regex(@"(^.+)\.(\d*\.\d*\.\d*\.?\d*)(\\.+)$");
        private static readonly Regex xmlnsRegex = new Regex(@"\sxmlns="".+"">");
        private static readonly StringComparison ignoreCase = StringComparison.OrdinalIgnoreCase;

        public ProjectConverter(string filePath)
        {
            if (string.IsNullOrWhiteSpace(filePath))
            {
                throw new ArgumentException("Invalid file name", nameof(filePath));
            }

            if (!File.Exists(filePath))
            {
                throw new ArgumentException($"The file {filePath} does not exist", nameof(filePath));
            }

            this.filePath = filePath;
        }

        public void Convert()
        {
            Stack<string> restoreIfNeeded = new Stack<string>();
            StringBuilder sb = new StringBuilder();

            Console.WriteLine($"Converting project '{this.filePath}'.");

            // Pre-process
            using (XmlReader reader = XmlReader.Create(this.filePath))
            {
                sb.Append(@"<?xml version=""1.0"" encoding=""utf-8""?>");
                while (reader.Read())
                {
                    if (reader.Name == "Project")
                    {
                        if (reader.IsStartElement())
                        {
                            sb.Append("<Project");
                            bool ok = reader.MoveToFirstAttribute();
                            while (ok)
                            {
                                sb.Append(" ");
                                sb.Append(reader.Name + @"=""" + reader.Value + @"""");
                                ok = reader.MoveToNextAttribute();
                            }

                            sb.Append($">{Environment.NewLine}");
                        }
                        else
                        {
                            sb.AppendLine("</Project>");
                        }
                    }

                    if (reader.NodeType == XmlNodeType.Element && reader.Name != "Project")
                    {
                        string xml = reader.ReadOuterXml();

                        if (xml.Contains("<ItemGroup") && xml.Contains("FilesToCopy", ignoreCase) && !xml.Contains("<Target Name"))
                        {
                            restoreIfNeeded.Push(xml);
                        }

                        if (xml.Contains("<PropertyGroup") && xml.Contains("PostBuildEvent", ignoreCase))
                        {
                            Console.ForegroundColor = ConsoleColor.Red;
                            Console.Error.WriteLine("Project contains a PostBuildEvent, which does not work properly with the new SDK project format.");
                            Console.Error.WriteLine("Re-write the PostBuildEvent logic using Targets and then re-try conversion.");
                            Console.ResetColor();
                            throw new InvalidOperationException("PostBuildEvent not supported.");
                        }

                        if (xml.Contains("<ItemGroup") 
                            && xml.Contains("<Reference ", ignoreCase)
                            && xml.Contains("<ProjectReference"))
                        {
                            Console.ForegroundColor = ConsoleColor.Red;
                            Console.Error.WriteLine("Project contains an ItemGroup with mixed Reference and ProjectReference.");
                            Console.Error.WriteLine("This causes conversion problems. Please separate the ProjectReferences into a separate ItemGroup");
                            Console.Error.WriteLine("Offending XML:");
                            Console.ForegroundColor = ConsoleColor.Blue;
                            Console.WriteLine(xml);
                            Console.ResetColor();
                            throw new InvalidOperationException("Mixed Reference and ProjectReference not supported.");
                        }

                        sb.AppendLine(xml);
                    }
                }
            }

            string rawPreprocessed = sb.ToString();
            string finalPreprocessed = FormatXml(rawPreprocessed);

            // Do the initial conversion

            string toolPath = Environment.ExpandEnvironmentVariables(@"%userprofile%\.dotnet\tools\dotnet-migrate-2017.exe");
            if (!File.Exists(toolPath))
            {
                throw new InvalidOperationException($"Expected {toolPath} to exist.");
            }

            ProcessStartInfo psi = new ProcessStartInfo(toolPath, $"migrate {this.filePath}");
            psi.RedirectStandardOutput = true;

            using (Process p = Process.Start(psi))
            {
                string output = p.StandardOutput.ReadToEnd();
                Console.WriteLine(output);
            }

            foreach ( var dir in Directory.GetDirectories(Path.GetDirectoryName(this.filePath), "Backup*"))
            {
                Directory.Delete(dir, true);
                Console.WriteLine($"Removed {dir}");
            }

            // Now do the first-pass post-processing.
            string tempPath = $"{this.filePath}.tmp.xxx";

            try
            {
                using (StreamReader sr = new StreamReader(this.filePath))
                using (StreamWriter sw = new StreamWriter(tempPath, append: false, encoding: Encoding.UTF8))
                {
                    string line;
                    bool intarget = false;

                    while ((line = sr.ReadLine()) != null)
                    {
                        if (line.Contains("<Target "))
                        {
                            intarget = true;
                        }
                        else if (line.Contains("</Target>"))
                        {
                            intarget = false;
                        }

                        if (line.Contains("<Project ") && !line.Contains("Microsoft.NET.Sdk"))
                        {
                            // Something went wrong.
                            throw new InvalidOperationException($"Failed to convert! Found unexpected: {line}");
                        }

                        if (ShouldExclude(line, this.filePath))
                        {
                            continue;
                        }

                        sw.WriteLine(line);

                        if (line.Contains("</PropertyGroup") && restoreIfNeeded.Count > 0 && !intarget)
                        {
                            string torestore = restoreIfNeeded.Pop();
                            if (torestore.StartsWith("<"))
                            {
                                torestore = "  " + torestore;
                            }

                            sw.WriteLine(torestore);
                        }
                    }
                }

                // Do the second pass post-processing
                sb.Clear();
                using (XmlReader reader = XmlReader.Create(tempPath))
                {
                    sb.Append(@"<?xml version=""1.0"" encoding=""utf-8""?>");
                    while (reader.Read())
                    {
                        if (reader.Name == "Project")
                        {
                            if (reader.IsStartElement())
                            {
                                sb.Append("<Project");
                                bool ok = reader.MoveToFirstAttribute();
                                while (ok)
                                {
                                    sb.Append(" ");
                                    sb.Append(reader.Name + @"=""" + reader.Value + @"""");
                                    ok = reader.MoveToNextAttribute();
                                }

                                sb.Append($">{Environment.NewLine}");
                            }
                            else
                            {
                                sb.AppendLine("</Project>");
                            }
                        }

                        if (reader.NodeType == XmlNodeType.Element && reader.Name != "Project")
                        {
                            string xml = reader.ReadOuterXml();

                            if (xml.Contains("<Reference Include")
                                && xml.Contains("<HintPath>")
                                && xml.Contains(@"..\packages", ignoreCase))
                            {
                                continue;
                            }

                            sb.AppendLine(xml);
                        }
                    }
                }

                string formatted = FormatXml(sb.ToString());

                var text = formatted.Split(Environment.NewLine);

                sb.Clear();

                foreach (string line in text)
                {
                    sb.AppendLine(Scrub(line));
                }

                File.WriteAllText(tempPath, sb.ToString());
                File.Copy(tempPath, this.filePath, true);
                Console.WriteLine($"Conversion finished for {this.filePath}");
            }
            finally
            {
                if (File.Exists(tempPath))
                {
                    File.Delete(tempPath);
                }
            }
        }

        private static bool ShouldExclude(string line, string filePath)
        {
            string allText = File.ReadAllText(filePath);

            string[] toexclude = new string[]
            {
                "<ProjectGuid",
                "<ProjectTypeGuids",
                "<VSToolsPath",
                "<IsCodedUITest",
                "<FileUpgradeFlags",
                "<TestProjectType",
                "<UpgradeBackupLocation",
                "<VisualStudioVersion",
                "<VSToolsPath",
                @"$(ProgramFiles)\Common Files\microsoft shared\VSTT\$(VisualStudioVersion)\UITestExtensionPackages</ReferencePath>",
                @"$(VSToolsPath)\TeamTest\Microsoft.TestTools.targets",
                "<SccProjectName",
                "<SccLocalPath",
                "<SccAuxPath",
                "<SccProvider",
                "<OldToolsVersion",
                "<PublishUrl",
                "<ProductVersion",
                "<Install>",
                "<InstallFrom>",
                "<UpdateEnabled",
                "<UpdateMode",
                "<UpdateInterval",
                "<UpdatePeriodically",
                "<UpdateRequired",
                "<MapFileExtensions",
                "<ApplicationRevision",
                "<ApplicationVersion",
                "<IsWebBootstrapper",
                "<UseApplicationTrust",
                "<BootstrapperEnabled",
                "<SchemaVersion",
            };

            foreach (string filter in toexclude)
            {
                if (line.Contains(filter, ignoreCase))
                {
                    return true;
                }
            }

            if (allText.Contains("MSTest.TestFramework", ignoreCase) 
                && allText.Contains("Microsoft.NET.Test.Sdk", ignoreCase)
                && allText.Contains("MSTest.TestAdapter", ignoreCase)
                && line.Contains("MsTest.Corext", ignoreCase))
                {
                    return true;
                }

            return false;
        }

        private static string Scrub(string input)
        {
            Dictionary<string, string> mappings = new Dictionary<string, string>
            {
                {
                    @"<PackageReference Include=""JetBrains.Annotations"" Version=""9.1.1"" />",
                    @"<PackageReference Include=""JetBrains.Annotations"" Version=""2018.2.1"" />"
                },

                {
                    @"<PackageReference Include=""MSTest.TestFramework"" Version=""1.3.2"" />",
                    @"<PackageReference Include=""MSTest.TestFramework"" Version=""1.4.0"" />"
                },

                {
                    @"<PackageReference Include=""MSTest.TestAdapter"" Version=""1.3.2"" />",
                    @"<PackageReference Include=""MSTest.TestAdapter"" Version=""1.4.0"" />"
                },

                {
                    @"<PackageReference Include=""Microsoft.NET.Test.Sdk"" Version=""15.7.2"" />",
                    @"<PackageReference Include=""Microsoft.NET.Test.Sdk"" Version=""15.9.0"" />"
                }
            };
            
            string updated = input;

            if (updated.Contains(@"..\packages", ignoreCase))
            {
                Match m = packageRegex.Match(updated);

                if (m.Success)
                {
                    var packageref = m.Result("$1");
                    int loc = packageref.IndexOf(@"..\packages\");
                    packageref = packageref.Substring(loc);
                    packageref = packageref.Replace(@"..\packages\", string.Empty);

                    Match m2 = numberPartRegex.Match(packageref);
                    if (m2.Success)
                    {
                        string pathPart = m2.Result("$1");
                        string versionPart = m2.Result("$2");
                        string filePart = m2.Result("$3");

                        packageref = $@"Include=""$(NuGetPackageRoot)\{pathPart}\{versionPart}\{filePart}""";
                    }

                    updated = updated.Replace(m.Result("$0"), packageref);
                }
            }

            Match m3 = xmlnsRegex.Match(updated);
            if (m3.Success)
            {
                updated = updated.Replace(m3.Result("$0"), ">");
            }

            foreach (var kvp in mappings)
            {
                updated = updated.Replace(kvp.Key, kvp.Value);
            }

            return updated;
        }

        private static string FormatXml(string input)
        {
            using (MemoryStream ms = new MemoryStream())
            using (XmlTextWriter writer = new XmlTextWriter(ms, Encoding.UTF8))
            {
                writer.Formatting = Formatting.Indented;

                XmlDocument doc = new XmlDocument();
                doc.LoadXml(input);
                doc.WriteContentTo(writer);

                writer.Flush();
                ms.Flush();

                return Encoding.UTF8.GetString(ms.ToArray());
            }
        }
    }
}
