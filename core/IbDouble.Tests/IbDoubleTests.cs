using NUnit.Framework;
using System;
using System.Numerics;

namespace IbDouble.Tests
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
    }
}
