using NUnit.Framework;
using System;

namespace IbReal.Tests
{
    public class IbFloatTests
    {
        [Test]
        public void TestContructor()
        {
            IbFloat d0;
            string d0s;

            // default constructors

            d0 = new IbFloat(123456L, 4);
            Assert.True(d0.ToString() == "1234560000");
            d0 = new IbFloat(123456L, -4);
            Assert.True(d0.ToString() == "12.3456");

            d0 = new IbFloat(-123456L, 4);
            Assert.True(d0.ToString() == "-1234560000");
            d0 = new IbFloat(-123456L, -4);
            Assert.True(d0.ToString() == "-12.3456");

            // big significands

            d0 = new IbFloat(123456789012L, 4);
            d0s = d0.ToString();
            Assert.True(d0s == "1234567890000000"); // normalized
            d0 = new IbFloat(123456789012L, -4);
            d0s = d0.ToString();
            Assert.True(d0s == "12345678.9"); // normalized

            d0 = new IbFloat(-123456789012L, 4);
            d0s = d0.ToString();
            Assert.True(d0s == "-1234567890000000"); // normalized
            d0 = new IbFloat(-123456789012L, -4);
            d0s = d0.ToString();
            Assert.True(d0s == "-12345678.9"); // normalized

            // constructor overload

            d0 = new IbFloat(123456, 4);
            Assert.True(d0.ToString() == "1234560000");
            d0 = new IbFloat(123456U, 4);
            Assert.True(d0.ToString() == "1234560000");
        }

        [Test]
        public void TestPlus()
        {
            IbFloat d0, d1, d;

            d0 = new IbFloat(123456, 4);
            d1 = new IbFloat(654321, 4);
            d = d0 + d1;
            Assert.True(d.ToString() == "7777770000");

            d0 = new IbFloat(12345, 4);
            d1 = new IbFloat(54321, 2);
            d = d0 + d1;
            Assert.True(d.ToString() == "128882100");

            d0 = new IbFloat(12345, 4);
            d1 = new IbFloat(54321, -5);
            d = d0 + d1;
            Assert.True(d.ToString() == "123450000");

            d0 = new IbFloat(12345, 9);
            d1 = new IbFloat(54321, -5);
            d = d0 + d1;
            Assert.True(d.ToString() == "12345000000000");
        }

        [Test]
        public void TestMinus()
        {
            IbFloat d0, d1, d;

            d0 = new IbFloat(123456, 4);
            d1 = new IbFloat(654321, 4);
            d = d0 - d1;
            Assert.True(d.ToString() == "-5308650000");

            d0 = new IbFloat(12345, 4);
            d1 = new IbFloat(54321, 2);
            d = d0 - d1;
            Assert.True(d.ToString() == "118017900");

            d0 = new IbFloat(12345, 4);
            d1 = new IbFloat(54321, -5);
            d = d0 - d1;
            Assert.True(d.ToString() == "123450000");
        }

        [Test]
        public void TestMultiplication()
        {
            IbFloat d0, d1, d;

            d0 = new IbFloat(123456, 4);
            d1 = new IbFloat(654321, 4);
            d = d0 * d1;
            Assert.True(d.ToString() == "8077985330000000000");

            d0 = new IbFloat(12345, 4);
            d1 = new IbFloat(54321, 2);
            d = d0 * d1;
            Assert.True(d.ToString() == "670592745000000");

            d0 = new IbFloat(12345, 4);
            d1 = new IbFloat(54321, -5);
            d = d0 * d1;
            Assert.True(d.ToString() == "67059274.5");
        }

        [Test]
        public void TestDivision()
        {
            IbFloat d0, d1, d;

            d0 = new IbFloat(123456, 4);
            d1 = new IbFloat(654321, 4);
            d = d0 / d1;
            Assert.True(d.ToString() == "0.188678034");

            d0 = new IbFloat(12345, 4);
            d1 = new IbFloat(54321, 2);
            d = d0 / d1;
            Assert.True(d.ToString() == "22.7260175");

            d0 = new IbFloat(12345, 4);
            d1 = new IbFloat(54321, -5);
            d = d0 / d1;
            Assert.True(d.ToString() == "227260175");
        }

        [Test]
        public void TestToString()
        {
            IbFloat d;

            d = new IbFloat(123456, 4);
            Assert.True(d.ToString() == "1234560000");

            d = new IbFloat(123456, -4);
            Assert.True(d.ToString() == "12.3456");

            d = new IbFloat(123456, -8);
            Assert.True(d.ToString() == "0.00123456");
        }

