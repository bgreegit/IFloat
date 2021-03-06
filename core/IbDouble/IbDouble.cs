﻿using System;
using System.Numerics;

namespace IbReal
{
    public struct IbDouble
    {
        private BigInteger _sig;
        private short _exp;

        private const int maxPrecision = 18;

        // s_maxSig = 10 ^ 18
        private static BigInteger s_maxSig = new BigInteger(1000000000000000000);

        private static long[] s_exp2sig = new long[maxPrecision + 1]
        {
            1,                   // 10 ^ 0
            10,                  // 10 ^ 1
            100,                 // 10 ^ 2
            1000,                // 10 ^ 3
            10000,               // 10 ^ 4
            100000,              // 10 ^ 5
            1000000,             // 10 ^ 6
            10000000,            // 10 ^ 7
            100000000,           // 10 ^ 8
            1000000000,          // 10 ^ 9
            10000000000,         // 10 ^ 10
            100000000000,        // 10 ^ 11
            1000000000000,       // 10 ^ 12
            10000000000000,      // 10 ^ 13
            100000000000000,     // 10 ^ 14
            1000000000000000,    // 10 ^ 15
            10000000000000000,   // 10 ^ 16
            100000000000000000,  // 10 ^ 17
            1000000000000000000, // 10 ^ 18
        };

        public IbDouble(BigInteger sig, short exp)
        {
            _sig = sig;
            _exp = exp;
            NormalizeSelf();
        }

        private IbDouble(BigInteger sig, short exp, bool needNomalize = true)
        {
            _sig = sig;
            _exp = exp;
            if (needNomalize)
                NormalizeSelf();
        }

        public IbDouble(long sig, short exp)
        {
            _sig = sig;
            _exp = exp;
        }

        public IbDouble(long v)
        {
            _sig = v;
            _exp = 0;
            NormalizeSelf();
        }

        public IbDouble(ulong v, short exp)
        {
            _sig = v;
            _exp = exp;
            NormalizeSelf();
        }

        public IbDouble(ulong v)
        {
            _sig = v;
            _exp = 0;
            NormalizeSelf();
        }

        public IbDouble(int v, short exp)
        {
            _sig = v;
            _exp = exp;
            NormalizeSelf();
        }

        public IbDouble(int v)
        {
            _sig = v;
            _exp = 0;
            NormalizeSelf();
        }

        public IbDouble(uint v, short exp)
        {
            _sig = v;
            _exp = exp;
            NormalizeSelf();
        }

        public IbDouble(uint v)
        {
            _sig = v;
            _exp = 0;
            NormalizeSelf();
        }

		public IbDouble(float v)
			: this((double)v)
		{
		}

		public IbDouble(double v)
		{
			if (v > 0)
			{
				short exp = 0;
				while (v < 100000000000000000 && Math.Truncate(v) != v) // 100000000000000000 = 10^17
				{
					v *= 10;
					exp += 1;
				}

				_sig = (long)Math.Truncate(v);
				_exp = exp;
			}
			else if (v < 0)
			{
				v = -v;
				short exp = 0;
				while (v < 100000000000000000 && Math.Truncate(v) != v) // 100000000000000000 = until 10^17
				{
					v *= 10;
					exp += 1;
				}

				_sig = -(long)Math.Truncate(v);
				_exp = exp;
			}
			else
			{
				_sig = 0;
				_exp = 0;
			}

			NormalizeSelf();
		}

		private IbDouble Normalize()
        {
            var sig = _sig;
            var exp = _exp;
            bool isNegative = false;
            if (sig < 0)
            {
                sig = -sig;
                isNegative = true;
            }

            // reduce too big significand and remove following zeros
            while (sig >= s_maxSig || sig != 0 && sig % 10 == 0)
            {
                sig /= 10;
                exp += 1;
            }
            return new IbDouble(isNegative ? -sig : sig, exp, false);
        }

        private void NormalizeSelf()
        {
            bool isNegative = false;
            if (_sig < 0)
            {
                _sig = -_sig;
                isNegative = true;
            }

            // reduce too big significand and remove following zeros
            while (_sig >= s_maxSig || _sig != 0 && _sig % 10 == 0)
            {
                _sig /= 10;
				checked { _exp += 1; }
            }
            if (isNegative)
                _sig = -_sig;
        }

        private IbDouble Denomalize(short exp)
        {
            if (exp > 0)
                throw new ArgumentException("Denomalize should be negative", nameof(exp));
            if (exp < -maxPrecision)
                throw new ArgumentException("Denomalize exp overflow", nameof(exp));
            return new IbDouble(_sig * s_exp2sig[-exp], checked((short)(_exp + exp)), false);
        }

