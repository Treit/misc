namespace Test
{
    using System;
    using System.Collections.Generic;

    class Program
    {
        static void Main()
        {
            int[] numbers = { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15 };

            PrintArray(numbers);
            WeirdShuffle(numbers);
            PrintArray(numbers);
        }

        private static void WeirdShuffle<T>(T[] arr)
            where T : struct
        {
            if (arr.Length == 0)
            {
                return;
            }

            if (!AllUnique(arr))
            {
                throw new ArgumentException("All values must be unique.", nameof(arr));
            }

            var indexMap = new Dictionary<T, int>(arr.Length);

            for (int i = 0; i < arr.Length; i++)
            {
                indexMap.Add(arr[i], i);
            }

            Random r = new Random();

            // Initial shuffle
            for (int i = 0; i < arr.Length - 2; i++)
            {
                RandomSwap(arr, i, r, indexMap);
            }
        }

        private static bool AllUnique<T>(T[] a)
            where T : struct
        {
            var check = new HashSet<T>(a);

            if (check.Count != a.Length)
            {
                return false;
            }

            return true;
        }

        private static void RandomSwap<T>(T[] arr, int x, Random r, Dictionary<T, int> indexMap)
        {
            while (true)
            {
                var y = r.Next(x, arr.Length);

                if (indexMap[arr[x]] == y)
                {
                    // Can't put it back to the original index!
                    continue;
                }

                var tmp = arr[x];
                arr[x] = arr[y];
                arr[y] = tmp;
                break;
            }
        }

        private static void PrintArray<T>(T[] arr)
        {
            Console.WriteLine(string.Join(',', arr));
        }

    }
}