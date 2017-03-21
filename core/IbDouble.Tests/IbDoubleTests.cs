using NUnit.Framework;
using System;
using System.Numerics;

namespace IbReal.Tests
{
    public class IbDoubleTests : AssertionHelper
    {
        [Test]
        public void TestContructor()
        {
            IbDouble d0;
            string d0s;

            // default constructors

            d0 = new IbDouble(new BigInteger(123456), 4);
            Assert.True(d0.ToString() == "1234560000");
            d0 = new IbDouble(new BigInteger(123456), -4);
            Assert.True(d0.ToString() == "12.3456");

            d0 = new IbDouble(new BigInteger(-123456), 4);
            Assert.True(d0.ToString() == "-1234560000");
            d0 = new IbDouble(new BigInteger(-123456), -4);
            Assert.True(d0.ToString() == "-12.3456");

            // big significands

            d0 = new IbDouble(new BigInteger(1234567890123456789012m), 4);
            d0s = d0.ToString();
            Assert.True(d0s == "12345678901234567800000000"); // normalized
            d0 = new IbDouble(new BigInteger(1234567890123456789012m), -4);
            d0s = d0.ToString();
            Assert.True(d0s == "123456789012345678"); // normalized

            d0 = new IbDouble(new BigInteger(-1234567890123456789012m), 4);
            d0s = d0.ToString();
            Assert.True(d0s == "-12345678901234567800000000"); // normalized
            d0 = new IbDouble(new BigInteger(-1234567890123456789012m), -4);
            d0s = d0.ToString();
            Assert.True(d0s == "-123456789012345678"); // normalized

            // constructor overload

            d0 = new IbDouble(123456UL, 4);
            Assert.True(d0.ToString() == "1234560000");
            d0 = new IbDouble(123456L, 4);
            Assert.True(d0.ToString() == "1234560000");
            d0 = new IbDouble(123456, 4);
            Assert.True(d0.ToString() == "1234560000");
            d0 = new IbDouble(123456U, 4);
            Assert.True(d0.ToString() == "1234560000");
        }

        [Test]
        public void TestPlus()
        {
            IbDouble d0, d1, d;

            d0 = new IbDouble(123456, 4);
            d1 = new IbDouble(654321, 4);
            d = d0 + d1;
            Assert.True(d.ToString() == "7777770000");

            d0 = new IbDouble(12345, 4);
            d1 = new IbDouble(54321, 2);
            d = d0 + d1;
            Assert.True(d.ToString() == "128882100");

            d0 = new IbDouble(12345, 4);
            d1 = new IbDouble(54321, -5);
            d = d0 + d1;
            Assert.True(d.ToString() == "123450000.54321");

            d0 = new IbDouble(12345, 18);
            d1 = new IbDouble(54321, -5);
            d = d0 + d1;
            Assert.True(d.ToString() == "12345000000000000000000");
        }

        [Test]
        public void TestMinus()
        {
            IbDouble d0, d1, d;

            d0 = new IbDouble(123456, 4);
            d1 = new IbDouble(654321, 4);
            d = d0 - d1;
            Assert.True(d.ToString() == "-5308650000");

            d0 = new IbDouble(12345, 4);
            d1 = new IbDouble(54321, 2);
            d = d0 - d1;
            Assert.True(d.ToString() == "118017900");

            d0 = new IbDouble(12345, 4);
            d1 = new IbDouble(54321, -5);
            d = d0 - d1;
            Assert.True(d.ToString() == "123449999.45679");
        }

        [Test]
        public void TestMultiplication()
        {
            IbDouble d0, d1, d;

            d0 = new IbDouble(123456, 4);
            d1 = new IbDouble(654321, 4);
            d = d0 * d1;
            Assert.True(d.ToString() == "8077985337600000000");

            d0 = new IbDouble(12345, 4);
            d1 = new IbDouble(54321, 2);
            d = d0 * d1;
            Assert.True(d.ToString() == "670592745000000");

            d0 = new IbDouble(12345, 4);
            d1 = new IbDouble(54321, -5);
            d = d0 * d1;
            Assert.True(d.ToString() == "67059274.5");
        }

        [Test]
        public void TestDivision()
        {
            IbDouble d0, d1, d;

            d0 = new IbDouble(123456, 4);
            d1 = new IbDouble(654321, 4);
            d = d0 / d1;
            Assert.True(d.ToString() == "0.188678034175886147");

            d0 = new IbDouble(12345, 4);
            d1 = new IbDouble(54321, 2);
            d = d0 / d1;
            Assert.True(d.ToString() == "22.7260175622687358");

            d0 = new IbDouble(12345, 4);
            d1 = new IbDouble(54321, -5);
            d = d0 / d1;
            Assert.True(d.ToString() == "227260175.622687358");
        }

        [Test]
        public void TestToString()
        {
            IbDouble d;

            d = new IbDouble(123456, 4);
            Assert.True(d.ToString() == "1234560000");

            d = new IbDouble(123456, -4);
            Assert.True(d.ToString() == "12.3456");

            d = new IbDouble(123456, -8);
            Assert.True(d.ToString() == "0.00123456");
        }

