namespace TerminalStress
{
    using System;
    using System.Diagnostics;
    using System.Drawing;
    using System.IO;
    using System.Text;
    using System.Threading;

    class Program
    {
        static string[] s_alphabet = new string[] { "👻", "🎃" };

        static void Main(string[] args)
        {

            Random r = new Random();

#pragma warning disable SYSLIB0001
            Console.OutputEncoding = Encoding.UTF8;
            Console.Title = "👻 HAPPY HALLOWEEN! 🎃";
            Console.Clear();
            string s = string.Empty;

            var colors = new ConsoleColor[]
            {
                ConsoleColor.Black,
                ConsoleColor.Red,
                ConsoleColor.Green,
                ConsoleColor.Blue,
                ConsoleColor.Cyan,
                ConsoleColor.Magenta,
                ConsoleColor.Yellow,
                ConsoleColor.White,
                ConsoleColor.DarkYellow,
                ConsoleColor.DarkCyan,
                ConsoleColor.DarkBlue,
                ConsoleColor.DarkGray,
                ConsoleColor.DarkGreen,
                ConsoleColor.DarkMagenta,
                ConsoleColor.DarkRed
            };

            while (true)
            {
                try
                {
                    Console.SetCursorPosition(r.Next(Console.WindowWidth), r.Next(Console.WindowHeight));
                    if (r.Next(0, 100) > 50)
                    {
                        Console.Write("👻");
                        Thread.Sleep(10);
                    }
                    else
                    {
                        Console.Write("🎃");
                    }
                }
                catch
                {
                    Console.Write("🎃");
                }

                try
                {
                    var color = colors[r.Next(colors.Length)];
                    Console.ForegroundColor = color;
                    int n = r.Next(0, s_alphabet.Length);

                    var c = s_alphabet[n];
                    Console.Write(c);
                    s += c;
                }
                catch
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.Write("👀🤬💀👀👀💀");
                }
            }
        }
    }
}