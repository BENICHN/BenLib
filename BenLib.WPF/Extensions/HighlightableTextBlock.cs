using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;

namespace BenLib.WPF
{
    /// <summary>
    /// Authors : kthsu (https://github.com/kthsu/HighlightableTextBlock/) and BenNat
    /// </summary>
    public class HighlightableTextBlock
    {
        #region FontWeight

        public static FontWeight GetFontWeight(DependencyObject obj) => (FontWeight)obj.GetValue(FontWeightProperty);

        public static void SetFontWeight(DependencyObject obj, FontWeight value) => obj.SetValue(FontWeightProperty, value);

        // Using a DependencyProperty as the backing store for FontWeight.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty FontWeightProperty = DependencyProperty.RegisterAttached("FontWeight", typeof(FontWeight), typeof(HighlightableTextBlock), new PropertyMetadata(FontWeights.Regular, Refresh));

        #endregion

        #region FontStyle

        public static FontStyle GetFontStyle(DependencyObject obj) => (FontStyle)obj.GetValue(FontStyleProperty);

        public static void SetFontStyle(DependencyObject obj, FontStyle value) => obj.SetValue(FontStyleProperty, value);

        // Using a DependencyProperty as the backing store for FontStyle.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty FontStyleProperty = DependencyProperty.RegisterAttached("FontStyle", typeof(FontStyle), typeof(HighlightableTextBlock), new PropertyMetadata(FontStyles.Normal, Refresh));

        #endregion

        #region Underline

        public static bool GetUnderline(DependencyObject obj) => (bool)obj.GetValue(UnderlineProperty);

        public static void SetUnderline(DependencyObject obj, bool value) => obj.SetValue(UnderlineProperty, value);

        // Using a DependencyProperty as the backing store for Underline.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty UnderlineProperty = DependencyProperty.RegisterAttached("Underline", typeof(bool), typeof(HighlightableTextBlock), new PropertyMetadata(false, Refresh));

        #endregion

        #region HighlightTextBrush

        public static Brush GetHighlightTextBrush(DependencyObject obj) => (Brush)obj.GetValue(HighlightTextBrushProperty);

        public static void SetHighlightTextBrush(DependencyObject obj, Brush value) => obj.SetValue(HighlightTextBrushProperty, value);

        // Using a DependencyProperty as the backing store for HighlightTextBrush.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty HighlightTextBrushProperty = DependencyProperty.RegisterAttached("HighlightTextBrush", typeof(Brush), typeof(HighlightableTextBlock), new PropertyMetadata(SystemColors.HighlightTextBrush, Refresh));

        #endregion

        #region HighlightBrush

        public static Brush GetHighlightBrush(DependencyObject obj) => (Brush)obj.GetValue(HighlightBrushProperty);

        public static void SetHighlightBrush(DependencyObject obj, Brush value) => obj.SetValue(HighlightBrushProperty, value);

        // Using a DependencyProperty as the backing store for HighlightBrush.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty HighlightBrushProperty = DependencyProperty.RegisterAttached("HighlightBrush", typeof(Brush), typeof(HighlightableTextBlock), new PropertyMetadata(SystemColors.HighlightBrush, Refresh));

        #endregion

        #region HighlightText

        public static string GetHightlightText(DependencyObject obj) => (string)obj.GetValue(HightlightTextProperty);

        public static void SetHightlightText(DependencyObject obj, string value) => obj.SetValue(HightlightTextProperty, value);

        // Using a DependencyProperty as the backing store for HightlightText.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty HightlightTextProperty = DependencyProperty.RegisterAttached("HightlightText", typeof(string), typeof(HighlightableTextBlock), new PropertyMetadata(string.Empty, Refresh));

        #endregion

        #region InternalText

        protected static string GetInternalText(DependencyObject obj) => (string)obj.GetValue(InternalTextProperty);

        protected static void SetInternalText(DependencyObject obj, string value) => obj.SetValue(InternalTextProperty, value);

        // Using a DependencyProperty as the backing store for InternalText.  This enables animation, styling, binding, etc...
        protected static readonly DependencyProperty InternalTextProperty = DependencyProperty.RegisterAttached("InternalText", typeof(string), typeof(HighlightableTextBlock), new PropertyMetadata(string.Empty, OnInternalTextChanged));

