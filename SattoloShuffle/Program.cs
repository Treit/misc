namespace Test
{
    using System;

    class Program
    {
        static void Main()
        {
            int[] numbers = { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15 };

            while (true)
            {
                PrintArray(numbers);
                SattoloShuffle(numbers);
                if (!Validate(numbers))
                {
                    break;
                }
                PrintArray(numbers);
            }
        }

        private static bool Validate(int[] arr)
        {
            for (int i = 1; i <= arr.Length; i++)
            {
                if (arr[i - 1] == i)
                {
                    Console.WriteLine($"Fail: {i}");
                    return false;
                }
            }

            return true;
        }

        private static void SattoloShuffle<T>(T[] arr)
            where T : struct
        {
            if (arr.Length == 0)
            {
                return;
            }

            Random r = new Random();

            for (int i = 0; i < arr.Length - 2; i++)
            {
                Swap(arr, i, r);
            }
        }

        private static void Swap<T>(T[] arr, int x, Random r)
        {
            var y = r.Next(x + 1, arr.Length);
            var tmp = arr[x];
            arr[x] = arr[y];
            arr[y] = tmp;
        }

        private static void PrintArray<T>(T[] arr)
        {
            Console.WriteLine(string.Join(',', arr));
        }
    }
}