        [Test]
        public void TestParse()
        {
            IbDouble d;

            d = IbDouble.Parse("123456000");
            Assert.True(d.ToString() == "123456000");

            d = IbDouble.Parse("123456789012345678901234"); // normalized
            Assert.True(d.ToString() == "123456789012345678000000");

            d = IbDouble.Parse("123.456");
            Assert.True(d.ToString() == "123.456");

            d = IbDouble.Parse("0.00123");
            Assert.True(d.ToString() == "0.00123");

            Assert.Throws(typeof(FormatException), () =>
            {
                var dd = IbDouble.Parse("AAA.456");
            });
        }

		[Test]
		public void TestOverflow()
		{
			IbDouble d0, d1;

			d0 = new IbDouble(123456, short.MaxValue);
			d1 = new IbDouble(654321, 10);
			Assert.Throws(typeof(OverflowException), () =>
			{
				var ds = d0 * d1;
			});

			d0 = new IbDouble(123456, short.MinValue);
			d1 = new IbDouble(654321, 10);
			Assert.Throws(typeof(OverflowException), () =>
			{
				var ds = d0 / d1;
			});
		}

		[Test]
		public void TestConvert_Integer()
		{
			IbDouble d0;

			byte vb;
			vb = (byte)new IbDouble(byte.MaxValue, 0);
			Assert.True(vb == byte.MaxValue);
			vb = (byte)new IbDouble(byte.MinValue, 0);
			Assert.True(vb == byte.MinValue);

			int vi;
			vi = (int)new IbDouble(int.MaxValue, 0);
			Assert.True(vi == int.MaxValue);
			vi = (int)new IbDouble(int.MinValue, 0);
			Assert.True(vi == int.MinValue);

			uint vui;
			vui = (uint)new IbDouble(uint.MaxValue, 0);
			Assert.True(vui == uint.MaxValue);
			vui = (uint)new IbDouble(uint.MinValue, 0);
			Assert.True(vui == uint.MinValue);

			// IbDouble's sig support [10^18 - 1, -10^18 + 1]
			long vl;
			vl = (long)new IbDouble(1000000000000000000L - 1, 0);
			Assert.True(vl == 1000000000000000000L - 1);
			vl = (long)new IbDouble(-1000000000000000000L + 1, 0);
			Assert.True(vl == -1000000000000000000L + 1);

			// IbDouble's sig support [10^18 - 1, -10^18 + 1]
			ulong vul;
			vul = (ulong)new IbDouble(1000000000000000000UL - 1, 0);
			Assert.True(vul == 1000000000000000000UL - 1);
			vul = (ulong)new IbDouble(ulong.MinValue, 0);
			Assert.True(vul == ulong.MinValue);

			Assert.Throws(typeof(OverflowException), () =>
			{
				d0 = new IbDouble(byte.MaxValue + 1, 0);
				var v = (byte)d0;
			});

			Assert.Throws(typeof(OverflowException), () =>
			{
				d0 = new IbDouble(byte.MinValue - 1, 0);
				var v = (byte)d0;
			});

			Assert.Throws(typeof(OverflowException), () =>
			{
				d0 = new IbDouble(char.MaxValue + 1, 0);
				var v = (char)d0;
			});

			Assert.Throws(typeof(OverflowException), () =>
			{
				d0 = new IbDouble(char.MinValue - 1, 0);
				var v = (char)d0;
			});

			Assert.Throws(typeof(OverflowException), () =>
			{
				d0 = new IbDouble((long)int.MaxValue + 1, 0);
				var v = (int)d0;
			});

			Assert.Throws(typeof(OverflowException), () =>
			{
				d0 = new IbDouble((long)int.MinValue - 1, 0);
				var v = (int)d0;
			});

			Assert.Throws(typeof(OverflowException), () =>
			{
				d0 = new IbDouble((long)uint.MaxValue + 1, 0);
				var v = (uint)d0;
			});

			Assert.Throws(typeof(OverflowException), () =>
			{
				d0 = new IbDouble((long)uint.MinValue - 1, 0);
				var v = (uint)d0;
			});

			// long.Max = 9223372036854775807
			Assert.Throws(typeof(OverflowException), () =>
			{
				d0 = new IbDouble(922337203685477581L, 1);
				var v = (long)d0;
			});

			// long.min = -9223372036854775808
			Assert.Throws(typeof(OverflowException), () =>
			{
				d0 = new IbDouble(-922337203685477581L, 1);
				var v = (uint)d0;
			});

			// long.Max = 9223372036854775807
			Assert.Throws(typeof(OverflowException), () =>
			{
				d0 = new IbDouble(922337203685477581L, 1);
				var v = (long)d0;
			});

			// long.min = -9223372036854775808
			Assert.Throws(typeof(OverflowException), () =>
			{
				d0 = new IbDouble(-922337203685477581L, 1);
				var v = (uint)d0;
			});
		}

		[Test]
		public void TestConvert_FloatDouble()
		{
			float vf;
			vf = (float)new IbDouble(123456, 3);
			Assert.True(vf == 123456000f);
			vf = (float)new IbDouble(123456, -3);
			Assert.True(vf == 123.456f);

			double vd;
			vd = (double)new IbDouble(123456, 3);
			Assert.True(Math.Abs(vd - 123456000) / 123456000 < 0.000001);
			vd = (double)new IbDouble(123456, -3);
			Assert.True(Math.Abs(vd - 123.456) / 123.456 < 0.000001);
		}
	}
}
