using BenLib.Standard;
using System;
using System.CodeDom;
using System.CodeDom.Compiler;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Media;

namespace BenLib.Framework
{
    public static partial class Extensions
    {
        public static string Escape(this string input)
        {
            using (var writer = new StringWriter())
            {
                using (var provider = CodeDomProvider.CreateProvider("CSharp"))
                {
                    provider.GenerateCodeFromExpression(new CodePrimitiveExpression(input), writer, null);
                    return writer.ToString();
                }
            }
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

            double? value = s.TrimEnd('*').ToDouble();
            return value != null ? (GridLength?)new GridLength((double)value, s.Contains('*') ? GridUnitType.Star : GridUnitType.Pixel) : null;
        }
    }
}
