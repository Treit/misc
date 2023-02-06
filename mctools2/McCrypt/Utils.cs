using Newtonsoft.Json;
using System;
using System.IO;
using System.Text;

namespace McCrypt
{
    internal class Utils
    {

        internal static object JsonDecodeCloserToMinecraft(string json)
        {
            for (int i = json.Length; i > 0; i--)
            {
                try
                { 
                    return JsonConvert.DeserializeObject(json.Substring(0, i));
                }
                catch (Exception) { };
            }
            throw new Exception();
        }
        internal static bool IsDirectory(string path)
        {
            if (Directory.Exists(path))
                return true;
            else if (File.Exists(path))
                return false;
            else
                throw new FileNotFoundException("Cannot find file: " + path);
        }

        internal static Int64 FindData(byte[] data, byte[] pattern)
        {

            for (Int64 i = 0; i < data.LongLength - pattern.LongLength; i++)
            {
                bool match = true;
                for (Int64 k = 0; k < pattern.LongLength; k++)
                {
                    if (data[i + k] != pattern[k])
                    {
                        match = false;
                        break;
                    }
                }
                if (match)
                {
                    return i;
                }
            }
            return -1;

        }

        internal static string ReadString(Stream str, int len)
        {
            byte[] stringBytes = new byte[len];
            str.Read(stringBytes, 0x00, len);
            return Encoding.UTF8.GetString(stringBytes);
        }
        internal static void WriteString(Stream stream, string str, long totalLength)
        {
            byte[] data = Encoding.UTF8.GetBytes(str);
            long paddingLen = totalLength - data.Length;
            byte[] padding = new byte[paddingLen];
            stream.Write(data, 0, data.Length);
            stream.Write(padding, 0, padding.Length);
        }

        internal static byte[] ForceDecodeBase64(string base64Data)
        {
            for (int i = 0; i < 20; i++)
            {
                try
                {
                    return Convert.FromBase64String(base64Data);
                }
                catch (Exception)
                {
                    base64Data += "=";
                }
            }
            return null;

        }
        internal static string TrimName(string name)
        {
            if (name.Contains("#"))
            {
                return name.Substring(0, name.IndexOf("#")).Trim();
            }
            return name.Trim();
        }

    }
}
