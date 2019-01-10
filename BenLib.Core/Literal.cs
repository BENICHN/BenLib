using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace BenLib
{
    public static class Literal
    {
        public static Regex Integer = new Regex(@"^(\-)?\d+$");
        public static Regex PreviewInteger = new Regex(@"^(\-)?(\d+)?$");

        public static Regex UnsignedInteger = new Regex(@"^\d+$");
        public static Regex PreviewUnsignedInteger = new Regex(@"^(\d+)?$");

        public static Regex Double = new Regex(@"^(\-)?\d+((\.|,)\d+)?(E(\+|\-)?\d+)?$");
        public static Regex PreviewDouble = new Regex(@"^(\-)?(\d+(((\.|,)(\d+)?)?(E(\+|\-)?(\d+)?)?)?)?$");

        public static Regex UnsignedDouble = new Regex(@"^\d+((\.|,)\d+)?(E(\+|\-)?\d+)?$");
        public static Regex PreviewUnsignedDouble = new Regex(@"^(\d+(((\.|,)(\d+)?)?(E(\+|\-)?(\d+)?)?)?)?$");

        public static NumberFormatInfo DecimalSeparatorPoint = new NumberFormatInfo() { NumberDecimalSeparator = ".", PercentDecimalSeparator = ".", CurrencyDecimalSeparator = "." };
        public static NumberFormatInfo DecimalSeparatorComma = new NumberFormatInfo() { NumberDecimalSeparator = ",", PercentDecimalSeparator = ",", CurrencyDecimalSeparator = "," };

        public static string CoefsToString(bool sort, params (double Coef, string Letter)[] expression)
        {
            var resultBuilder = new StringBuilder();
            foreach (var (coef, letter) in sort ? expression.Where(cl => cl.Coef != 0).GroupBy(cl => cl.Coef > 0).OrderByDescending(group => group.Key).SelectMany(group => group) : expression) AppendCoef(coef, letter);
            string result = resultBuilder.ToString().Trim('+', ' ');
            if (result.StartsWith("- ")) result = result.Remove(1, 1);
            return result;

            void AppendCoef(double coef, string letter)
            {
                if (coef > 0) resultBuilder.Append($"+ {(coef == 1 && !letter.IsNullOrWhiteSpace() ? string.Empty : coef.ToString())}{letter} ");
                else if (coef < 0) resultBuilder.Append($"- {(coef == -1 && !letter.IsNullOrWhiteSpace() ? string.Empty : (-coef).ToString())}{letter} ");
            }
        }
    }

    public static partial class Extensions
    {
        public static string TrimStart(this string target, string trimString, int times = 0)
        {
            double timesD = times > 0 ? times : double.PositiveInfinity;
            string result = target;

            for (int i = 0; i < timesD && result.StartsWith(trimString); i++) result = result.Substring(trimString.Length);

            return result;
        }

        public static string TrimEnd(this string target, string trimString, int times = 0)
        {
            double timesD = times > 0 ? times : double.PositiveInfinity;
            string result = target;

            for (int i = 0; i < timesD && result.EndsWith(trimString); i++) result = result.Substring(0, result.Length - trimString.Length);

            return result;
        }

        public static bool ContainsAny(this string s, IEnumerable<char> values)
        {
            if (s == null || values == null) return false;

            foreach (char c in values)
            {
                if (s.Contains(c)) return true;
            }

            return false;
        }

        public static bool ContainsAny(this string s, IEnumerable<string> values)
        {
            if (s == null || values == null) return false;

            foreach (string value in values)
            {
                if (s.Contains(value)) return true;
            }

            return false;
        }

        public static bool Contains(this string s, IEnumerable<char> values)
        {
            if (s == null || values == null) return false;

            foreach (char c in values)
            {
                if (!s.Contains(c)) return false;
            }

            return true;
        }

        public static bool Contains(this string s, IEnumerable<string> values)
        {
            if (s == null || values == null) return false;

            foreach (string value in values)
            {
                if (!s.Contains(value)) return false;
            }

            return true;
        }

        public static bool Contains(this string s, IEnumerable<string> values, StringComparison comparison)
        {
            if (s == null || values == null) return false;

            foreach (string value in values)
            {
                if (s.IndexOf(value, comparison) < 0) return false;
            }

            return true;
        }

        public static bool Contains(this string s, string value, StringComparison comparison)
        {
            return s.IndexOf(value, comparison) >= 0;
        }

        public static string Replace(this string s, IEnumerable<string> oldValues, string newValue)
        {
            if (s == null) return null;
            if (oldValues == null || newValue == null) return s;

            foreach (string value in oldValues)
            {
                s = s.Replace(value, newValue);
            }

            return s;
        }

        public static string Replace(this string s, IEnumerable<char> oldChars, char newChar)
        {
            if (s == null) return null;
            if (oldChars == null) return s;

            foreach (char c in oldChars)
            {
                s = s.Replace(c, newChar);
            }

            return s;
        }

        public static bool IsEmpty(this string s) => s == string.Empty ? true : false;

        public static bool IsNullOrEmpty(this string s) => string.IsNullOrEmpty(s);
        public static bool IsNullOrWhiteSpace(this string s) => string.IsNullOrWhiteSpace(s);

        public static List<int> AllIndexesOf(this string str, string value)
        {
            if (string.IsNullOrEmpty(value)) throw new ArgumentException("the string to find may not be empty", "value");
            List<int> indexes = new List<int>();
            for (int index = 0; ; index += value.Length)
            {
                index = str.IndexOf(value, index);
                if (index == -1) return indexes;
                indexes.Add(index);
            }
        }

        #region Is

        public static bool IsInteger(this string s) => Literal.Integer.IsMatch(s);

        public static bool IsDecimalNumber(this string s) => Literal.Double.IsMatch(s);

        #region Integer

        #region Signed

        public static TryResult IsInt(this string s)
        {
            try
            {
                int.Parse(s);
                return new TryResult(true);
            }
            catch (Exception ex) { return new TryResult(false, ex); }
        }

        public static TryResult IsLong(this string s)
        {
            try
            {
                long.Parse(s);
                return new TryResult(true);
            }
            catch (Exception ex) { return new TryResult(false, ex); }
        }

        public static TryResult IsShort(this string s)
        {
            try
            {
                short.Parse(s);
                return new TryResult(true);
            }
            catch (Exception ex) { return new TryResult(false, ex); }
        }

        public static TryResult IsSByte(this string s)
        {
            try
            {
                sbyte.Parse(s);
                return new TryResult(true);
            }
            catch (Exception ex) { return new TryResult(false, ex); }
        }

        #endregion

        #region Unsigned

        public static TryResult IsUInt(this string s)
        {
            try
            {
                uint.Parse(s);
                return new TryResult(true);
            }
            catch (Exception ex) { return new TryResult(false, ex); }
        }

        public static TryResult IsULong(this string s)
        {
            try
            {
                ulong.Parse(s);
                return new TryResult(true);
            }
            catch (Exception ex) { return new TryResult(false, ex); }
        }

        public static TryResult IsUShort(this string s)
        {
            try
            {
                ushort.Parse(s);
                return new TryResult(true);
            }
            catch (Exception ex) { return new TryResult(false, ex); }
        }

        public static TryResult IsByte(this string s)
        {
            try
            {
                byte.Parse(s);
                return new TryResult(true);
            }
            catch (Exception ex) { return new TryResult(false, ex); }
        }

        #endregion

        #endregion

        #region Decimal

        public static TryResult IsDouble(this string s)
        {
            try
            {
                double.Parse(s.Replace(',', '.'), Literal.DecimalSeparatorPoint);
                return new TryResult(true);
            }
            catch (Exception ex) { return new TryResult(false, ex); }
        }

        public static TryResult IsFloat(this string s)
        {
            try
            {
                float.Parse(s.Replace(',', '.'), Literal.DecimalSeparatorPoint);
                return new TryResult(true);
            }
            catch (Exception ex) { return new TryResult(false, ex); }
        }

        public static TryResult IsDecimal(this string s)
        {
            try
            {
                decimal.Parse(s.Replace(',', '.'), Literal.DecimalSeparatorPoint);
                return new TryResult(true);
            }
            catch (Exception ex) { return new TryResult(false, ex); }
        }

        #endregion

        #endregion

        #region Num.ToString

        public static string ToString(this double d, int maxDecimalPlaces, bool unlimitedAtZero = false)
        {
            if (unlimitedAtZero && maxDecimalPlaces == 0) return d.ToString();
            if (d == 0) return "0";
            StringBuilder sb = new StringBuilder("0");
            if (maxDecimalPlaces > 0) sb.Append('.');
            maxDecimalPlaces.Times(i => sb.Append('#'));
            return d.ToString(sb.ToString());
        }

        public static string ToString(this decimal d, int maxDecimalPlaces, bool unlimitedAtZero = false)
        {
            if (unlimitedAtZero && maxDecimalPlaces == 0) return d.ToString();
            if (d == 0) return "0";
            StringBuilder sb = new StringBuilder("0");
            if (maxDecimalPlaces > 0) sb.Append('.');
            maxDecimalPlaces.Times(i => sb.Append('#'));
            return d.ToString(sb.ToString());
        }

        public static string ToString(this float f, int maxDecimalPlaces, bool unlimitedAtZero = false)
        {
            if (unlimitedAtZero && maxDecimalPlaces == 0) return f.ToString();
            if (f == 0) return "0";
            StringBuilder sb = new StringBuilder("0");
            if (maxDecimalPlaces > 0) sb.Append('.');
            maxDecimalPlaces.Times(i => sb.Append('#'));
            return f.ToString(sb.ToString());
        }

        #endregion

        #region Parse

        #region Integer

        #region Signed

        public static int? ToInt(this string s)
        {
            try { return int.Parse(s); }
            catch { return null; }
        }

        public static long? ToLong(this string s)
        {
            try { return long.Parse(s); }
            catch { return null; }
        }

        public static short? ToShort(this string s)
        {
            try { return short.Parse(s); }
            catch { return null; }
        }

        public static sbyte? ToSByte(this string s)
        {
            try { return sbyte.Parse(s); }
            catch { return null; }
        }

        #endregion

        #region Unsigned

        public static uint? ToUInt(this string s)
        {
            try { return uint.Parse(s); }
            catch { return null; }
        }

        public static ulong? ToULong(this string s)
        {
            try { return ulong.Parse(s); }
            catch { return null; }
        }

        public static ushort? ToUShort(this string s)
        {
            try { return ushort.Parse(s); }
            catch { return null; }
        }

        public static byte? ToByte(this string s)
        {
            try { return byte.Parse(s); }
            catch { return null; }
        }

        #endregion

        #endregion

        #region Decimal

        public static double? ToDouble(this string s)
        {
            try { return double.Parse(s.Replace(',', '.'), Literal.DecimalSeparatorPoint); }
            catch { return null; }
        }

        public static float? ToFloat(this string s)
        {
            try { return float.Parse(s.Replace(',', '.'), Literal.DecimalSeparatorPoint); }
            catch { return null; }
        }

        public static decimal? ToDecimal(this string s)
        {
            try { return decimal.Parse(s.Replace(',', '.'), Literal.DecimalSeparatorPoint); }
            catch { return null; }
        }

        #endregion

        public static bool? ToBool(this string s)
        {
            try { return bool.Parse(s); }
            catch { return null; }
        }

        public static TEnum? ToEnum<TEnum>(this string s) where TEnum : struct
        {
            try { return (TEnum)Enum.Parse(typeof(TEnum), s); }
            catch { return null; }
        }

        #endregion
    }
}
