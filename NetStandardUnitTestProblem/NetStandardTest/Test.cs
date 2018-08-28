namespace NetStandardTest
{
    using System;
    using System.Configuration;

    public static class Test
    {
        public static void DoSomething()
        {
            string s = ConfigurationManager.AppSettings["Test"];

            if (s == null)
            {
                Console.WriteLine("The configuration setting 'Test' is not set.");
            }
            else
            {
                Console.WriteLine($"The configuration setting 'Test' is set to: '{s}'");
            }
        }
    }
}
