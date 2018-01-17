using System;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Collections.Generic;
using System.Linq;
using BenLib;

namespace BenLib.WPF
{
    public sealed class TypedTextBox
    {
        /// <summary>
        /// Type de contenu de la <see cref='TextBox'/>.
        /// </summary>
        public static ContentTypes GetContentType(TextBox textBox) => (ContentTypes)textBox.GetValue(ContentTypeProperty);
        public static void SetContentType(TextBox textBox, ContentTypes value) => textBox.SetValue(ContentTypeProperty, value);

        public static readonly DependencyProperty ContentTypeProperty = DependencyProperty.RegisterAttached("ContentType", typeof(ContentTypes), typeof(TypedTextBox), new UIPropertyMetadata(ContentTypes.Text, ContentTypeChanged));

        private ContentTypes ContentType { get => (ContentTypes)m_textBox.GetValue(ContentTypeProperty); set => m_textBox.SetValue(ContentTypeProperty, value); }

        /// <summary>
        /// Contient les chaînes qu'il est interdit d'écrire.
        /// </summary>
        public static IEnumerable<string> GetForbiddenStrings(TextBox textBox) => (IEnumerable<string>)textBox.GetValue(ForbiddenStringsProperty);
        public static void SetForbiddenStrings(TextBox textBox, IEnumerable<string> value) => textBox.SetValue(ForbiddenStringsProperty, value);

        public static readonly DependencyProperty ForbiddenStringsProperty = DependencyProperty.RegisterAttached("ForbiddenStrings", typeof(IEnumerable<string>), typeof(TypedTextBox), new UIPropertyMetadata(null, ForbiddenStringsChanged));

        private IEnumerable<string> ForbiddenStrings { get => (IEnumerable<string>)m_textBox.GetValue(ForbiddenStringsProperty); set => m_textBox.SetValue(ForbiddenStringsProperty, value); }

        /// <summary>
        /// Contient les chaînes qu'il est interdit d'écrire.
        /// </summary>
        public static IEnumerable<string> GetAllowedStrings(TextBox textBox) => (IEnumerable<string>)textBox.GetValue(AllowedStringsProperty);
        public static void SetAllowedStrings(TextBox textBox, IEnumerable<string> value) => textBox.SetValue(AllowedStringsProperty, value);

        public static readonly DependencyProperty AllowedStringsProperty = DependencyProperty.RegisterAttached("AllowedStrings", typeof(IEnumerable<string>), typeof(TypedTextBox));

        private IEnumerable<string> AllowedStrings { get => (IEnumerable<string>)m_textBox.GetValue(AllowedStringsProperty); set => m_textBox.SetValue(AllowedStringsProperty, value); }

        private static readonly Dictionary<TextBox, TypedTextBox> m_attachedControls = new Dictionary<TextBox, TypedTextBox>();

        private readonly TextBox m_textBox;

        private string m_ante;
        private int m_sst;
        private int m_slt;
        private bool m_txtforcechange;
        private bool m_sstforcechange;

        private TypedTextBox(TextBox textBox)
        {
            m_textBox = textBox;
            if (m_textBox.IsLoaded) Register();
            else m_textBox.Loaded += TextBox_Loaded;
        }

        private static void ContentTypeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is TextBox textBox)
            {
                if ((ContentTypes)e.NewValue != ContentTypes.Text) //Register
                {
                    if (!m_attachedControls.ContainsKey(textBox))
                        m_attachedControls.Add(textBox, new TypedTextBox(textBox));
                }
                else if (GetForbiddenStrings(textBox).IsNullOrEmpty()) //Unregister
                {
                    if (m_attachedControls.TryGetValue(textBox, out TypedTextBox typedTextBox))
                    {
                        m_attachedControls.Remove(textBox);
                        typedTextBox.UnRegister();
                    }
                }
            }
        }

        private static void ForbiddenStringsChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is TextBox textBox)
            {
                if (!((IEnumerable<string>)e.NewValue).IsNullOrEmpty()) //Register
                {
                    if (!m_attachedControls.ContainsKey(textBox))
                        m_attachedControls.Add(textBox, new TypedTextBox(textBox));
                }
                else if (GetContentType(textBox) == ContentTypes.Text) //Unregister
                {
                    if (m_attachedControls.TryGetValue(textBox, out TypedTextBox typedTextBox))
                    {
                        m_attachedControls.Remove(textBox);
                        typedTextBox.UnRegister();
                    }
                }
            }
        }

        private void TextBox_Loaded(object sender, RoutedEventArgs e)
        {
            if (Register()) m_textBox.Loaded -= TextBox_Loaded;
        }

        private bool Register()
        {
            if (m_textBox != null)
            {
                m_textBox.TextChanged += TextBox_TextChanged;
                m_textBox.SelectionChanged += TextBox_SelectionChanged;
                m_ante = m_textBox.Text;
                m_sst = m_textBox.SelectionStart;
                m_slt = m_textBox.SelectionLength;
                return true;
            }
            else return false;
        }

        private void UnRegister()
        {
            m_textBox.TextChanged -= TextBox_TextChanged;
            m_textBox.SelectionChanged -= TextBox_SelectionChanged;
        }

        private void TextBox_SelectionChanged(object sender, RoutedEventArgs e)
        {
            if (!m_sstforcechange)
            {
                if (m_txtforcechange)
                {
                    m_sstforcechange = true;

                    m_textBox.SelectionStart = m_sst;
                    m_textBox.SelectionLength = m_slt;

                    m_sstforcechange = false;
                    m_txtforcechange = false;
                }
                else { m_sst = m_textBox.SelectionStart; m_slt = m_textBox.SelectionLength; }
            }
        }

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (!m_txtforcechange)
            {
                if (!m_textBox.Text.IsEmpty())
                {
                    if (AllowedStrings != null && AllowedStrings.Contains(m_textBox.Text)) goto End;

                    if (m_textBox.Text.Replace(ForbiddenStrings, String.Empty) != m_textBox.Text) { m_txtforcechange = true; m_textBox.Text = m_ante; }

                    switch (ContentType)
                    {
                        case ContentTypes.Integrer:
                            CheckNumText(Literal.PreviewIntegrer);
                            break;
                        case ContentTypes.UnsignedIntegrer:
                            CheckNumText(Literal.PreviewUnsignedIntegrer);
                            break;
                        case ContentTypes.Double:
                            CheckNumText(Literal.PreviewDouble);
                            break;
                        case ContentTypes.UnsignedDouble:
                            CheckNumText(Literal.PreviewUnsignedDouble);
                            break;
                    }
                }

                End:
                if (m_ante != m_textBox.Text)
                {
                    m_ante = m_textBox.Text;
                    //m_textBox.TextChanged?.Invoke(sender, e);
                }
            }
        }

        private void CheckNumText(Regex regex)
        {
            if (m_textBox.Text.Replace(" ", String.Empty) != m_textBox.Text)
            {
                m_txtforcechange = true;
                m_textBox.Text = m_textBox.Text.Replace(" ", String.Empty);
            }

            if (!regex.IsMatch(m_textBox.Text))
            {
                m_txtforcechange = true;
                m_textBox.Text = m_ante;
            }
        }
    }

    public enum ContentTypes { Text, Integrer, UnsignedIntegrer, Double, UnsignedDouble }
}