        private void DenomalizeSelf(short exp)
        {
            if (exp > 0)
                throw new ArgumentException("DenomalizeSelf should be negative", nameof(exp));
            if (exp < -maxPrecision)
                throw new ArgumentException("DenomalizeSelf exp overflow", nameof(exp));
            _sig *= s_exp2sig[-exp];
			checked { _exp += exp; }
        }

        public override bool Equals(object obj)
        {
            if (!(obj is IbDouble))
                return false;
            return Equals((IbDouble)obj);
        }

        public override int GetHashCode()
        {
            return CombineHash(_sig.GetHashCode(), _exp);
        }

        // from NumericHelpers of .net core
        private static uint CombineHash(uint u1, uint u2)
        {
            return ((u1 << 7) | (u1 >> 25)) ^ u2;
        }

        // from NumericHelpers of .net core
        private static int CombineHash(int n1, int n2)
        {
            return (int)CombineHash((uint)n1, (uint)n2);
        }

        public override string ToString()
        {
            string ss = _sig.ToString();
            if (_exp == 0)
            {
                return ss;
            }
            else if (_exp > 0)
            {
                return ss + new string('0', _exp);
            }
            else
            {
                if (ss.Length > -_exp)
                {
                    // XXX.YYYY format
                    return ss.Substring(0, ss.Length + _exp) + "." + ss.Substring(ss.Length + _exp);
                }
                else
                {
                    // 0.00XXX format
                    return "0." + new string('0', -_exp - ss.Length) + ss;
                }
            }
        }

        public static IbDouble Parse(string value)
        {
            int dotIndex = value.IndexOf('.');
            if (dotIndex < 0)
            {
                return (new IbDouble(BigInteger.Parse(value), 0)).Normalize();
            }
            else
            {
                var exp = -value.Length + dotIndex + 1;
                value = value.Substring(0, dotIndex) + value.Substring(dotIndex + 1);
                return (new IbDouble(BigInteger.Parse(value), checked((short)exp))).Normalize();
            }
        }

        public static bool TryParse(string value, out IbDouble result)
        {
            int dotIndex = value.IndexOf('.');
            if (dotIndex < 0)
            {
                BigInteger sig;
                if (BigInteger.TryParse(value, out sig))
                {
                    result = (new IbDouble(sig, 0)).Normalize();
                    return true;
                }
                else
                {
                    result = Zero;
                    return false;
                }
            }
            else
            {
                var exp = value.Length - dotIndex - 1;
                value = value.Substring(0, dotIndex) + value.Substring(dotIndex + 1);
                BigInteger sig;
                if (BigInteger.TryParse(value, out sig))
                {
                    result = (new IbDouble(sig, checked((short)exp))).Normalize();
                    return true;
                }
                else
                {
                    result = Zero;
                    return false;
                }
            }
        }

        public static IbDouble operator +(IbDouble v)
        {
            return v;
        }

        public static IbDouble operator -(IbDouble v)
        {
            return new IbDouble(-v._sig, v._exp);
        }

        public static IbDouble operator +(IbDouble v1, IbDouble v2)
        {
            IbDouble v1d;
            IbDouble v2d;
            var ed = v1._exp - v2._exp;
            if (ed >= maxPrecision)
            {
                return v1;
            }
            else if (ed <= -maxPrecision)
            {
                return v2;
            }
            else if (ed > 0)
            {
                v1d = v1.Denomalize((short)-ed);
                v2d = v2;
            }
            else if (ed < 0)
            {
                v1d = v1;
                v2d = v2.Denomalize((short)ed);
            }
            else
            {
                v1d = v1;
                v2d = v2;
            }

            return (new IbDouble(v1d._sig + v2d._sig, v1d._exp)).Normalize();
        }

        public static IbDouble operator -(IbDouble v1, IbDouble v2)
        {
            return v1 + (-v2);
        }

        public static IbDouble operator *(IbDouble v1, IbDouble v2)
        {
			var sig = v1._sig* v2._sig;
			var exp = checked((short)(v1._exp + v2._exp));
            return (new IbDouble(sig, exp)).Normalize();
        }

        public static IbDouble operator /(IbDouble v1, IbDouble v2)
        {
            // first, denomalize temprary
            var v1d = v1.Denomalize(-maxPrecision);
			// do integer division and normalize
			var sig = v1d._sig / v2._sig;
			var exp = checked((short)(v1d._exp - v2._exp));
			return (new IbDouble(sig, exp)).Normalize();
        }