        [Test]
        public void TestParse()
        {
            IbFloat d;

            d = IbFloat.Parse("123456000");
            Assert.True(d.ToString() == "123456000");

            d = IbFloat.Parse("123456789012"); // normalized
            Assert.True(d.ToString() == "123456789000");

            d = IbFloat.Parse("123.456");
            Assert.True(d.ToString() == "123.456");

            d = IbFloat.Parse("0.00123");
            Assert.True(d.ToString() == "0.00123");

            Assert.Throws(typeof(FormatException), () =>
            {
                var dd = IbFloat.Parse("AAA.456");
            });
        }

		[Test]
		public void TestOverflow()
		{
			IbFloat d0, d1;

			d0 = new IbFloat(123456, short.MaxValue);
			d1 = new IbFloat(654321, 10);
			Assert.Throws(typeof(OverflowException), () =>
			{
				var ds = d0 * d1;
			});

			d0 = new IbFloat(123456, short.MinValue);
			d1 = new IbFloat(654321, 10);
			Assert.Throws(typeof(OverflowException), () =>
			{
				var ds = d0 / d1;
			});
		}

		[Test]
		public void TestConvert()
		{
			IbFloat d0;

			byte vb;
			vb = (byte)new IbFloat(byte.MaxValue, 0);
			Assert.True(vb == byte.MaxValue);
			vb = (byte)new IbFloat(byte.MinValue, 0);
			Assert.True(vb == byte.MinValue);

			short vs;
			vs = (short)new IbFloat(short.MaxValue, 0);
			Assert.True(vs == short.MaxValue);
			vs = (short)new IbFloat(short.MinValue, 0);
			Assert.True(vs == short.MinValue);

			ushort vus;
			vus = (ushort)new IbFloat(ushort.MaxValue, 0);
			Assert.True(vus == ushort.MaxValue);
			vus = (ushort)new IbFloat(ushort.MinValue, 0);
			Assert.True(vus == ushort.MinValue);

			// IbFloat's sig support [10^9 - 1, -10^9 + 1]
			long vl;
			vl = (long)new IbFloat(1000000000 - 1, 0);
			Assert.True(vl == 1000000000 - 1);
			vl = (long)new IbFloat(-1000000000 + 1, 0);
			Assert.True(vl == -1000000000 + 1);

			// IbDouble's sig support [10^9 - 1, -10^9 + 1]
			ulong vul;
			vul = (ulong)new IbFloat(1000000000 - 1, 0);
			Assert.True(vul == 1000000000 - 1);
			vul = (ulong)new IbFloat(0, 0);
			Assert.True(vul == ulong.MinValue);

			Assert.Throws(typeof(OverflowException), () =>
			{
				d0 = new IbFloat(byte.MaxValue + 1, 0);
				var v = (byte)d0;
			});

			Assert.Throws(typeof(OverflowException), () =>
			{
				d0 = new IbFloat(byte.MinValue - 1, 0);
				var v = (byte)d0;
			});

			Assert.Throws(typeof(OverflowException), () =>
			{
				d0 = new IbFloat(char.MaxValue + 1, 0);
				var v = (char)d0;
			});

			Assert.Throws(typeof(OverflowException), () =>
			{
				d0 = new IbFloat(char.MinValue - 1, 0);
				var v = (char)d0;
			});

			// int.Max = 2147483647
			Assert.Throws(typeof(OverflowException), () =>
			{
				d0 = new IbFloat(214748365, 1);
				var v = (int)d0;
			});

			// int.Min = -2147483648
			Assert.Throws(typeof(OverflowException), () =>
			{
				d0 = new IbFloat(-214748365, 1);
				var v = (int)d0;
			});

			// uint.Max = 4294967295
			Assert.Throws(typeof(OverflowException), () =>
			{
				d0 = new IbFloat(429496730, 1);
				var v = (uint)d0;
			});

			// uint.Min = 0
			Assert.Throws(typeof(OverflowException), () =>
			{
				d0 = new IbFloat(-1, 0);
				var v = (uint)d0;
			});

			// long.Max = 9223372036854775807
			// test     = 9223372040000000000
			Assert.Throws(typeof(OverflowException), () =>
			{
				d0 = new IbFloat(922337204, 10);
				var v = (long)d0;
			});

			// long.Min = -9223372036854775808
			// test     = -9223372040000000000
			Assert.Throws(typeof(OverflowException), () =>
			{
				d0 = new IbFloat(-922337204, 10);
				var v = (long)d0;
			});
		}
	}
}
