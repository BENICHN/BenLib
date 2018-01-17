using System;
using System.Text;
using System.Text.RegularExpressions;
using System.Globalization;
using System.Linq;
using System.Windows.Media;
using System.Collections.Generic;
using System.Windows;

namespace BenLib
{
    public class Literal
    {
        public static Regex Integrer = new Regex(@"^(\-)?\d+$");
        public static Regex PreviewIntegrer = new Regex(@"^(\-)?(\d+)?$");

        public static Regex UnsignedIntegrer = new Regex(@"^\d+$");
        public static Regex PreviewUnsignedIntegrer = new Regex(@"^(\d+)?$");

        public static Regex Double = new Regex(@"^(\-)?\d+((\.|,)\d+)?(E(\+|\-)?\d+)?$");
        public static Regex PreviewDouble = new Regex(@"^(\-)?(\d+(((\.|,)(\d+)?)?(E(\+|\-)?(\d+)?)?)?)?$");

        public static Regex UnsignedDouble = new Regex(@"^\d+((\.|,)\d+)?(E(\+|\-)?\d+)?$");
        public static Regex PreviewUnsignedDouble = new Regex(@"^(\d+(((\.|,)(\d+)?)?(E(\+|\-)?(\d+)?)?)?)?$");

        public static NumberFormatInfo DecimalSeparatorPoint = new NumberFormatInfo() { NumberDecimalSeparator = ".", PercentDecimalSeparator = ".", CurrencyDecimalSeparator = "." };
        public static NumberFormatInfo DecimalSeparatorComma = new NumberFormatInfo() { NumberDecimalSeparator = ",", PercentDecimalSeparator = ",", CurrencyDecimalSeparator = "," };

        public static string[] ForbiddenPathNameCharacters { get; set; } = { "<", ">", ":", "\"", "/", "\\", "|", "?", "*" };
    }

    public static partial class Extensions
    {
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

        public static bool IsEmpty(this string s) => s == String.Empty ? true : false;

        public static bool IsNullOrEmpty(this string s) => String.IsNullOrEmpty(s);

        #region Is

        public static bool IsIntegrer(this string s) => Literal.Integrer.IsMatch(s);

        public static bool IsDecimalNumber(this string s) => Literal.Double.IsMatch(s);

        #region Integrer

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
            maxDecimalPlaces.Times(() => sb.Append('#'));
            return d.ToString(sb.ToString());
        }

        public static string ToString(this decimal d, int maxDecimalPlaces, bool unlimitedAtZero = false)
        {
            if (unlimitedAtZero && maxDecimalPlaces == 0) return d.ToString();
            if (d == 0) return "0";
            StringBuilder sb = new StringBuilder("0");
            if (maxDecimalPlaces > 0) sb.Append('.');
            maxDecimalPlaces.Times(() => sb.Append('#'));
            return d.ToString(sb.ToString());
        }

        public static string ToString(this float f, int maxDecimalPlaces, bool unlimitedAtZero = false)
        {
            if (unlimitedAtZero && maxDecimalPlaces == 0) return f.ToString();
            if (f == 0) return "0";
            StringBuilder sb = new StringBuilder("0");
            if (maxDecimalPlaces > 0) sb.Append('.');
            maxDecimalPlaces.Times(() => sb.Append('#'));
            return f.ToString(sb.ToString());
        }

        #endregion

        #region Parse

        #region Integrer

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

        public static Color? ToColor(this string s)
        {
            try { return (Color)ColorConverter.ConvertFromString(s); }
            catch { return null; }
        }

        public static GridLength? ToGridLength(this string s)
        {
            if (s.IsNullOrEmpty()) return null;
            if (s.Equals("Auto", StringComparison.OrdinalIgnoreCase)) return new GridLength(1, GridUnitType.Auto);
            if (s == "*") return new GridLength(1, GridUnitType.Star);

            var value = s.TrimEnd('*').ToDouble();
            if (value != null) return new GridLength((double)value, s.Contains('*') ? GridUnitType.Star : GridUnitType.Pixel);

            return null;
        }

        public static TEnum? ToEnum<TEnum>(this string s) where TEnum : struct
        {
            try { return (TEnum)Enum.Parse(typeof(TEnum), s); }
            catch { return null; }
        }

        #endregion
    }
}
