namespace NetStandardTest
{
    using System;
    using System.Configuration;
    using System.IO;
    using System.Reflection;

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
