namespace Test
{
    class Program
    {
        private static readonly Dictionary<string, int> dict = new Dictionary<string, int>
        {
            {"intlv3_pokã©mon", 1216},
            {"intlv3_pokémon", 1248}
        };

        static void Main(string[] args)
        {
            foreach (var item in dict)
            {
                Console.WriteLine($"{item.Key} -> {item.Value}");
            }
        }
    }
}