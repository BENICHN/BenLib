using BenLib.Standard;
using System.Collections.Generic;
using static BenLib.Standard.Num;

namespace System.Numerics
{
    public readonly struct BigRational : IComparable<BigRational>, IEquatable<BigRational>
    {
        public const int ApproxPrecision = 50;
        private static readonly BigInteger s_ten = new BigInteger(10);

        public BigRational(BigInteger numerator, BigInteger denominator)
        {
            if (denominator.IsZero) numerator = numerator.Sign;
            else if (numerator.IsZero) denominator = BigInteger.One;
            else
            {
                var pgcd = BigInteger.GreatestCommonDivisor(numerator, denominator);
                if (denominator.Sign < 0) pgcd = -pgcd;
                numerator /= pgcd;
                denominator /= pgcd;
            }
            Numerator = numerator;
            Denominator = denominator;
        }
        public static BigRational InvertOf(BigInteger value) => new BigRational(BigInteger.One, value);

        public BigInteger Numerator { get; }
        public BigInteger Denominator { get; }

        public BigRational Opposite => new BigRational(-Numerator, Denominator);
        public BigRational Invert => new BigRational(Denominator, Numerator);

        public static readonly BigRational Epsilon = new BigRational(BigInteger.One, BigInteger.Pow(s_ten, ApproxPrecision));
        public static readonly BigRational Zero = new BigRational(BigInteger.Zero, BigInteger.One);
        public static readonly BigRational One = new BigRational(BigInteger.One, BigInteger.One);
        public static readonly BigRational MinusOne = new BigRational(BigInteger.MinusOne, BigInteger.One);
        public static readonly BigRational NaN = new BigRational(BigInteger.Zero, BigInteger.Zero);
        public static readonly BigRational PositiveInfinity = new BigRational(BigInteger.One, BigInteger.Zero);
        public static readonly BigRational NegativeInfinity = new BigRational(BigInteger.MinusOne, BigInteger.Zero);

        public bool IsZero => Numerator.IsZero && Denominator.IsOne;
        public bool IsOne => Numerator.IsOne && Denominator.IsOne;
        public bool IsMinusOne => Numerator == MinusOne && Denominator.IsOne;
        public bool IsNaN => Numerator.IsZero && Denominator.IsZero;
        public bool IsPositiveInfinity => Numerator.IsOne && Denominator.IsZero;
        public bool IsNegativeInfinity => Numerator == MinusOne && Denominator.IsZero;
        public int Sign => Numerator.Sign;

        public int CompareTo(BigRational other)
        {
            var d = EqualizeDenominator(this, other, out var n1, out var n2);
            return d.IsZero ? (Numerator * other.Numerator).Sign < 0 ? Numerator.Sign : 0 : n1.CompareTo(n2);
        }

        public override bool Equals(object obj) => obj is BigRational rational && Equals(rational);
        public bool Equals(BigRational other) => Numerator.Equals(other.Numerator) && Denominator.Equals(other.Denominator);
        public override int GetHashCode()
        {
            int hashCode = -1534900553;
            hashCode = hashCode * -1521134295 + EqualityComparer<BigInteger>.Default.GetHashCode(Numerator);
            hashCode = hashCode * -1521134295 + EqualityComparer<BigInteger>.Default.GetHashCode(Denominator);
            return hashCode;
        }

        public override string ToString() => Denominator.IsZero ? Numerator.Sign switch
        {
            -1 => "-∞",
            0 => "NaN",
            1 => "+∞",
            _ => throw new FormatException()
        } : Denominator.IsOne ? Numerator.ToString() : $"{Numerator}/{Denominator}";

        public static BigRational operator +(BigRational value) => value;
        public static BigRational operator +(BigRational left, BigRational right)
        {
            if (left.IsNaN || right.IsNaN) return NaN;
            var d = EqualizeDenominator(left, right, out var n1, out var n2);
            return new BigRational(n1 + n2, d);
        }
        public static BigRational operator -(BigRational value) => value.Opposite;
        public static BigRational operator -(BigRational left, BigRational right)
        {
            if (left.IsNaN || right.IsNaN) return NaN;
            var d = EqualizeDenominator(left, right, out var n1, out var n2);
            return new BigRational(n1 - n2, d);
        }
        public static BigRational operator *(BigRational left, BigRational right) => new BigRational(left.Numerator * right.Numerator, left.Denominator * right.Denominator);
        public static BigRational operator /(BigRational left, BigRational right) => new BigRational(left.Numerator * right.Denominator, left.Denominator * right.Numerator);
        public static BigRational operator %(BigRational left, BigRational right) => left - (BigInteger)(left / right) * right;

        public static BigRational Pow(BigRational value, int exponent) => exponent < 0 ? new BigRational(BigInteger.Pow(value.Denominator, -exponent), BigInteger.Pow(value.Numerator, -exponent)) : new BigRational(BigInteger.Pow(value.Numerator, exponent), BigInteger.Pow(value.Denominator, exponent));
        public static BigRational IntegerRoot(BigRational value, int root) => new BigRational(value.Numerator.IntegerRoot(root), value.Denominator.IntegerRoot(root));
        public static BigRational Root(BigRational value, int root)
        {
            var oldValue = Zero;
            BigRational newValue = RoughRoot((BigInteger)value, root);

            for (int i = 0; Abs(newValue - oldValue) >= Epsilon && i < MaxRootIterations; i++)
            {
                oldValue = newValue;
                newValue = Truncate(((root - 1) * oldValue + value / Pow(oldValue, root - 1)) / root, ApproxPrecision);
            }

            return newValue;
        }
        public static double Log(BigRational value, double baseValue) => BigInteger.Log(value.Numerator, baseValue) - BigInteger.Log(value.Denominator, baseValue);
        public static double Log10(BigRational value) => Log(value, 10.0);
        public static double Ln(BigRational value) => Log(value, Math.E);

        public static BigRational Abs(BigRational value) => new BigRational(BigInteger.Abs(value.Numerator), value.Denominator);
        public static BigRational Min(BigRational v1, BigRational v2) => v1 < v2 ? v1 : v2;
        public static BigRational Max(BigRational v1, BigRational v2) => v1 > v2 ? v1 : v2;
        public static BigRational Truncate(BigRational value, int precision) => new BigRational(DivideInt(value.Numerator, value.Denominator, precision, out int logDiff), BigInteger.Pow(s_ten, logDiff));

        public static bool operator ==(BigRational left, BigRational right) => left.Equals(right);
        public static bool operator !=(BigRational left, BigRational right) => !left.Equals(right);
        public static bool operator >(BigRational left, BigRational right) => left.CompareTo(right) > 0;
        public static bool operator >=(BigRational left, BigRational right) => left.CompareTo(right) >= 0;
        public static bool operator <(BigRational left, BigRational right) => left.CompareTo(right) < 0;
        public static bool operator <=(BigRational left, BigRational right) => left.CompareTo(right) <= 0;
        public static bool operator ==(BigRational left, decimal right) => left.Equals(right);
        public static bool operator !=(BigRational left, decimal right) => !left.Equals(right);
        public static bool operator >(BigRational left, decimal right) => left.CompareTo(right) > 0;
        public static bool operator >=(BigRational left, decimal right) => left.CompareTo(right) >= 0;
        public static bool operator <(BigRational left, decimal right) => left.CompareTo(right) < 0;
        public static bool operator <=(BigRational left, decimal right) => left.CompareTo(right) <= 0;
        public static bool operator ==(decimal left, BigRational right) => left.Equals(right);
        public static bool operator !=(decimal left, BigRational right) => !left.Equals(right);
        public static bool operator >(decimal left, BigRational right) => left.CompareTo(right) > 0;
        public static bool operator >=(decimal left, BigRational right) => left.CompareTo(right) >= 0;
        public static bool operator <(decimal left, BigRational right) => left.CompareTo(right) < 0;
        public static bool operator <=(decimal left, BigRational right) => left.CompareTo(right) <= 0;
        public static bool operator ==(BigRational left, double right) => left.Equals(right);
        public static bool operator !=(BigRational left, double right) => !left.Equals(right);
        public static bool operator >(BigRational left, double right) => left.CompareTo(right) > 0;
        public static bool operator >=(BigRational left, double right) => left.CompareTo(right) >= 0;
        public static bool operator <(BigRational left, double right) => left.CompareTo(right) < 0;
        public static bool operator <=(BigRational left, double right) => left.CompareTo(right) <= 0;
        public static bool operator ==(double left, BigRational right) => left.Equals(right);
        public static bool operator !=(double left, BigRational right) => !left.Equals(right);
        public static bool operator >(double left, BigRational right) => left.CompareTo(right) > 0;
        public static bool operator >=(double left, BigRational right) => left.CompareTo(right) >= 0;
        public static bool operator <(double left, BigRational right) => left.CompareTo(right) < 0;
        public static bool operator <=(double left, BigRational right) => left.CompareTo(right) <= 0;

        public static implicit operator BigRational(BigInteger value) => new BigRational(value, BigInteger.One);
        public static implicit operator BigRational(long value) => new BigRational(value, BigInteger.One);
        public static implicit operator BigRational(ulong value) => new BigRational(value, BigInteger.One);
        public static implicit operator BigRational(int value) => new BigRational(value, BigInteger.One);
        public static implicit operator BigRational(uint value) => new BigRational(value, BigInteger.One);
        public static implicit operator BigRational(short value) => new BigRational(value, BigInteger.One);
        public static implicit operator BigRational(ushort value) => new BigRational(value, BigInteger.One);
        public static implicit operator BigRational(sbyte value) => new BigRational(value, BigInteger.One);
        public static implicit operator BigRational(byte value) => new BigRational(value, BigInteger.One);
        public static implicit operator BigRational(float value)
        {
            BigInteger exp = 1;
            while (Math.Truncate(value) != value)
            {
                exp *= s_ten;
                value *= 10;
            }
            return new BigRational((BigInteger)value, exp);
        }
        public static implicit operator BigRational(double value)
        {
            BigInteger exp = 1;
            while (Math.Truncate(value) != value)
            {
                exp *= s_ten;
                value *= 10;
            }
            return new BigRational((BigInteger)value, exp);
        }
        public static implicit operator BigRational(decimal value)
        {
            BigInteger exp = 1;
            while (Math.Truncate(value) != value)
            {
                exp *= s_ten;
                value *= 10;
            }
            return new BigRational((BigInteger)value, exp);
        }

        public static explicit operator BigInteger(BigRational value) => value.Numerator / value.Denominator;
        public static explicit operator long(BigRational value) => (long)(value.Numerator / value.Denominator);
        public static explicit operator ulong(BigRational value) => (ulong)(value.Numerator / value.Denominator);
        public static explicit operator int(BigRational value) => (int)(value.Numerator / value.Denominator);
        public static explicit operator uint(BigRational value) => (uint)(value.Numerator / value.Denominator);
        public static explicit operator short(BigRational value) => (short)(value.Numerator / value.Denominator);
        public static explicit operator ushort(BigRational value) => (ushort)(value.Numerator / value.Denominator);
        public static explicit operator sbyte(BigRational value) => (sbyte)(value.Numerator / value.Denominator);
        public static explicit operator byte(BigRational value) => (byte)(value.Numerator / value.Denominator);
        public static explicit operator decimal(BigRational value) => (decimal)DivideInt(value.Numerator, value.Denominator, 29, out int logDiff) / 10M.IntPow((uint)logDiff);
        public static explicit operator double(BigRational value) => (double)DivideInt(value.Numerator, value.Denominator, 17, out int logDiff) / 10.0.IntPow((uint)logDiff);
        public static explicit operator float(BigRational value) => (float)DivideInt(value.Numerator, value.Denominator, 9, out int logDiff) / 10F.IntPow((uint)logDiff);

        #region Helper

        public static BigInteger DivideInt(BigInteger dividend, BigInteger divisor, int digits, out int logDiff)
        {
            int diff = dividend.Digits() - divisor.Digits();
            logDiff = Math.Max(0, digits - diff);
            dividend *= BigInteger.Pow(s_ten, logDiff);
            return dividend / divisor;
        }

        public static BigInteger EqualizeDenominator(in BigRational r1, in BigRational r2, out BigInteger n1, out BigInteger n2)
        {
            var (num1, den1, num2, den2) = (r1.Numerator, r1.Denominator, r2.Numerator, r2.Denominator);

            if (den1 == den2)
            {
                n1 = num1;
                n2 = num2;
                return den1;
            }

            n1 = num1 * den2;
            n2 = num2 * den1;
            return den1 * den2;
        }

        #endregion
    }
}