        public static bool operator ==(IbDouble left, IbDouble right) { return left._sig == right._sig && left._exp == right._exp; }
        public static bool operator ==(IbDouble left, long right) { return left == new IbDouble(right); }
        public static bool operator ==(long left, IbDouble right) { return new IbDouble(left) == right; }
        public static bool operator ==(IbDouble left, ulong right) { return left == new IbDouble(right); }
        public static bool operator ==(ulong left, IbDouble right) { return new IbDouble(left) == right; }
        public static bool operator !=(IbDouble left, IbDouble right) { return left._sig != right._sig || left._exp != right._exp; }
        public static bool operator !=(IbDouble left, long right) { return left != new IbDouble(right); }
        public static bool operator !=(long left, IbDouble right) { return new IbDouble(left) == right; }
        public static bool operator !=(IbDouble left, ulong right) { return left != new IbDouble(right); }
        public static bool operator !=(ulong left, IbDouble right) { return new IbDouble(left) == right; }

        public static bool operator <(IbDouble left, IbDouble right)
        {
            var ed = left._exp - right._exp;
            if (ed >= maxPrecision)
            {
                return false;
            }
            else if (ed <= -maxPrecision)
            {
                return true;
            }
            else if (ed > 0)
            {
                var ld = left.Denomalize((short)-ed);
                return ld._sig < right._sig;
            }
            else if (ed < 0)
            {
                var rd = right.Denomalize((short)ed);
                return left._sig < rd._sig;
            }
            else
            {
                return left._sig < right._sig;
            }
        }
        public static bool operator <(IbDouble left, ulong right) { return left < new IbDouble(right); }
        public static bool operator <(ulong left, IbDouble right) { return new IbDouble(left) < right; }
        public static bool operator <(IbDouble left, long right) { return left < new IbDouble(right); }
        public static bool operator <(long left, IbDouble right) { return new IbDouble(left) < right; }

        public static bool operator >(IbDouble left, IbDouble right)
        {
            var ed = left._exp - right._exp;
            if (ed >= maxPrecision)
            {
                return true;
            }
            else if (ed <= -maxPrecision)
            {
                return false;
            }
            else if (ed > 0)
            {
                var ld = left.Denomalize((short)-ed);
                return ld._sig > right._sig;
            }
            else if (ed < 0)
            {
                var rd = right.Denomalize((short)ed);
                return left._sig > rd._sig;
            }
            else
            {
                return left._sig > right._sig;
            }
        }
        public static bool operator >(IbDouble left, long right) { return left > new IbDouble(right); }
        public static bool operator >(long left, IbDouble right) { return new IbDouble(left) > right; }
        public static bool operator >(IbDouble left, ulong right) { return left > new IbDouble(right); }
        public static bool operator >(ulong left, IbDouble right) { return new IbDouble(left) > right; }

        public static bool operator <=(IbDouble left, IbDouble right)
        {
            var ed = left._exp - right._exp;
            if (ed >= maxPrecision)
            {
                return false;
            }
            else if (ed <= -maxPrecision)
            {
                return true;
            }
            else if (ed > 0)
            {
                var ld = left.Denomalize((short)-ed);
                return ld._sig <= right._sig;
            }
            else if (ed < 0)
            {
                var rd = right.Denomalize((short)ed);
                return left._sig <= rd._sig;
            }
            else
            {
                return left._sig <= right._sig;
            }
        }

        public static bool operator <=(IbDouble left, long right) { return left <= new IbDouble(right); }
        public static bool operator <=(ulong left, IbDouble right) { return new IbDouble(left) <= right; }
        public static bool operator <=(IbDouble left, ulong right) { return left <= new IbDouble(right); }
        public static bool operator <=(long left, IbDouble right) { return new IbDouble(left) <= right; }

        public static bool operator >=(IbDouble left, IbDouble right)
        {
            var ed = left._exp - right._exp;
            if (ed >= maxPrecision)
            {
                return true;
            }
            else if (ed <= -maxPrecision)
            {
                return false;
            }
            else if (ed > 0)
            {
                var ld = left.Denomalize((short)-ed);
                return ld._sig >= right._sig;
            }
            else if (ed < 0)
            {
                var rd = right.Denomalize((short)ed);
                return left._sig >= rd._sig;
            }
            else
            {
                return left._sig >= right._sig;
            }
        }
        public static bool operator >=(IbDouble left, ulong right) { return left >= new IbDouble(right); }
        public static bool operator >=(long left, IbDouble right) { return new IbDouble(left) >= right; }
        public static bool operator >=(IbDouble left, long right) { return left >= new IbDouble(right); }
        public static bool operator >=(ulong left, IbDouble right) { return new IbDouble(left) >= right; }

