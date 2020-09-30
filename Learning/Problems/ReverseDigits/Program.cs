using System;
using System.Runtime.InteropServices.ComTypes;

namespace ReverseDigits
{
    class Program
    {
        static void Main(string[] args)
        {
            // Reverse the digits of the input.
            if (args.Length == 0 || !int.TryParse(args[0], out int val))
            {
                Console.WriteLine("Provide an integer value to process.");
                return;
            }

            int numDigits = CountDigits(val);

            int mult = 1;
            for (int i = 0; i < numDigits - 1; i++)
            {
                mult *= 10;
            }

            int result = 0;

            while (mult > 0)
            {
                int digit = val % 10;

                if (result == 0)
                {
                    result = mult * digit;
                }
                else
                {
                    result += mult * digit;
                }

                mult /= 10;
                val /= 10;
            }

            Console.WriteLine(result);
        }

        private static int CountDigits(int val)
        {
            if (val == 0 || val == 1)
            {
                return 1;
            }

            return (int)Math.Ceiling(Math.Log10(val));
        }
    }
}
