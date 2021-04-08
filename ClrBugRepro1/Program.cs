// Repro taken from https://gist.github.com/ChrisAhna/3813c637a7dabc25fd7e3557d5a0b6e1 as part of the
// investigation into https://github.com/dotnet/runtime/issues/50952.

using System;
using System.Collections.Generic;
using System.Runtime;
using System.Threading;

namespace Test
{
    public static partial class Util
    {
        public static Exception Fail(string format, params object[] args)
        {
            Console.Write("FAIL! {0}", String.Format(format, args));
            Environment.Exit(101);
            throw new Exception("Not reached.");
        }

        public static void Log(string format, params object[] args)
        {
            Console.Write("{0}", String.Format(format, args));
            return;
        }
    }

    public class Fields1
    {
        public object Ref1;
        public object Ref2;
    }

    public class Fields2 : Fields1
    {
        public Int64? Vt000;
        public Int64? Vt001;
        public Int64? Vt002;
        public Int64? Vt003;
        public Int64? Vt004;
        public Int64? Vt005;
        public Int64? Vt006;
        public Int64? Vt007;
        public Int64? Vt008;
        public Int64? Vt009;
        public Int64? Vt010;
        public Int64? Vt011;
        public Int64? Vt012;
        public Int64? Vt013;
        public Int64? Vt014;
        public Int64? Vt015;
        public Int64? Vt016;
        public Int64? Vt017;
        public Int64? Vt018;
        public Int64? Vt019;
        public Int64? Vt020;
        public Int64? Vt021;
        public Int64? Vt022;
        public Int64? Vt023;
        public Int64? Vt024;
        public Int64? Vt025;
        public Int64? Vt026;
        public Int64? Vt027;
        public Int64? Vt028;
        public Int64? Vt029;
        public Int64? Vt030;
        public Int64? Vt031;
        public Int64? Vt032;
        public Int64? Vt033;
        public Int64? Vt034;
        public Int64? Vt035;
        public Int64? Vt036;
        public Int64? Vt037;
        public Int64? Vt038;
        public Int64? Vt039;
        public Int64? Vt040;
        public Int64? Vt041;
        public Int64? Vt042;
        public Int64? Vt043;
        public Int64? Vt044;
        public Int64? Vt045;
        public Int64? Vt046;
        public Int64? Vt047;
        public Int64? Vt048;
        public Int64? Vt049;
        public Int64? Vt050;
        public Int64? Vt051;
        public Int64? Vt052;
        public Int64? Vt053;
        public Int64? Vt054;
        public Int64? Vt055;
        public Int64? Vt056;
        public Int64? Vt057;
        public Int64? Vt058;
        public Int64? Vt059;
        public Int64? Vt060;
        public Int64? Vt061;
        public Int64? Vt062;
        public Int64? Vt063;
        public Int64? Vt064;
        public Int64? Vt065;
        public Int64? Vt066;
        public Int64? Vt067;
        public Int64? Vt068;
        public Int64? Vt069;
        public Int64? Vt070;
        public Int64? Vt071;
        public Int64? Vt072;
        public Int64? Vt073;
        public Int64? Vt074;
        public Int64? Vt075;
        public Int64? Vt076;
        public Int64? Vt077;
        public Int64? Vt078;
        public Int64? Vt079;
        public Int64? Vt080;
        public Int64? Vt081;
        public Int64? Vt082;
        public Int64? Vt083;
        public Int64? Vt084;
        public Int64? Vt085;
        public Int64? Vt086;
        public Int64? Vt087;
        public Int64? Vt088;
        public Int64? Vt089;
        public Int64? Vt090;
        public Int64? Vt091;
        public Int64? Vt092;
        public Int64? Vt093;
        public Int64? Vt094;
        public Int64? Vt095;
        public Int64? Vt096;
        public Int64? Vt097;
        public Int64? Vt098;
        public Int64? Vt099;
        public Int64? Vt100;
        public Int64? Vt101;
        public Int64? Vt102;
        public Int64? Vt103;
        public Int64? Vt104;
        public Int64? Vt105;
        public Int64? Vt106;
        public Int64? Vt107;
        public Int64? Vt108;
        public Int64? Vt109;
        public Int64? Vt110;
        public Int64? Vt111;
        public Int64? Vt112;
        public Int64? Vt113;
    }

    public class Fields3 : Fields2
    {
        public string Str1;
    }

    public class Fields4 : Fields3
    {
        public object Ref3;
        public Int64 Vt200;
        public Int64 Vt201;
        public Int32 Vt202;
    }

    public sealed class ObjectSet
    {
        private readonly Fields4[] allMembers;


        public ObjectSet(int objectCount)
        {
            int index;
            Fields4[] members;

            members = new Fields4[objectCount];

            for (index = 0; index < members.Length; index++)
            {
                members[index] = new Fields4();
            }

            this.allMembers = members;
            return;
        }

        public void EndlesslyRefreshReferences()
        {
            Random generator;
            int index;
            Int64 iteration;
            Fields4[] members;
            int slotCount;

            generator = new Random();
            members = this.allMembers;
            slotCount = members.Length;
            index = 0;
            iteration = 0;

            while (true)
            {
                index = ((index + generator.Next(minValue: 0, maxValue: slotCount)) % slotCount);

                this.RefreshReferences(
                    instance: members[index],
                    index: index,
                    iteration: iteration
                );
            }

            // throw Util.Fail("Not reached.");
        }

        private void RefreshReferences(Fields4 instance, int index, Int64 iteration)
        {
            string capturedStr1;
            Int64 oldIteration;
            string oldString;

            oldIteration = instance.Vt200;

            if (oldIteration != 0)
            {
                oldString = this.BuildString(index: index, iteration: oldIteration);
                capturedStr1 = instance.Str1;

                if (capturedStr1 != oldString)
                {
                    throw Util.Fail("Error: Observed a stale/bad Str1 value: {0} vs {1}\r\n", capturedStr1.Length, oldString.Length);
                }
            }

            instance.Str1 = this.BuildString(index: index, iteration: iteration);
            instance.Vt200 = iteration;
            return;
        }

        private string BuildString(int index, Int64 iteration)
        {
            return String.Format(
                "{0:x8}_{1:x16}",
                index,
                iteration
            );
        }
    }

    static class App
    {
        const UInt64 ObjectSize = 0x768UL;
        const int ObjectsPerAllocatorThread = checked((int)(0x0000000050000000UL / ObjectSize));
        const int AllocatorThreadCount = 4;

        static void Main()
        {
            int index;
            List<Thread> threadList;

            if (!GCSettings.IsServerGC)
            {
                throw Util.Fail("Server GC must be enabled.\r\n");
            }

            threadList = new List<Thread>();

            for (index = 0; index < AllocatorThreadCount; index++)
            {
                threadList.Add(
                    new Thread(
                        App.RunAllocatorThread
                    )
                );
            }

            foreach (Thread thread in threadList)
            {
                thread.Start();
            }

            foreach (Thread thread in threadList)
            {
                thread.Join();
            }

            throw Util.Fail("Error: Threads are not expected to exit.\r\n");
        }

        private static void RunAllocatorThread()
        {
            ObjectSet objectSet;

            objectSet = new ObjectSet(
                objectCount: ObjectsPerAllocatorThread
            );

            objectSet.EndlesslyRefreshReferences();
            throw Util.Fail("Error: Endless refresh function is not expected to return.\r\n");
        }
    }
}

