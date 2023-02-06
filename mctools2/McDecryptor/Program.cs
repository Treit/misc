using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Threading;
using McCrypt;

namespace McDecryptor
{
    class Program
    {
        public struct ContentListing
        {
            public string Path;
            public string Type;
            public string Name;
            public int Id;
        }
        public static string EscapeFilename(string filename)
        {
            return filename.Replace("/", "_").Replace("\\", "_").Replace(":", "_").Replace("?", "_").Replace("*", "_").Replace("<", "_").Replace(">", "_").Replace("|", "_").Replace("\"", "_");
        }

        static void CopyDirectory(string sourcePath, string targetPath)
        {
            List<Thread> threads = new List<Thread>();

            //Now Create all of the directories
            foreach (string dirPath in Directory.GetDirectories(sourcePath, "*", SearchOption.AllDirectories))
            {
                Thread thrd = new Thread(() =>
                {
                    Directory.CreateDirectory(dirPath.Replace(sourcePath, targetPath));
                });
                thrd.Priority = ThreadPriority.Highest;
                thrd.Start();
                threads.Add(thrd);
            }

            foreach (Thread t in threads.ToArray())
                t.Join();
            threads.Clear();

            //Copy all the files & Replaces any files with the same name
            foreach (string newPath in Directory.GetFiles(sourcePath, "*", SearchOption.AllDirectories))
            {

                Thread thrd = new Thread(() =>
                {
                    File.Copy(newPath, newPath.Replace(sourcePath, targetPath), true);
                });
                thrd.Priority = ThreadPriority.Highest;
                thrd.Start();
                threads.Add(thrd);

            }

            foreach (Thread t in threads.ToArray())
                t.Join();

            threads.Clear();
        }

        static void Main(string[] args)
        {
            Console.WriteLine("-- McDecryptor --");

            Console.WriteLine("Reading Configuration File...");
            Config.Init();
            Directory.SetCurrentDirectory(Config.ApplicationDirectory);
            Config.ReadConfig("McDecryptor.cfg");
            
            Keys.KeyDbFile = Config.KeysDbPath;
            if (File.Exists(Config.KeysDbPath))
            {
                Console.WriteLine("Parsing Key Database File...");
                Keys.ReadKeysDb(Config.KeysDbPath);
            }


            if (File.Exists(Config.OptionsTxt))
            {
                Keys.ReadOptionsTxt(Config.OptionsTxt);
            }

            Console.WriteLine("Minecraft Folder: " + Config.MinecraftFolder);


            if (Directory.Exists(Config.MinecraftFolder))
            {
                string[] entFiles = Directory.GetFiles(Config.LocalState, "*.ent", SearchOption.TopDirectoryOnly);

                foreach (string entFile in entFiles)
                {
                    Console.WriteLine("Reading Entitlement File: " + Path.GetFileName(entFile));
                    Keys.ReadEntitlementFile(entFile);
                }
            }


            string[] entFilesWorkDir = Directory.GetFiles(Config.ApplicationDirectory, "*.ent", SearchOption.TopDirectoryOnly);

            foreach (string entFile in entFilesWorkDir)
            {
                Console.WriteLine("Reading Entitlement File: " + Path.GetFileName(entFile));
                Keys.ReadEntitlementFile(entFile);
            }

            
            List<ContentListing> premiumContents = new List<ContentListing>();
            Console.WriteLine("\n\n");
            Console.WriteLine("Select what to decrypt: ");
            int total = 1;


            foreach (string searchFolder in Config.SearchFolders)
            {
                foreach(string searchModule in Config.SearchModules)
                {
                    string moduleFolder = Path.Combine(searchFolder, searchModule);

                    if (Directory.Exists(moduleFolder))
                    {
                        foreach (string moduleItem in Directory.GetDirectories(moduleFolder, "*", SearchOption.TopDirectoryOnly))
                        {
                            ContentListing cList = new ContentListing();
                            cList.Name = Manifest.ReadName(Path.Combine(moduleItem, "manifest.json"));
                            cList.Type = searchModule;
                            cList.Id = total;
                            cList.Path = moduleItem;

                            premiumContents.Add(cList);

                            Console.WriteLine(cList.Id.ToString() + ") (" + cList.Type + ") " + cList.Name);

                            total++;
                        }
                    }
                }
            }

            Console.WriteLine("Select multiple (seperated by ',') or write \"ALL\"");

            List<int> toDecrypt = new List<int>();
            while (true)
            {
                Console.Write("Which do you want to decrypt? ");
                try
                {
                    string readText = Console.ReadLine();
                    if (readText.ToUpper() == "ALL")
                    {
                        for(int i = 0; i < total-1; i++)
                            toDecrypt.Add(i);
                        break;
                    }
                    string[] entries = readText.Split(',');

                    foreach(string entry in entries) {
                        int tdc = Convert.ToInt32(entry.Trim())-1;
                        if (tdc < 0 || tdc >= total)
                            continue;
                        toDecrypt.Add(tdc);
                    }

                    break;
                }
                catch (Exception) { }
            }

            

            foreach (int decryptMe in toDecrypt.ToArray())
            {
                ContentListing cListing = premiumContents.ToArray()[decryptMe];
                string outFolder = Path.Combine(Config.OutFolder, cListing.Type, EscapeFilename(cListing.Name));

                int counter = 1;
                string ogOutFolder = outFolder;
                while (Directory.Exists(outFolder))
                {
                    outFolder = ogOutFolder + "_" + counter.ToString();
                    counter++;
                }

                new Thread(() =>
                {
                    Console.WriteLine("Decrypting: " + cListing.Name);
                    Directory.CreateDirectory(outFolder);
                    CopyDirectory(cListing.Path, outFolder);
                    try 
                    {
                        string levelDatFile = Path.Combine(outFolder, "level.dat");
                        string skinsJsonFile = Path.Combine(outFolder, "skins.json");
                        string oldSchoolZipe = Path.Combine(outFolder, "content.zipe");

                        Marketplace.DecryptContents(outFolder);

                        if (Config.CrackPacks)
                        {
                            if (File.Exists(oldSchoolZipe))
                                Marketplace.CrackZipe(oldSchoolZipe);

                            if (File.Exists(levelDatFile))
                                Marketplace.CrackLevelDat(levelDatFile);

                            if (File.Exists(skinsJsonFile))
                                Marketplace.CrackSkinsJson(skinsJsonFile);
                        }

                        if (Config.ZipPacks)
                        {
                            string ext = "";
                            if (File.Exists(levelDatFile))
                                ext += ".mctemplate";
                            else
                                ext += ".mcpack";

                            ZipFile.CreateFromDirectory(outFolder, outFolder + ext, CompressionLevel.NoCompression, false);
                            Directory.Delete(outFolder, true);
                        }
                    }
                    catch (Exception)
                    {
                        Console.Error.WriteLine("Failed to decrypt: " + cListing.Name);
                        Directory.Delete(outFolder, true);
                    }

                }).Start();
            }
        }

    }
}
