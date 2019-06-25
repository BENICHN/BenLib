using BenLib.Standard;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;

namespace BenLib.WPF
{
    public class TypedTextBox : TextBox
    {
        public Regex Regex { get => (Regex)GetValue(RegexProperty); set => SetValue(RegexProperty, value); }
        public static readonly DependencyProperty RegexProperty = DependencyProperty.Register("Regex", typeof(Regex), typeof(TypedTextBox));

        public ContentType ContentType { get => (ContentType)GetValue(ContentTypeProperty); set => SetValue(ContentTypeProperty, value); }
        public static readonly DependencyProperty ContentTypeProperty = DependencyProperty.Register("ContentType", typeof(ContentType), typeof(TypedTextBox));

        public ICollection<string> AllowedStrings { get => (ICollection<string>)GetValue(AllowedStringsProperty); set => SetValue(AllowedStringsProperty, value); }
        public static readonly DependencyProperty AllowedStringsProperty = DependencyProperty.Register("AllowedStrings", typeof(ICollection<string>), typeof(TypedTextBox));

        public ICollection<string> ForbiddenStrings { get => (ICollection<string>)GetValue(ForbiddenStringsProperty); set => SetValue(ForbiddenStringsProperty, value); }
        public static readonly DependencyProperty ForbiddenStringsProperty = DependencyProperty.Register("ForbiddenStrings", typeof(ICollection<string>), typeof(TypedTextBox));

        static TypedTextBox() => TextProperty.OverrideMetadata(typeof(TypedTextBox), new FrameworkPropertyMetadata(string.Empty, null, (d, value) =>
        {
            if (d is TypedTextBox t && value is string s)
            {
                bool valid = (t.AllowedStrings?.Contains(s) ?? false) || (!t.ForbiddenStrings?.Contains(s) ?? true) && (t.Regex?.IsMatch(s) ?? true) && t.ContentType switch
                {
                    ContentType.Integer => Literal.PreviewInteger.IsMatch(s),
                    ContentType.UnsignedInteger => Literal.PreviewUnsignedInteger.IsMatch(s),
                    ContentType.Decimal => Literal.PreviewDecimal.IsMatch(s),
                    ContentType.UnsignedDecimal => Literal.PreviewUnsignedDecimal.IsMatch(s),
                    _ => true
                };
                return valid ? s : t.Text;
            }
            else return null;
        }));
    }

    public enum ContentType { Text, Integer, UnsignedInteger, Decimal, UnsignedDecimal }
}
