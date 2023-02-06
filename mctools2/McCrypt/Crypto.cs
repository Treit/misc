using System;
using System.IO;
using System.Security.Cryptography;

namespace McCrypt
{
    internal class Crypto
    {
        internal static byte[] Sha256(byte[] data)
        {
            SHA256 sha256 = SHA256.Create();
            byte[] hash = sha256.ComputeHash(data);
            sha256.Dispose();
            return hash;
        }
        internal static byte[] Aes256CfbEncrypt(byte[] key, byte[] iv, byte[] data)
        {
            Aes aes = Aes.Create();
            aes.Mode = CipherMode.CFB;
            aes.Padding = PaddingMode.None;
            aes.BlockSize = 128;
            aes.KeySize = 256;

            ICryptoTransform aesEncryptor = aes.CreateEncryptor(key, iv);
            using (MemoryStream msEncrypt = new MemoryStream())
            {
                using (CryptoStream csEncrypt = new CryptoStream(msEncrypt, aesEncryptor, CryptoStreamMode.Write))
                {
                    csEncrypt.Write(data, 0, data.Length);

                    long totalWritten = data.Length;
                    while ((totalWritten % 16 != 0))
                    {
                        csEncrypt.WriteByte(0);
                        totalWritten++;
                    }

                    msEncrypt.Seek(0x00, SeekOrigin.Begin);
                    return msEncrypt.ToArray();
                }
            }
        }

        internal static byte[] Aes256CfbDecrypt(byte[] key, byte[] iv, byte[] data)
        {
            Aes aes = Aes.Create();
            aes.Mode = CipherMode.CFB;
            aes.Padding = PaddingMode.Zeros;
            aes.BlockSize = 128;
            aes.KeySize = 256;

            ICryptoTransform aesDecryptor = aes.CreateDecryptor(key, iv);
            using (MemoryStream msDecrypt = new MemoryStream())
            {
                msDecrypt.Write(data, 0, data.Length);

                while (msDecrypt.Length % 16 != 0)
                    msDecrypt.WriteByte(0);

                msDecrypt.Seek(0x00, SeekOrigin.Begin);
                using (CryptoStream csDecrypt = new CryptoStream(msDecrypt, aesDecryptor, CryptoStreamMode.Read))
                {
                    byte[] plaintext = new byte[msDecrypt.Length];
                    csDecrypt.Read(plaintext, 0x00, plaintext.Length);

                    Array.Copy(plaintext, data, data.Length);
                    return data;
                }
            }
        }

    }
}
