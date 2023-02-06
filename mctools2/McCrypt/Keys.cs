using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace McCrypt
{
    public class Keys
    {
        private static Random rng = new Random();
        public static string KeyDbFile = "";
        internal struct content
        {
            public string FriendlyId;
            public byte[] ContentKey;
        }
        public struct keysJsonStruct
        {
            public string id;
            public string contentKey;
        }

        public static string lastTitleAccountId = "";
        public static string lastMinecraftId = "";

        private static string lastDeviceId = "";
        private static List<content> contentList = new List<content>();
        public static string ExportKeysJson()
        {
            List<keysJsonStruct> keysJson = new List<keysJsonStruct>();
            foreach (content key in contentList.ToArray())
            {
                string ckey = Encoding.UTF8.GetString(key.ContentKey);
                if (ckey == "s5s5ejuDru4uchuF2drUFuthaspAbepE")
                    continue;

                keysJsonStruct kjs = new keysJsonStruct();
                kjs.id = key.FriendlyId;
                kjs.contentKey = ckey;
                keysJson.Add(kjs);
            }

            return JsonConvert.SerializeObject(keysJson);
        }
        private static byte[] deriveUserKey(string UserId, string DeviceId)
        {
            byte[] userBytes = Encoding.Unicode.GetBytes(UserId);
            byte[] deviceBytes = Encoding.Unicode.GetBytes(DeviceId);

            int kLen = userBytes.Length;
            if (deviceBytes.Length < kLen)
                kLen = deviceBytes.Length;

            byte[] key = new byte[kLen];

            for (int i = 0; i < kLen; i++)
            {
                key[i] = (byte)(deviceBytes[i] ^ userBytes[i]);
            }

            return key;
        }
        internal static string GenerateKey()
        {
            string allowedChars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890";
            string key = "";
            for (int i = 0; i < 32; i++)
            {
                key += allowedChars[rng.Next(0, allowedChars.Length)];
            }
            return key;
        }

        private static byte[] deriveEntKey(byte[] versionkey, byte[] titleaccountId)
        {
            int kLen = versionkey.Length;
            if (titleaccountId.Length < kLen)
                kLen = titleaccountId.Length;

            byte[] key = new byte[kLen];

            for (int i = 0; i < kLen; i++)
            {
                key[i] = (byte)(versionkey[i] ^ titleaccountId[i]);
            }

            return key;
        }

        private static byte[] deriveContentKey(byte[] UserKey, byte[] ContentKey)
        {
            int kLen = UserKey.Length;
            if (ContentKey.Length < kLen)
                kLen = ContentKey.Length;

            byte[] key = new byte[kLen];

            for (int i = 0; i < kLen; i++)
            {
                key[i] = (byte)(UserKey[i] ^ ContentKey[i]);
            }

            int ckLen = kLen / 2;
            byte[] contentKey = new byte[ckLen];

            for (int i = 0; i < kLen; i += 2)
            {
                contentKey[i / 2] = key[i];
            }

            return contentKey;
        }

        public static void AddKey(string FriendlyId, byte[] ContentKey, bool addToKeyCache = true)
        {
            if (LookupKey(FriendlyId) != null)
                return;

            string keyCacheEntry = FriendlyId + "=" + Encoding.UTF8.GetString(ContentKey);

            if (addToKeyCache && KeyDbFile != "")
                File.AppendAllText(KeyDbFile, keyCacheEntry + "\n");

            content content = new content();
            content.FriendlyId = FriendlyId;
            content.ContentKey = ContentKey;

            contentList.Add(content);
        }

        private static void readReceipt(string receiptData)
        {
            dynamic recData = Utils.JsonDecodeCloserToMinecraft(receiptData);
            string userId = recData.Receipt.EntityId;
            string deviceId = "";

            if (recData.Receipt.ReceiptData != null)
                deviceId = recData.Receipt.ReceiptData.DeviceId;

            if (deviceId == "" || deviceId == null)
                deviceId = lastDeviceId;

            if (deviceId == "" || deviceId == null)
                return;

            lastDeviceId = deviceId;

            byte[] userKey = deriveUserKey(userId, deviceId);

            // Derive content keys
            int totalEntitlements = recData.Receipt.Entitlements.Count;

            for (int i = 0; i < totalEntitlements; i++)
            {
                try
                {
                    string friendlyId = recData.Receipt.Entitlements[i].FriendlyId;
                    string contentKeyB64 = recData.Receipt.Entitlements[i].ContentKey;
                    if (contentKeyB64 == null)
                        continue;

                    byte[] contentKey = Utils.ForceDecodeBase64(contentKeyB64);
                    byte[] realContentKey = deriveContentKey(userKey, contentKey);

                    AddKey(friendlyId, realContentKey);

                }
                catch (Exception) { continue; }
            }

        }

        public static void ReadOptionsTxt(string optionsTxtPath)
        {
            string[] optionsTxt = File.ReadAllLines(optionsTxtPath);
            foreach (string option in optionsTxt)
            {
                string opt = option.Replace("\r", "").Replace("\n", "").Trim();

                string[] kvpair = opt.Split(':');
                if (kvpair.Length >= 2)
                {
                    if (kvpair[0].Trim() == "last_minecraft_id")
                    {
                        lastMinecraftId = kvpair[1].Trim().ToUpper();
                    }

                    if (kvpair[0].Trim() == "last_title_account_id")
                    {
                        lastTitleAccountId = kvpair[1].Trim().ToUpper();
                    }

                }
            }
        }

        private static string decryptEntitlementFile(string encryptedEnt)
        {
            int version = Int32.Parse(encryptedEnt.Substring(7, 1));
            byte[] versionkey;
            switch (version)
            {
                case 2:
                default:
                    versionkey = Encoding.UTF8.GetBytes("X(nG*ejm&E8)m+8c;-SkLTjF)*QdN6_Y");
                    break;
            }
            string deriveText = lastTitleAccountId + lastTitleAccountId;
            byte[] entKey = deriveEntKey(versionkey, Encoding.UTF8.GetBytes(deriveText));

            string entBase64 = encryptedEnt.Substring(8);
            byte[] entCiphertext = Utils.ForceDecodeBase64(entBase64);

            byte[] entPlaintext = Marketplace.decryptEntitlementBuffer(entCiphertext, entKey);

            return Encoding.UTF8.GetString(entPlaintext);
        }
        public static void ReadEntitlementFile(string entPath)
        {
            string jsonData = File.ReadAllText(entPath);

            
            if(jsonData.StartsWith("Version")) // Thanks mojang, this was a fun challange <3
            {
                jsonData = decryptEntitlementFile(jsonData);
            }
            dynamic entData;
            try
            {
                entData  = Utils.JsonDecodeCloserToMinecraft(jsonData);
            }
            catch (Exception) { return;  }

            string receiptB64 = entData.Receipt;

            if (receiptB64 == null)
                return;

            if (receiptB64.Split('.').Length <= 1)
                return;

            string receiptData = Encoding.UTF8.GetString(Utils.ForceDecodeBase64(receiptB64.Split('.')[1]));
            readReceipt(receiptData);
            int totalItems = entData.Items.Count;
            for (int i = 0; i < totalItems; i++)
            {
                string b64Data = entData.Items[i].Receipt;

                if (b64Data == null)
                    continue;

                if (b64Data.Split('.').Length <= 1)
                    continue;

                string recept = Encoding.UTF8.GetString(Utils.ForceDecodeBase64(b64Data.Split('.')[1]));
                readReceipt(recept);
            }
        }
        public static void ReadKeysDb(string keyFile)
        {
            KeyDbFile = keyFile;
            string[] keyList = File.ReadAllLines(keyFile);
            foreach (string key in keyList)
            {
                if (key.Contains('='))
                {
                    string[] keys = key.Split('=');
                    if (keys.Length >= 2)
                    {
                        string friendlyId = keys[0];
                        byte[] contentKey = Encoding.UTF8.GetBytes(keys[1]);
                        AddKey(friendlyId, contentKey, false);
                    }
                }
            }
        }
        public static byte[] LookupKey(string FriendlyId)
        {
            foreach (content content in contentList)
            {
                if (content.FriendlyId == FriendlyId)
                    return content.ContentKey;
            }
            return null;
        }
    }
}
