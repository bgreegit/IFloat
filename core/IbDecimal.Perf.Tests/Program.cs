using System;
using System.Diagnostics;

namespace IbReal.Perf.Tests
{
    class Program
    {
        static void Main(string[] args)
        {
			Console.WriteLine("<TestDouble>");
			TestDouble();
			Console.WriteLine(Environment.NewLine);

			Console.WriteLine("<TestFloat>");
			TestFloat();
			Console.WriteLine(Environment.NewLine);

			Console.WriteLine("<TestDouble>");
			TestDecimal();
			Console.WriteLine(Environment.NewLine);
		}

        static void TestFloat()
        {
            Stopwatch sw = new Stopwatch();

            int loopCount = 10000000;
            float rd0, rd1, rd;
            rd0 = 123.456f;
            rd1 = 654.321f;
            IbFloat id0, id1, id;
            id0 = new IbFloat(123456, -3);
            id1 = new IbFloat(654321, -3);

            // +

            sw.Start();
            {
                rd = 0;
                for (int i = 0; i < loopCount; ++i)
                {
                    rd += rd0 + rd1;
                }
            }
            sw.Stop();
            Console.WriteLine($"Float +   : {sw.Elapsed}");
            sw.Reset();

            sw.Start();
            {
                id = IbFloat.Zero;
                for (int i = 0; i < loopCount; ++i)
                {
                    id += id0 + id1;
                }
            }
            sw.Stop();
            Console.WriteLine($"IbFloat + : {sw.Elapsed}");
            sw.Reset();

            // *

            sw.Start();
            {
                rd = 0;
                for (int i = 0; i < loopCount; ++i)
                {
                    rd += rd0 * rd1;
                }
            }
            sw.Stop();
            Console.WriteLine($"Float *   : {sw.Elapsed}");
            sw.Reset();

            sw.Start();
            {
                id = IbFloat.Zero;
                for (int i = 0; i < loopCount; ++i)
                {
                    id += id0 * id1;
                }
            }
            sw.Stop();
            Console.WriteLine($"IbFloat * : {sw.Elapsed}");
            sw.Reset();

            // /

            sw.Start();
            {
                rd = 0;
                for (int i = 0; i < loopCount; ++i)
                {
                    rd += rd0 / rd1;
                }
            }
            sw.Stop();
            Console.WriteLine($"Float /   : {sw.Elapsed}");
            sw.Reset();

            sw.Start();
            {
                id = IbFloat.Zero;
                for (int i = 0; i < loopCount; ++i)
                {
                    id += id0 / id1;
                }
            }
            sw.Stop();
            Console.WriteLine($"IbFloat / : {sw.Elapsed}");
            sw.Reset();
        }

        static void TestDouble()
        {
            Stopwatch sw = new Stopwatch();

            int loopCount = 10000000;
            double rd0, rd1, rd;
            rd0 = 123.456;
            rd1 = 654.321;
            IbDouble id0, id1, id;
            id0 = new IbDouble(123456, -3);
            id1 = new IbDouble(654321, -3);

            // +

            sw.Start();
            {
                rd = 0;
                for (int i = 0; i < loopCount; ++i)
                {
                    rd += rd0 + rd1;
                }
            }
            sw.Stop();
            Console.WriteLine($"Double +   : {sw.Elapsed}");
            sw.Reset();

            sw.Start();
            {
                id = IbDouble.Zero;
                for (int i = 0; i < loopCount; ++i)
                {
                    id += id0 + id1;
                }
            }
            sw.Stop();
            Console.WriteLine($"IbDouble + : {sw.Elapsed}");
            sw.Reset();

            // *

            sw.Start();
            {
                rd = 0;
                for (int i = 0; i < loopCount; ++i)
                {
                    rd += rd0 * rd1;
                }
            }
            sw.Stop();
            Console.WriteLine($"Double *   : {sw.Elapsed}");
            sw.Reset();

            sw.Start();
            {
                id = IbDouble.Zero;
                for (int i = 0; i < loopCount; ++i)
                {
                    id += id0 * id1;
                }
            }
            sw.Stop();
            Console.WriteLine($"IbDouble * : {sw.Elapsed}");
            sw.Reset();

            // /

            sw.Start();
            {
                rd = 0;
                for (int i = 0; i < loopCount; ++i)
                {
                    rd += rd0 / rd1;
                }
            }
            sw.Stop();
            Console.WriteLine($"Double /   : {sw.Elapsed}");
            sw.Reset();

            sw.Start();
            {
                id = IbDouble.Zero;
                for (int i = 0; i < loopCount; ++i)
                {
                    id += id0 / id1;
                }
            }
            sw.Stop();
            Console.WriteLine($"IbDouble / : {sw.Elapsed}");
            sw.Reset();
        }

		static void TestDecimal()
        {
            Stopwatch sw = new Stopwatch();

            int loopCount = 10000000;
            decimal rd0, rd1, rd;
            rd0 = 123.456m;
            rd1 = 654.321m;
            IbFloat id0, id1, id;
            id0 = new IbFloat(123456, -3);
            id1 = new IbFloat(654321, -3);

            // +

            sw.Start();
            {
                rd = 0;
                for (int i = 0; i < loopCount; ++i)
                {
                    rd += rd0 + rd1;
                }
            }
            sw.Stop();
            Console.WriteLine($"decimal + : {sw.Elapsed}");
            sw.Reset();

            sw.Start();
            {
                id = IbFloat.Zero;
                for (int i = 0; i < loopCount; ++i)
                {
                    id += id0 + id1;
                }
            }
            sw.Stop();
            Console.WriteLine($"IbFloat + : {sw.Elapsed}");
            sw.Reset();

            // *

            sw.Start();
            {
                rd = 0;
                for (int i = 0; i < loopCount; ++i)
                {
                    rd += rd0 * rd1;
                }
            }
            sw.Stop();
            Console.WriteLine($"decimal * : {sw.Elapsed}");
            sw.Reset();

            sw.Start();
            {
                id = IbFloat.Zero;
                for (int i = 0; i < loopCount; ++i)
                {
                    id += id0 * id1;
                }
            }
            sw.Stop();
            Console.WriteLine($"IbFloat * : {sw.Elapsed}");
            sw.Reset();

            // /

            sw.Start();
            {
                rd = 0;
                for (int i = 0; i < loopCount; ++i)
                {
                    rd += rd0 / rd1;
                }
            }
            sw.Stop();
            Console.WriteLine($"decimal / : {sw.Elapsed}");
            sw.Reset();

            sw.Start();
            {
                id = IbFloat.Zero;
                for (int i = 0; i < loopCount; ++i)
                {
                    id += id0 / id1;
                }
            }
            sw.Stop();
            Console.WriteLine($"IbFloat / : {sw.Elapsed}");
            sw.Reset();
        }
    }
}
