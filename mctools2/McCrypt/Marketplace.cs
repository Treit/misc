using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading;

namespace McCrypt
{
    public class Marketplace
    {
        // Hi mojang <3
        // https://www.youtube.com/watch?v=jIM6dN3ogbk
        // https://www.youtube.com/watch?v=mnnYCJNhw7w

        private static string[] dontEncrypt = new string[] { "manifest.json", "contents.json", "texts", "pack_icon.png", "ui"};
        private struct contentsJson
        {
            public int version;
            public List<Object> content;
        }
        private struct contentKeys
        {
            public string key;
            public string path;
        }

        private struct content
        {
            public string path;
        }

        // Checks if file is inside the filename blacklist
        private static bool shouldEncrypt(string relPath)
        {
            foreach (string part in relPath.Split('/'))
                if (dontEncrypt.Contains(part))
                    return false;
            return true;
        }

        // Removes "prid" NBT tag
        // This is the one that makes the game tell you to plz buy the pack
        public static void CrackLevelDat(string levelDatFile)
        {
            byte[] find = Encoding.UTF8.GetBytes("prid"); // bytes to locate
            byte[] leveldat = File.ReadAllBytes(levelDatFile); // read level.dat

            Int64 location = Utils.FindData(leveldat, find); // locate where "prid" is inside level.dat
            if (location != -1)
            {
                FileStream fs = File.Open(levelDatFile, FileMode.Open, FileAccess.ReadWrite); // Open the file for writing
                fs.Seek(location + 3, SeekOrigin.Begin);
                fs.WriteByte((byte)'a'); // Change to "pria" which the game will just ignore
                fs.Close();
            }
        }

        // Change all skins type to "free" instead of "paid"
        // This makes the game let you actually apply them
        public static void CrackSkinsJson(string skinsJsonFile)
        {
            File.WriteAllText(skinsJsonFile, File.ReadAllText(skinsJsonFile).Replace("\"paid\"", "\"free\"")); // Read file, replace all "paid" with "free", write file back
        }

        // Extract a zipe file to the folder its contained in
        // And delete the zipe file.
        public static void CrackZipe(string zipeFile)
        {
            ZipFile.ExtractToDirectory(zipeFile, Path.GetDirectoryName(zipeFile));
            File.Delete(zipeFile);
        }

        // EncryptContents Overload to generate keys
        public static string EncryptContents(string basePath, string uuid)
        {
            return EncryptContents(basePath, uuid, Keys.GenerateKey());
        }


        // Encrypts a contents.json and all files in it-
        public static string EncryptContents(string basePath, string uuid, string ContentKey) 
        {
            string contentsJsonPath = Path.Combine(basePath, "contents.json"); // Path to contents.json

            contentsJson contentsJson = new contentsJson();
            contentsJson.version = 1;
            contentsJson.content = new List<object>();

            foreach (string entry in Directory.GetFileSystemEntries(basePath, "*", SearchOption.AllDirectories))
            {
                string relPath = entry.Remove(0, basePath.Length + 1); // Get path relative to pack folder
                relPath = relPath.Replace("\\", "/"); // Replace Windows-Style paths, with UNIX paths

                bool shouldEnc = shouldEncrypt(relPath);

                if (Utils.IsDirectory(entry)) // If its a directroy, add "/" to the end to signify this
                {
                    relPath += "/";
                    shouldEnc = false;
                } 

                if (shouldEnc) // Check file is not blacklisted file
                {
                    contentKeys keys = new contentKeys();
                    keys.path = relPath;
                    keys.key = Keys.GenerateKey(); // Generate a random key for this file

                    byte[] key = Encoding.UTF8.GetBytes(keys.key); // Copy first 16 bytes of key as IV
                    byte[] iv = new byte[16];
                    Array.Copy(key, iv, 16);


                    byte[] encryptedData = Crypto.Aes256CfbEncrypt(key, iv, File.ReadAllBytes(entry)); // Encrypt the file
                    File.WriteAllBytes(entry, encryptedData); // Write file

                    contentsJson.content.Add(keys); // add to content list
                }
                else // Just add to the content list without encrypting it
                {   
                    content content = new content();
                    content.path = relPath;
                    contentsJson.content.Add(content);
                }

            }

            string json = JsonConvert.SerializeObject(contentsJson); // JSON Encode contents.json

            byte[] contentKey = Encoding.UTF8.GetBytes(ContentKey); // Copy first 16 bytes of the key for IV
            byte[] contentIv = new byte[16];
            Array.Copy(contentKey, contentIv, 16);

            byte[] encryptedJson = Crypto.Aes256CfbEncrypt(contentKey, contentIv, Encoding.UTF8.GetBytes(json)); // Encrypt JSON

            // Create encrypted file w header
            FileStream fs = File.OpenWrite(contentsJsonPath); 
            BinaryWriter bw = new BinaryWriter(fs);
            bw.Write((uint)0);
            bw.Write((uint)0x9BCFB9FC);
            bw.Write((UInt64)0);
            fs.WriteByte((byte)uuid.Length);
            Utils.WriteString(fs, uuid, 0xEF);
            fs.Write(encryptedJson, 0, encryptedJson.Length);
            fs.Close();
            return ContentKey;
        }

