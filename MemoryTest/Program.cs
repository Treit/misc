using System;
using System.Runtime.InteropServices;

namespace Test
{
    class MemoryTest
    {
        static byte[]? _buffer;

        public static void Main(string[] args)
        {
            DoTest();
        }

        static void DoTest()
        {
            Console.WriteLine("About to allocate buffer.");
            Console.ReadKey();

            _buffer = new byte[1024 * 1024 * 1024];
            Console.WriteLine("Buffer was allocated.");
            Console.ReadKey();

            _buffer[_buffer.Length - 1] = 0xFF;
            Console.WriteLine("Wrote a value at the end of the buffer.");
            Console.ReadKey();

            for (int i = 0; i < _buffer.Length / 2; i++)
            {
                if (_buffer[i] == 0xFF)
                {
                    Console.WriteLine($"Needle found at {i}.");
                }
            }

            Console.WriteLine("Enumerated half the buffer.");
            Console.ReadKey();

            for (int i = _buffer.Length / 2; i < _buffer.Length; i++)
            {
                if (_buffer[i] == 0xFF)
                {
                    Console.WriteLine($"Needle found at {i}.");
                }
            }

            Console.WriteLine("Enumerated rest of the buffer.");
            Console.ReadKey();

            var r = new Random();
            r.NextBytes(_buffer);
            Console.WriteLine("Filled the buffer with random values..");
            Console.ReadKey();

            int size = 1024 * 1024 * 1024; // 1GB

            var ptr = AllocateNativeMemory(size);

            Console.WriteLine("Allocated 1GB of native memory.");
            Console.ReadKey();

            unsafe
            {
                r.NextBytes(new Span<byte>(ptr.ToPointer(), size / 2));
            }

            Console.WriteLine("Filled half of native memory with random values.");
            Console.ReadKey();

            unsafe
            {
                r.NextBytes(new Span<byte>(ptr.ToPointer(), size));
            }

            Console.WriteLine("Filled all of native memory with random values.");
            Console.ReadKey();

            ReleaseMemory(ptr);

            Console.WriteLine("Released the native memory.");
            Console.ReadKey();

            IntPtr AllocateNativeMemory(int size)
            {
                unsafe
                {
                    void* ptr = NativeMemory.AllocZeroed((nuint)size);
                    return new IntPtr(ptr);
                }
            }

            void ReleaseMemory(IntPtr ptr)
            {
                unsafe
                {
                    NativeMemory.Free(ptr.ToPointer());
                }
            }
        }
    }
}