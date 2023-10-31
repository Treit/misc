namespace TerminalStress
{
    using System;
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

            while (true)
            {
                try
                {
                    Console.SetCursorPosition(r.Next(Console.WindowWidth), r.Next(Console.WindowHeight));
                    if (r.Next(0, 100) > 50)
                    {
                        Console.Write(s_alphabet[0]);
                        Thread.Sleep(10);
                    }
                    else
                    {
                        Console.Write(s_alphabet[1]);
                    }
                }
                catch
                {
                    Console.Write("😨");
                }

                try
                {
                    int n = r.Next(0, s_alphabet.Length);
                    var c = s_alphabet[n];
                    Console.Write(c);
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