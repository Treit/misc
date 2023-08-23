namespace ProjectConverter
{
    using System;

    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                if (args.Length < 1)
                {
                    PrintUsage();
                    return;
                }

                ProjectConverter pc = new ProjectConverter(args[0]);
                pc.Convert();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        private static void PrintUsage()
        {
            Console.WriteLine();
            Console.WriteLine("Usage:");
            Console.WriteLine("ProjectConverter.exe <PathToProject>");
        }
    }
}
