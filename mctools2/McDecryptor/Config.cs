using McDecryptor.Properties;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace McDecryptor
{
    internal class Config
    {
        public static string LocalAppdata = Environment.GetEnvironmentVariable("LOCALAPPDATA");
        public static string RoamingAppdata = Environment.GetEnvironmentVariable("APPDATA");

        public static string ApplicationDirectory;
        private static List<string> searchFolders = new List<string>();
        private static List<string> searchModules = new List<string>();


        public static string MinecraftFolder;

        public static string LocalState;
        public static string LocalCache;

        public static string KeysDbPath;
        public static string PremiumCache;
        public static string ServerPackCache;
        public static string RealmsPremiumCache;


        public static string OptionsTxt;
        public static string OutFolder;

        public static bool CrackPacks;
        public static bool ZipPacks;
        public static string[] SearchFolders
        {
            get
            {
                return searchFolders.ToArray();
            }
        }

        public static string[] SearchModules
        {
            get
            {
                return searchModules.ToArray();
            }
        }

        private static void rebaseSearchFolders()
        {
            searchFolders.Clear();
            searchFolders.Add(ApplicationDirectory);
            searchFolders.Add(PremiumCache);
            searchFolders.Add(ServerPackCache);
            searchFolders.Add(RealmsPremiumCache);

            searchModules.Clear();
            searchModules.Add("resource_packs");
            searchModules.Add("skin_packs");
            searchModules.Add("world_templates");
            searchModules.Add("persona");
            searchModules.Add("behaviour_packs");
            searchModules.Add("resource");
        }
        private static void rebaseLocalData()
        {
            OutFolder = Path.Combine(LocalState, "games", "com.mojang");
            PremiumCache = Path.Combine(LocalState, "premium_cache");
            ServerPackCache = Path.Combine(LocalCache, "packcache");
            RealmsPremiumCache = Path.Combine(LocalCache, "premiumcache");
            rebaseSearchFolders();
        }
        private static void rebaseAll()
        {
            LocalState = Path.Combine(MinecraftFolder, "LocalState");
            LocalCache = Path.Combine(MinecraftFolder, "LocalCache", "minecraftpe");
            rebaseLocalData();
        }
        private static string resolve(string str)
        {
            str = str.Trim();
            str = str.Replace("$LOCALAPPDATA", LocalAppdata);
            str = str.Replace("$APPDATA", RoamingAppdata);

            str = str.Replace("$MCDIR", MinecraftFolder);
            str = str.Replace("$EXECDIR", ApplicationDirectory);

            str = str.Replace("$LOCALSTATE", LocalState);
            str = str.Replace("$LOCALCACHE", LocalCache);

            str = str.Replace("$PREMIUMCACHE", PremiumCache);
            str = str.Replace("$SERVERPACKCACHE", ServerPackCache);
            str = str.Replace("$REALMSPREMIUMCACHE", RealmsPremiumCache);

            str = str.Replace("$OUTFOLDER", OutFolder);
            return str;
        }

        public static void Init()
        {
            CrackPacks = true;
            ZipPacks = false;

            ApplicationDirectory = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);
            KeysDbPath = Path.Combine(ApplicationDirectory, "keys.db");
            MinecraftFolder = Path.Combine(LocalAppdata, "Packages", "Microsoft.MinecraftUWP_8wekyb3d8bbwe");
            rebaseAll();
        }

        public static void ReadConfig(string configFile)
        {
            if (File.Exists(configFile))
            {
                string[] configLines = File.ReadAllLines(configFile);
                foreach(string line in configLines)
                {
                    if (line.Trim().StartsWith("#"))
                        continue;

                    if (!line.Contains(":"))
                        continue;

                    string[] keyvalpair = line.Trim().Split(':');
                    
                    if (keyvalpair.Length < 2)
                        continue;

                    switch(keyvalpair[0])
                    {
                        case "MinecraftFolder":
                            MinecraftFolder = resolve(keyvalpair[1]);
                            rebaseAll();
                            break;

                        case "LocalState":
                            LocalState = resolve(keyvalpair[1]);
                            rebaseLocalData();
                            break;
                        case "LocalCache":
                            LocalCache = resolve(keyvalpair[1]);
                            rebaseLocalData();
                            break;

                        case "PremiumCache":
                            PremiumCache = resolve(keyvalpair[1]);
                            rebaseSearchFolders();
                            break;
                        case "ServerPackCache":
                            ServerPackCache = resolve(keyvalpair[1]);
                            rebaseSearchFolders();
                            break;
                        case "RealmsPremiumCache":
                            RealmsPremiumCache = resolve(keyvalpair[1]);
                            rebaseSearchFolders();
                            break;

                        case "OutputFolder":
                            OutFolder = resolve(keyvalpair[1]);
                            break;

                        case "KeysDb":
                            KeysDbPath = resolve(keyvalpair[1]);
                            break;

                        case "OptionsTxt":
                            OptionsTxt = resolve(keyvalpair[1]);
                            break;

                        case "AdditionalSearchDir":
                            searchFolders.Add(resolve(keyvalpair[1]));
                            break;
                        case "AdditionalModuleDir":
                            searchModules.Add(resolve(keyvalpair[1]));
                            break;

                        case "CrackThePacks":
                            CrackPacks = (resolve(keyvalpair[1]).ToLower() == "yes");
                            break;
                        case "ZipThePacks":
                            ZipPacks = (resolve(keyvalpair[1]).ToLower() == "yes");
                            break;

                    }
                }
            }
            else
            {
                File.WriteAllBytes(configFile, Resources.DefaultConfigFile);
                ReadConfig(configFile);
            }
        }

    }
}
