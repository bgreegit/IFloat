using System;

namespace IbDecimal
{
    public struct IbFloat
    {
        private long _sig;
        private short _exp;

        private const int maxPrecision = 9;

        // s_maxSig = 10 ^ 9
        private static long s_maxSig = 1000000000L;

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
        };

        public IbFloat(long sig, short exp)
        {
            _sig = sig;
            _exp = exp;
            NormalizeSelf();
        }

        private IbFloat(long sig, short exp, bool needNomalize = true)
        {
            _sig = sig;
            _exp = exp;
            if (needNomalize)
                NormalizeSelf();
        }

        public IbFloat(long sig)
        {
            _sig = sig;
            _exp = 0;
            NormalizeSelf();
        }

        public IbFloat(int v, short exp)
        {
            _sig = v;
            _exp = exp;
            NormalizeSelf();
        }

        public IbFloat(int v)
        {
            _sig = v;
            _exp = 0;
            NormalizeSelf();
        }

        public IbFloat(uint v, short exp)
        {
            _sig = v;
            _exp = exp;
            NormalizeSelf();
        }

        public IbFloat(uint v)
        {
            _sig = v;
            _exp = 0;
            NormalizeSelf();
        }

        private IbFloat Normalize()
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
            return new IbFloat(isNegative ? -sig : sig, exp, false);
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
				checked { _exp += 1; };
            }
            if (isNegative)
                _sig = -_sig;
        }

        private IbFloat Denomalize(short exp)
        {
            if (exp > 0)
                throw new ArgumentException("Denomalize should be negative", nameof(exp));
            if (exp < -maxPrecision)
                throw new ArgumentException("Denomalize exp overflow", nameof(exp));
            return new IbFloat(_sig * s_exp2sig[-exp], checked((short)(_exp + exp)), false);
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
            if (!(obj is IbFloat))
                return false;
            return Equals((IbFloat)obj);
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

        public static IbFloat Parse(string value)
        {
            int dotIndex = value.IndexOf('.');
            if (dotIndex < 0)
            {
                return (new IbFloat(long.Parse(value), 0)).Normalize();
            }
            else
            {
                var exp = -value.Length + dotIndex + 1;
                value = value.Substring(0, dotIndex) + value.Substring(dotIndex + 1);
				return (new IbFloat(long.Parse(value), checked((short)exp))).Normalize();
            }
        }

        public static bool TryParse(string value, out IbFloat result)
        {
            int dotIndex = value.IndexOf('.');
            if (dotIndex < 0)
            {
                long sig;
                if (long.TryParse(value, out sig))
                {
                    result = (new IbFloat(sig, 0)).Normalize();
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
                long sig;
                if (long.TryParse(value, out sig))
                {
                    result = (new IbFloat(sig, checked((short)exp))).Normalize();
                    return true;
                }
                else
                {
                    result = Zero;
                    return false;
                }
            }
        }

        public static IbFloat operator +(IbFloat v)
        {
            return v;
        }

        public static IbFloat operator -(IbFloat v)
        {
            return new IbFloat(-v._sig, v._exp);
        }

        public static IbFloat operator +(IbFloat v1, IbFloat v2)
        {
            IbFloat v1d;
            IbFloat v2d;
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

            return (new IbFloat(v1d._sig + v2d._sig, v1d._exp)).Normalize();
        }

        public static IbFloat operator -(IbFloat v1, IbFloat v2)
        {
            return v1 + (-v2);
        }

        public static IbFloat operator *(IbFloat v1, IbFloat v2)
        {
			var sig = v1._sig * v2._sig;
			var exp = checked((short)(v1._exp + v2._exp));
            return (new IbFloat(sig, exp)).Normalize();
        }

        public static IbFloat operator /(IbFloat v1, IbFloat v2)
        {
            // first, denomalize temprary
            var v1d = v1.Denomalize(-maxPrecision);
			// do integer division and normalize
			var sig = v1d._sig / v2._sig;
			var exp = checked((short)(v1d._exp - v2._exp));
            return (new IbFloat(sig, exp)).Normalize();
        }

        public static bool operator ==(IbFloat left, IbFloat right) { return left._sig == right._sig && left._exp == right._exp; }
        public static bool operator ==(IbFloat left, long right) { return left == new IbFloat(right); }
        public static bool operator ==(long left, IbFloat right) { return new IbFloat(left) == right; }
        public static bool operator !=(IbFloat left, IbFloat right) { return left._sig != right._sig || left._exp != right._exp; }
        public static bool operator !=(IbFloat left, long right) { return left != new IbFloat(right); }
        public static bool operator !=(long left, IbFloat right) { return new IbFloat(left) == right; }

        public static bool operator <(IbFloat left, IbFloat right)
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
        public static bool operator <(IbFloat left, long right) { return left < new IbFloat(right); }
        public static bool operator <(long left, IbFloat right) { return new IbFloat(left) < right; }

        public static bool operator >(IbFloat left, IbFloat right)
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
        public static bool operator >(IbFloat left, long right) { return left > new IbFloat(right); }
        public static bool operator >(long left, IbFloat right) { return new IbFloat(left) > right; }

        public static bool operator <=(IbFloat left, IbFloat right)
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

        public static bool operator <=(IbFloat left, long right) { return left <= new IbFloat(right); }
        public static bool operator <=(long left, IbFloat right) { return new IbFloat(left) <= right; }

        public static bool operator >=(IbFloat left, IbFloat right)
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
        public static bool operator >=(long left, IbFloat right) { return new IbFloat(left) >= right; }
        public static bool operator >=(IbFloat left, long right) { return left >= new IbFloat(right); }

        public static implicit operator IbFloat(ushort value) { return new IbFloat(value); }
        public static implicit operator IbFloat(short value) { return new IbFloat(value); }
        public static implicit operator IbFloat(sbyte value) { return new IbFloat(value); }
        public static implicit operator IbFloat(byte value) { return new IbFloat(value); }
        public static implicit operator IbFloat(long value) { return new IbFloat(value); }
        public static implicit operator IbFloat(uint value) { return new IbFloat(value); }
        public static implicit operator IbFloat(int value) { return new IbFloat(value); }

        public static IbFloat Zero { get { return new IbFloat(0, 0); } }
        public static IbFloat One { get { return new IbFloat(1, 0); } }
        public static IbFloat MinusOne { get { return new IbFloat(-1, 0); } }
        public bool IsZero { get { return _sig == 0 && _exp == 0; } }
        public bool IsOne { get { return _sig == 1 && _exp == 0; } }
        public bool IsMinusOne { get { return _sig == -1 && _exp == 0; } }

        public static IbFloat Abs(IbFloat value) { return new IbFloat(value._sig < 0 ? value._sig : value._sig, value._exp); }
        public static IbFloat Negate(IbFloat value) { return -value; }
        public static IbFloat Add(IbFloat left, IbFloat right) { return left + right; }
        public static IbFloat Subtract(IbFloat left, IbFloat right) { return left - right; }
        public static IbFloat Multiply(IbFloat left, IbFloat right) { return left * right; }
        public static IbFloat Divide(IbFloat dividend, IbFloat divisor) { return dividend / divisor; }

        public static IbFloat Max(IbFloat left, IbFloat right) { return left > right ? left : right; }
        public static IbFloat Min(IbFloat left, IbFloat right) { return left < right ? left : right; }
        public static int Compare(IbFloat left, IbFloat right) { return left < right ? -1 : (left > right ? 1 : 0); }
        public int CompareTo(long other) { return Compare(this, other); }
        public bool Equals(long other) { return this == other; }
    }
}