        // Decrypt encrypted entitlement buffer using a specified ent key
        internal static byte[] decryptEntitlementBuffer(byte[] EntCiphertext, byte[] EntKey)
        {
            byte[] iv = new byte[16];
            Array.Copy(EntKey, iv, iv.Length);
            return Crypto.Aes256CfbDecrypt(EntKey, iv, EntCiphertext);
        }

        // Decrypt a world (leveldb) or contents.json file
        // For the file types that have a header-
        private static byte[] worldOrContentsJsonDecrypt(string file)
        {
            FileStream fs = File.OpenRead(file); // Open file for reading
            BinaryReader br = new BinaryReader(fs); // Create a binary reader overlay of it

            if (fs.Length <= 0)
            {
                fs.Dispose();
                return new byte[0] { };
            }

            uint version = br.ReadUInt32();
            uint magic = br.ReadUInt32();
            UInt64 unk = br.ReadUInt64();

            if (version == 0 && magic == 0x9bcfb9fc) // is valid header?
            {

                int len = fs.ReadByte();
                string uuid = Utils.ReadString(fs, len); // Read the pack UUID for this file
                byte[] key = Keys.LookupKey(uuid); // Look for key inside .ent / keys.db


                if (key == null)
                    key = Encoding.UTF8.GetBytes("s5s5ejuDru4uchuF2drUFuthaspAbepE"); // Generic skinpack key 
                                                                                      // Every skinpack has the same key lol
                                                                                      // This might be wrong, but hey! if it works, it works :D
                fs.Seek(0x100, SeekOrigin.Begin); // Read ciphertext
                byte[] ciphertext = new byte[fs.Length - 0x100];
                fs.Read(ciphertext, 0x00, ciphertext.Length);


                byte[] iv = new byte[16]; // Copy first 16 bytes of Key for IV
                Array.Copy(key, iv, iv.Length);

                byte[] plaintext = Crypto.Aes256CfbDecrypt(key, iv, ciphertext); // Decrypt data

                fs.Dispose();
                return plaintext;

            }
            else
            {
                fs.Dispose();
                throw new InvalidDataException("Not a valid LEVELDB or CONTENTS.JSON file.");
            }

        }


        // Read contents.json, and decrypt all files inside
        // Now Multi-Threaded for speed!
        private static void decryptContentsJsonFiles(string contentsJsonPath, List<Thread> threadList)
        {
            string baseDirectory = Path.GetDirectoryName(contentsJsonPath); // Get pack folder
            string contentsJson = File.ReadAllText(contentsJsonPath); // Read contents.json
            dynamic contentsJsonData = Utils.JsonDecodeCloserToMinecraft(contentsJson); // Parse contents.json

            int totalContents = contentsJsonData.content.Count;
            for (int i = 0; i < totalContents; i++)
            {
                string relPath = contentsJsonData.content[i].path; // Relative path to file to be decrypted
                string decKey = contentsJsonData.content[i].key; // Key for file to be decrypted
                if (decKey == null)
                    continue;

                Thread thrd = new Thread(() =>
                {
                    string filePath = Path.Combine(baseDirectory, relPath); // Combine pack dir, with file relative path
                    byte[] key = Encoding.UTF8.GetBytes(decKey); // Get key bytes
                    byte[] iv = new byte[16]; 
                    Array.Copy(key, iv, iv.Length); // Copy first 16 bytes of key as IV

                    byte[] cipherText = File.ReadAllBytes(filePath); // Read the file
                    byte[] plainText = Crypto.Aes256CfbDecrypt(key, iv, cipherText); // Decrypt the file
                    File.WriteAllBytes(filePath, plainText); // Write back decrypted filie
                });
                thrd.Priority = ThreadPriority.Highest;
                threadList.Add(thrd);
                thrd.Start();
            }
        }

