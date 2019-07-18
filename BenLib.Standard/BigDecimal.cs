namespace System.Numerics
{
    //https://gist.github.com/nberardi/2667136
    public struct BigDecimal : IConvertible, IFormattable, IComparable, IComparable<BigDecimal>, IEquatable<BigDecimal>
    {
        public static readonly BigDecimal MinusOne = new BigDecimal(BigInteger.MinusOne, 0);
        public static readonly BigDecimal Zero = new BigDecimal(BigInteger.Zero, 0);
        public static readonly BigDecimal One = new BigDecimal(BigInteger.One, 0);

        private readonly BigInteger m_unscaledValue;
        private readonly int m_scale;

        public BigDecimal(double value) : this((decimal)value) { }
        public BigDecimal(float value) : this((decimal)value) { }
        public BigDecimal(decimal value)
        {
            byte[] bytes = FromDecimal(value);

            byte[] unscaledValueBytes = new byte[12];
            Array.Copy(bytes, unscaledValueBytes, unscaledValueBytes.Length);

            var unscaledValue = new BigInteger(unscaledValueBytes);
            byte scale = bytes[14];

            if (bytes[15] == 128) unscaledValue *= BigInteger.MinusOne;

            m_unscaledValue = unscaledValue;
            m_scale = scale;
        }

        public BigDecimal(int value) : this(new BigInteger(value), 0) { }
        public BigDecimal(long value) : this(new BigInteger(value), 0) { }
        public BigDecimal(uint value) : this(new BigInteger(value), 0) { }
        public BigDecimal(ulong value) : this(new BigInteger(value), 0) { }
        public BigDecimal(in BigInteger unscaledValue, int scale)
        {
            m_unscaledValue = unscaledValue;
            m_scale = scale;
        }

        public BigDecimal(byte[] value)
        {
            byte[] number = new byte[value.Length - 4];
            byte[] flags = new byte[4];

            Array.Copy(value, 0, number, 0, number.Length);
            Array.Copy(value, value.Length - 4, flags, 0, 4);

            m_unscaledValue = new BigInteger(number);
            m_scale = BitConverter.ToInt32(flags, 0);
        }

        public bool IsEven => m_unscaledValue.IsEven;
        public bool IsOne => m_unscaledValue.IsOne;
        public bool IsPowerOfTwo => m_unscaledValue.IsPowerOfTwo;
        public bool IsZero => m_unscaledValue.IsZero;
        public int Sign => m_unscaledValue.Sign;

        public override string ToString()
        {
            string number = m_unscaledValue.ToString("G");
            return m_scale > 0 ? number.Insert(number.Length - m_scale, ".") : number;
        }

        public byte[] ToByteArray()
        {
            byte[] unscaledValue = m_unscaledValue.ToByteArray();
            byte[] scale = BitConverter.GetBytes(m_scale);

            byte[] bytes = new byte[unscaledValue.Length + scale.Length];
            Array.Copy(unscaledValue, 0, bytes, 0, unscaledValue.Length);
            Array.Copy(scale, 0, bytes, unscaledValue.Length, scale.Length);

            return bytes;
        }

        private static byte[] FromDecimal(decimal d)
        {
            byte[] bytes = new byte[16];

            int[] bits = decimal.GetBits(d);
            int lo = bits[0];
            int mid = bits[1];
            int hi = bits[2];
            int flags = bits[3];

            bytes[0] = (byte)lo;
            bytes[1] = (byte)(lo >> 8);
            bytes[2] = (byte)(lo >> 0x10);
            bytes[3] = (byte)(lo >> 0x18);
            bytes[4] = (byte)mid;
            bytes[5] = (byte)(mid >> 8);
            bytes[6] = (byte)(mid >> 0x10);
            bytes[7] = (byte)(mid >> 0x18);
            bytes[8] = (byte)hi;
            bytes[9] = (byte)(hi >> 8);
            bytes[10] = (byte)(hi >> 0x10);
            bytes[11] = (byte)(hi >> 0x18);
            bytes[12] = (byte)flags;
            bytes[13] = (byte)(flags >> 8);
            bytes[14] = (byte)(flags >> 0x10);
            bytes[15] = (byte)(flags >> 0x18);

            return bytes;
        }

        #region Operators

        public static bool operator ==(BigDecimal left, BigDecimal right) => left.Equals(right);
        public static bool operator !=(BigDecimal left, BigDecimal right) => !left.Equals(right);
        public static bool operator >(BigDecimal left, BigDecimal right) => left.CompareTo(right) > 0;
        public static bool operator >=(BigDecimal left, BigDecimal right) => left.CompareTo(right) >= 0;
        public static bool operator <(BigDecimal left, BigDecimal right) => left.CompareTo(right) < 0;
        public static bool operator <=(BigDecimal left, BigDecimal right) => left.CompareTo(right) <= 0;
        public static bool operator ==(BigDecimal left, decimal right) => left.Equals(right);
        public static bool operator !=(BigDecimal left, decimal right) => !left.Equals(right);
        public static bool operator >(BigDecimal left, decimal right) => left.CompareTo(right) > 0;
        public static bool operator >=(BigDecimal left, decimal right) => left.CompareTo(right) >= 0;
        public static bool operator <(BigDecimal left, decimal right) => left.CompareTo(right) < 0;
        public static bool operator <=(BigDecimal left, decimal right) => left.CompareTo(right) <= 0;
        public static bool operator ==(decimal left, BigDecimal right) => left.Equals(right);
        public static bool operator !=(decimal left, BigDecimal right) => !left.Equals(right);
        public static bool operator >(decimal left, BigDecimal right) => left.CompareTo(right) > 0;
        public static bool operator >=(decimal left, BigDecimal right) => left.CompareTo(right) >= 0;
        public static bool operator <(decimal left, BigDecimal right) => left.CompareTo(right) < 0;
        public static bool operator <=(decimal left, BigDecimal right) => left.CompareTo(right) <= 0;

        public static BigDecimal operator +(BigDecimal value) => value;
        public static BigDecimal operator +(BigDecimal left, BigDecimal right)
        {
            int scale = AlignExponent(left, right, out var v1, out var v2, out _);
            return new BigDecimal(v1 + v2, scale);
        }
        public static BigDecimal operator -(BigDecimal value) => new BigDecimal(-value.m_unscaledValue, value.m_scale);
        public static BigDecimal operator -(BigDecimal left, BigDecimal right) => left + -right;
        public static BigDecimal operator *(BigDecimal left, BigDecimal right) => new BigDecimal(left.m_unscaledValue * right.m_unscaledValue, left.m_scale + right.m_scale);

        public const int MaxPrecision = 50;

        public static BigDecimal operator /(BigDecimal left, BigDecimal right)
        {
            int scale = AlignExponent(left, right, out var v1, out var v2, out int scaleDiff);
            v1 *= MaxPrecision;
            return new BigDecimal(v1 / v2, scaleDiff - MaxPrecision);
        }

        public static BigDecimal operator %(BigDecimal left, BigDecimal right) => left - right * (left / right).Floor();

        private static readonly BigInteger s_ten = 10;

        private static int AlignExponent(in BigDecimal d1, in BigDecimal d2, out BigInteger v1, out BigInteger v2, out int scaleDiff)
        {
            scaleDiff = d1.m_scale - d2.m_scale;
            if (scaleDiff == 0)
            {
                v1 = d1.m_unscaledValue;
                v2 = d2.m_unscaledValue;
                return d1.m_scale;
            }
            if (scaleDiff < 0)
            {
                v1 = d1.m_unscaledValue * BigInteger.Pow(s_ten, -scaleDiff);
                v2 = d2.m_unscaledValue;
                return d2.m_scale;
            }

            v1 = d1.m_unscaledValue;
            v2 = d2.m_unscaledValue * BigInteger.Pow(s_ten, -scaleDiff);
            return d1.m_scale;
        }

        #endregion

        #region Explicity and Implicit Casts

        public static explicit operator byte(BigDecimal value) => value.ToType<byte>();
        public static explicit operator sbyte(BigDecimal value) => value.ToType<sbyte>();
        public static explicit operator short(BigDecimal value) => value.ToType<short>();
        public static explicit operator int(BigDecimal value) => value.ToType<int>();
        public static explicit operator long(BigDecimal value) => value.ToType<long>();
        public static explicit operator ushort(BigDecimal value) => value.ToType<ushort>();
        public static explicit operator uint(BigDecimal value) => value.ToType<uint>();
        public static explicit operator ulong(BigDecimal value) => value.ToType<ulong>();
        public static explicit operator float(BigDecimal value) => value.ToType<float>();
        public static explicit operator double(BigDecimal value) => value.ToType<double>();
        public static explicit operator decimal(BigDecimal value) => value.ToType<decimal>();
        public static explicit operator BigInteger(BigDecimal value)
        {
            var scaleDivisor = BigInteger.Pow(s_ten, value.m_scale);
            var scaledValue = BigInteger.Divide(value.m_unscaledValue, scaleDivisor);
            return scaledValue;
        }

        public static implicit operator BigDecimal(byte value) => new BigDecimal(value);
        public static implicit operator BigDecimal(sbyte value) => new BigDecimal(value);
        public static implicit operator BigDecimal(short value) => new BigDecimal(value);
        public static implicit operator BigDecimal(int value) => new BigDecimal(value);
        public static implicit operator BigDecimal(long value) => new BigDecimal(value);
        public static implicit operator BigDecimal(ushort value) => new BigDecimal(value);
        public static implicit operator BigDecimal(uint value) => new BigDecimal(value);
        public static implicit operator BigDecimal(ulong value) => new BigDecimal(value);
        public static implicit operator BigDecimal(float value) => new BigDecimal(value);
        public static implicit operator BigDecimal(double value) => new BigDecimal(value);
        public static implicit operator BigDecimal(decimal value) => new BigDecimal(value);
        public static implicit operator BigDecimal(BigInteger value) => new BigDecimal(value, 0);

        #endregion

        public T ToType<T>() where T : struct => (T)((IConvertible)this).ToType(typeof(T), null);

        object IConvertible.ToType(Type conversionType, IFormatProvider provider)
        {
            var scaleDivisor = BigInteger.Pow(s_ten, m_scale);
            var remainder = BigInteger.Remainder(m_unscaledValue, scaleDivisor);
            var scaledValue = BigInteger.Divide(m_unscaledValue, scaleDivisor);

            if (scaledValue > new BigInteger(decimal.MaxValue)) throw new ArgumentOutOfRangeException("value", "The value " + m_unscaledValue + " cannot fit into " + conversionType.Name + ".");

            decimal leftOfDecimal = (decimal)scaledValue;
            decimal rightOfDecimal = (decimal)remainder / (decimal)scaleDivisor;

            decimal value = leftOfDecimal + rightOfDecimal;
            return Convert.ChangeType(value, conversionType);
        }

        public override bool Equals(object obj) => obj is BigDecimal && Equals((BigDecimal)obj);

        public override int GetHashCode() => m_unscaledValue.GetHashCode() ^ m_scale.GetHashCode();

        #region IConvertible Members

        TypeCode IConvertible.GetTypeCode() => TypeCode.Object;
        bool IConvertible.ToBoolean(IFormatProvider provider) => Convert.ToBoolean(this);
        byte IConvertible.ToByte(IFormatProvider provider) => Convert.ToByte(this);
        char IConvertible.ToChar(IFormatProvider provider) => throw new InvalidCastException("Cannot cast BigDecimal to Char");
        DateTime IConvertible.ToDateTime(IFormatProvider provider) => throw new InvalidCastException("Cannot cast BigDecimal to DateTime");
        decimal IConvertible.ToDecimal(IFormatProvider provider) => Convert.ToDecimal(this);
        double IConvertible.ToDouble(IFormatProvider provider) => Convert.ToDouble(this);
        short IConvertible.ToInt16(IFormatProvider provider) => Convert.ToInt16(this);
        int IConvertible.ToInt32(IFormatProvider provider) => Convert.ToInt32(this);
        long IConvertible.ToInt64(IFormatProvider provider) => Convert.ToInt64(this);
        sbyte IConvertible.ToSByte(IFormatProvider provider) => Convert.ToSByte(this);
        float IConvertible.ToSingle(IFormatProvider provider) => Convert.ToSingle(this);
        string IConvertible.ToString(IFormatProvider provider) => Convert.ToString(this);
        ushort IConvertible.ToUInt16(IFormatProvider provider) => Convert.ToUInt16(this);
        uint IConvertible.ToUInt32(IFormatProvider provider) => Convert.ToUInt32(this);
        ulong IConvertible.ToUInt64(IFormatProvider provider) => Convert.ToUInt64(this);

        #endregion

        #region IFormattable Members

        public string ToString(string format, IFormatProvider formatProvider) => throw new NotImplementedException();

        #endregion

        #region IComparable Members

        public int CompareTo(object obj)
        {
            if (obj == null)
                return 1;

            if (!(obj is BigDecimal))
                throw new ArgumentException("Compare to object must be a BigDecimal", "obj");

            return CompareTo((BigDecimal)obj);
        }

        #endregion

        #region IComparable<BigDecimal> Members

        public int CompareTo(BigDecimal other)
        {
            int unscaledValueCompare = m_unscaledValue.CompareTo(other.m_unscaledValue);
            int scaleCompare = m_scale.CompareTo(other.m_scale);

            // if both are the same value, return the value
            if (unscaledValueCompare == scaleCompare) return unscaledValueCompare;

            // if the scales are both the same return unscaled value
            if (scaleCompare == 0) return unscaledValueCompare;

            var scaledValue = BigInteger.Divide(m_unscaledValue, BigInteger.Pow(s_ten, m_scale));
            var otherScaledValue = BigInteger.Divide(other.m_unscaledValue, BigInteger.Pow(s_ten, other.m_scale));

            return scaledValue.CompareTo(otherScaledValue);
        }

        #endregion

        #region IEquatable<BigDecimal> Members

        public bool Equals(BigDecimal other) => m_scale == other.m_scale && m_unscaledValue == other.m_unscaledValue;

        #endregion
    }
}