        private static void OnInternalTextChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is TextBlock textblock)
            {
                textblock.Text = e.NewValue as string;
                Highlight(textblock);
            }
        }

        #endregion

        #region  IsBusy 

        private static bool GetIsBusy(DependencyObject obj) => (bool)obj.GetValue(IsBusyProperty);

        private static void SetIsBusy(DependencyObject obj, bool value) => obj.SetValue(IsBusyProperty, value);

        // Using a DependencyProperty as the backing store for IsBusy.  This enables animation, styling, binding, etc...
        private static readonly DependencyProperty IsBusyProperty = DependencyProperty.RegisterAttached("IsBusy", typeof(bool), typeof(HighlightableTextBlock), new PropertyMetadata(false));

        #endregion

        #region CaseSensitive

        public static bool GetCaseSensitive(DependencyObject obj) => (bool)obj.GetValue(CaseSensitiveProperty);
        public static void SetCaseSensitive(DependencyObject obj, bool value) => obj.SetValue(CaseSensitiveProperty, value);

        public static readonly DependencyProperty CaseSensitiveProperty = DependencyProperty.RegisterAttached("CaseSensitive", typeof(bool), typeof(HighlightableTextBlock), new PropertyMetadata(false, Refresh));

        #endregion

        #region IsRegex

        public static bool GetIsRegex(DependencyObject obj) => (bool)obj.GetValue(IsRegexProperty);
        public static void SetIsRegex(DependencyObject obj, bool value) => obj.SetValue(IsRegexProperty, value);

        public static readonly DependencyProperty IsRegexProperty = DependencyProperty.RegisterAttached("IsRegex", typeof(bool), typeof(HighlightableTextBlock), new PropertyMetadata(false, Refresh));

        #endregion

        #region Methods

        private static void Refresh(DependencyObject d, DependencyPropertyChangedEventArgs e) => Highlight(d as TextBlock);

        private static void Highlight(TextBlock textblock)
        {
            if (textblock == null) return;

            string text = textblock.Text;

            if (textblock.GetBindingExpression(InternalTextProperty) == null)
            {
                var textBinding = textblock.GetBindingExpression(TextBlock.TextProperty);

                if (textBinding != null)
                {
                    textblock.SetBinding(InternalTextProperty, textBinding.ParentBindingBase);

                    var propertyDescriptor = DependencyPropertyDescriptor.FromProperty(TextBlock.TextProperty, typeof(TextBlock));

                    propertyDescriptor.RemoveValueChanged(textblock, OnTextChanged);
                }
                else
                {
                    var propertyDescriptor = DependencyPropertyDescriptor.FromProperty(TextBlock.TextProperty, typeof(TextBlock));

                    propertyDescriptor.AddValueChanged(textblock, OnTextChanged);

                    textblock.Unloaded -= Textblock_Unloaded;
                    textblock.Unloaded += Textblock_Unloaded;
                }
            }

            if (!string.IsNullOrEmpty(text))
            {
                SetIsBusy(textblock, true);

                string toHighlight = GetHightlightText(textblock);

                if (!string.IsNullOrEmpty(toHighlight))
                {
                    textblock.Inlines.Clear();

                    var highlightBrush = GetHighlightBrush(textblock);
                    var highlightTextBrush = GetHighlightTextBrush(textblock);

                    foreach (var match in GetMatches())
                    {
                        if (match.Match)
                        {
                            var formattedText = new Run(match.Text)
                            {
                                Background = highlightBrush,
                                Foreground = highlightTextBrush,
                            };

                            formattedText.FontWeight = GetFontWeight(textblock);
                            formattedText.FontStyle = GetFontStyle(textblock);
                            if (GetUnderline(textblock)) formattedText.TextDecorations.Add(TextDecorations.Underline);

                            textblock.Inlines.Add(formattedText);
                        }
                        else textblock.Inlines.Add(match.Text);
                    }

                    IEnumerable<(string Text, bool Match)> GetMatches()
                    {
                        MatchCollection matches = null;
                        try { matches = Regex.Matches(text, GetIsRegex(textblock) ? toHighlight : $"({Regex.Escape(toHighlight)})", GetCaseSensitive(textblock) ? RegexOptions.None : RegexOptions.IgnoreCase); }
                        catch (ArgumentException) { }

                        if (matches == null)
                        {
                            yield return (text, false);
                            yield break;
                        }

                        var previous = Match.Empty;
                        for (int i = 0; i < matches.Count; i++)
                        {
                            var match = matches[i];
                            int previousLastIndex = previous.Index + previous.Length;
                            if (previousLastIndex < match.Index) yield return (text.Substring(previousLastIndex, match.Index - previousLastIndex), false);
                            if (match.Length > 0) yield return (text.Substring(match.Index, match.Length), true);
                            previous = match;
                        }
                        int lastIndex = previous.Index + previous.Length;
                        if (lastIndex < text.Length) yield return (text.Substring(lastIndex, text.Length - lastIndex), false);
                    }
                }
                else
                {
                    textblock.Inlines.Clear();
                    textblock.SetCurrentValue(TextBlock.TextProperty, text);
                }

                SetIsBusy(textblock, false);
            }
        }

        private static void Textblock_Unloaded(object sender, RoutedEventArgs e)
        {
            var propertyDescriptor = DependencyPropertyDescriptor.FromProperty(TextBlock.TextProperty, typeof(TextBlock));

            propertyDescriptor.RemoveValueChanged(sender as TextBlock, OnTextChanged);
        }

        private static void OnTextChanged(object sender, EventArgs e)
        {
            if (sender is TextBlock textBlock &&
                !GetIsBusy(textBlock))
            {
                Highlight(textBlock);
            }
        }

        #endregion
    }
}