        // Decrypt an entire pack / world
        // Recursively decrypts all sub-packs.
        // Mutli-Threaded.
        public static void DecryptContents(string contentsPath)
        {
            List<Thread> threadList = new List<Thread>();
            string oldSchoolZipe = Path.Combine(contentsPath, "content.zipe");
            string contentsJsonPath = Path.Combine(contentsPath, "contents.json");
            if (File.Exists(oldSchoolZipe)) // Resource packs or Skin Packs
            {
                byte[] decryptedData = worldOrContentsJsonDecrypt(oldSchoolZipe); // Decrypt the zipe file
                File.WriteAllBytes(oldSchoolZipe, decryptedData); // Write decrypted zip back to disk
            }
            else if (File.Exists(contentsJsonPath)) // Resource packs or Skin Packs
            {
                string subPacksFolder = Path.Combine(contentsPath, "subpacks");

                byte[] decryptedData = worldOrContentsJsonDecrypt(contentsJsonPath); // Decrypt the contents.json file
                File.WriteAllBytes(contentsJsonPath, decryptedData); // Write decrypted contents.json back to disk
                decryptContentsJsonFiles(contentsJsonPath, threadList); // Decrypt all files in contents.json

                // Decrypt all Sub-packs
                if (Directory.Exists(subPacksFolder))
                {
                    string[] subPacks = Directory.GetDirectories(subPacksFolder, "*", SearchOption.TopDirectoryOnly);
                    foreach (string subPack in subPacks)
                        DecryptContents(Path.Combine(subPacksFolder, subPack));
                }
            }
            else // World Templates
            {
                string behaviourPacksFolder = Path.Combine(contentsPath, "behavior_packs"); // Get World Resource Packs folder
                string resourcePacksFolder = Path.Combine(contentsPath, "resource_packs"); // Get World Behaviour Packs folder 
                string levelDbFolder = Path.Combine(contentsPath, "db"); // Get leveldb folder

                // Decrypt all sub-behavour packs
                if (Directory.Exists(behaviourPacksFolder))
                {
                    string[] behaviourPacks = Directory.GetDirectories(behaviourPacksFolder, "*", SearchOption.TopDirectoryOnly);
                    foreach (string behaviourPack in behaviourPacks)
                        DecryptContents(Path.Combine(behaviourPacksFolder, behaviourPack));
                }

                // Decrypt all sub-resource packs
                if (Directory.Exists(resourcePacksFolder))
                {
                    string[] resourcePacks = Directory.GetDirectories(resourcePacksFolder, "*", SearchOption.TopDirectoryOnly);
                    foreach (string resourcePack in resourcePacks)
                        DecryptContents(Path.Combine(resourcePacksFolder, resourcePack));
                }

                // Decrypt leveldb files
                if (Directory.Exists(levelDbFolder))
                {
                    string[] levelDbFiles = Directory.GetFiles(levelDbFolder, "*", SearchOption.AllDirectories);
                    foreach (string levelDbFile in levelDbFiles)
                    {
                        Thread thrd = new Thread(() =>
                        {
                            string fileToDecrypt = Path.Combine(levelDbFolder, levelDbFile); // Get full path to leveldb file
                            byte[] decryptedData;
                            try
                            {
                                decryptedData = worldOrContentsJsonDecrypt(fileToDecrypt); // Decrypr file
                                File.WriteAllBytes(fileToDecrypt, decryptedData); // Write to disk
                            }
                            catch (InvalidDataException)
                            {
                                Console.Error.WriteLine("Failed to decrypt db/" + Path.GetFileName(levelDbFile));
                            }
                        });
                        thrd.Priority = ThreadPriority.Highest;
                        threadList.Add(thrd);
                        thrd.Start();
                    }
                }
            }

            Thread[] threads = threadList.ToArray();
            threadList.Clear();

            foreach(Thread t in threads)
                t.Join();
        }
    }
}
