using System;
using System.Text;

namespace Test
{
    class Program
    {
        public static void Main()
        {
            var r = new Random();

            while (true)
            {
                for (int i = 0; i < 1000; i++)
                {
                    Task.Factory.StartNew(() =>
                    {
                        var str = GetRandomString(r, 10000);

                        if (str.Length == 9999)
                        {
                            Console.Write("!");
                        }

                        Console.Write(".");
                    });
                }
            }
        }
        static string GetRandomString(Random r, int maxLength)
        {
            var alphabet = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz-_@!#$%^&*()+{}[]";
            var len = r.Next(maxLength);
            var sb = new StringBuilder(len);

            for (int i = 0; i < len; i++)
            {
                sb.Append(alphabet[r.Next(alphabet.Length)]);
            }

            return sb.ToString();
        }
    }
}
