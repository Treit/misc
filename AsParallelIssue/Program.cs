using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Http;

namespace Test
{
    class Program
    {
        static void Main(string[] args)
        {
            var sw = new StreamWriter(@"c:\temp\test.txt", append: true);
            sw.WriteLine("A");
            sw.WriteLine("B");
        }
    }
}
