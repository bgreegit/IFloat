using NUnit.Framework;
using System;

namespace IbDecimal.Tests
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
	}
}