        public static implicit operator IbDouble(ushort value) { return new IbDouble(value); }
        public static implicit operator IbDouble(short value) { return new IbDouble(value); }
        public static implicit operator IbDouble(sbyte value) { return new IbDouble(value); }
        public static implicit operator IbDouble(ulong value) { return new IbDouble(value); }
        public static implicit operator IbDouble(byte value) { return new IbDouble(value); }
        public static implicit operator IbDouble(long value) { return new IbDouble(value); }
        public static implicit operator IbDouble(uint value) { return new IbDouble(value); }
        public static implicit operator IbDouble(int value) { return new IbDouble(value); }
		public static implicit operator IbDouble(float value) { return new IbDouble(value); }
		public static implicit operator IbDouble(double value) { return new IbDouble(value); }

		public static IbDouble Zero { get { return new IbDouble(0, 0); } }
        public static IbDouble One { get { return new IbDouble(1, 0); } }
        public static IbDouble MinusOne { get { return new IbDouble(-1, 0); } }
        public bool IsZero { get { return _sig == 0 && _exp == 0; } }
        public bool IsOne { get { return _sig == 1 && _exp == 0; } }
        public bool IsMinusOne { get { return _sig == -1 && _exp == 0; } }

        public static IbDouble Abs(IbDouble value) { return new IbDouble(value._sig < 0 ? value._sig : value._sig, value._exp); }
        public static IbDouble Negate(IbDouble value) { return -value; }
        public static IbDouble Add(IbDouble left, IbDouble right) { return left + right; }
        public static IbDouble Subtract(IbDouble left, IbDouble right) { return left - right; }
        public static IbDouble Multiply(IbDouble left, IbDouble right) { return left * right; }
        public static IbDouble Divide(IbDouble dividend, IbDouble divisor) { return dividend / divisor; }

        public static IbDouble Max(IbDouble left, IbDouble right) { return left > right ? left : right; }
        public static IbDouble Min(IbDouble left, IbDouble right) { return left < right ? left : right; }
        public static int Compare(IbDouble left, IbDouble right) { return left < right ? -1 : (left > right ? 1 : 0); }
        public int CompareTo(long other) { return Compare(this, other); }
        public int CompareTo(ulong other) { return Compare(this, other); }
        public bool Equals(long other) { return this == other; }
        public bool Equals(ulong other) { return this == other; }

		public static explicit operator byte(IbDouble value)
		{
			return checked((byte)((long)value));
		}

		public static explicit operator sbyte(IbDouble value)
		{
			return checked((sbyte)((long)value));
		}

		public static explicit operator short(IbDouble value)
		{
			return checked((short)((long)value));
		}

		public static explicit operator ushort(IbDouble value)
		{
			return checked((ushort)((long)value));
		}

		public static explicit operator int(IbDouble value)
		{
			return checked((int)((long)value));
		}

		public static explicit operator uint(IbDouble value)
		{
			return checked((uint)((long)value));
		}

		public static explicit operator long(IbDouble value)
		{
			// value._sig is nomalized. So do not check overflow
			long v = (long)value._sig;

			if (value._exp >= 19)
			{
				throw new OverflowException("Value was either too large or too small for an Int64.");
			}
			else if (value._exp >= 0)
			{
				// multiply exp with checked statement
				for (int i = 0; i < value._exp; ++i)
				{
					checked { v *= 10; }
				}
				return v;
			}
			else
			{
				for (int i = 0; i < -value._exp && v != 0; ++i)
				{
					v /= 10;
				}
				return v;
			}
		}

		public static explicit operator ulong(IbDouble value)
		{
			if (value._sig < 0)
			{
				throw new OverflowException("Value was either too small for an UInt64.");
			}

			ulong v = (ulong)value._sig;

			if (value._exp >= 19)
			{
				throw new OverflowException("Value was either too large or too small for an UInt64.");
			}
			else if (value._exp >= 0)
			{
				// multiply exp with checked statement
				for (int i = 0; i < value._exp; ++i)
				{
					checked { v *= 10; }
				}
				return v;
			}
			else
			{
				for (int i = 0; i < -value._exp && v > 0; ++i)
				{
					v /= 10;
				}
				return v;
			}
		}

		public static explicit operator float(IbDouble value)
		{
			return (float)((double)value);
		}

		public static explicit operator double(IbDouble value)
		{
			var v = (double)value._sig;

			// multiply exp with checked statement
			if (value._exp >= 0)
			{
				for (int i = 0; i < value._exp; ++i)
				{
					checked { v *= 10; }
				}
				return v;
			}
			else
			{
				for (int i = 0; i < -value._exp; ++i)
				{
					checked { v *= 0.1; }
				}
				return v;
			}
		}
	}
}
