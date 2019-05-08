using BenLib.Standard;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;

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
        public static ICollection<string> GetForbiddenStrings(TextBox textBox) => (ICollection<string>)textBox.GetValue(ForbiddenStringsProperty);
        public static void SetForbiddenStrings(TextBox textBox, ICollection<string> value) => textBox.SetValue(ForbiddenStringsProperty, value);

        public static readonly DependencyProperty ForbiddenStringsProperty = DependencyProperty.RegisterAttached("ForbiddenStrings", typeof(ICollection<string>), typeof(TypedTextBox), new UIPropertyMetadata(null, ForbiddenStringsChanged));

        private ICollection<string> ForbiddenStrings { get => (ICollection<string>)m_textBox.GetValue(ForbiddenStringsProperty); set => m_textBox.SetValue(ForbiddenStringsProperty, value); }

        /// <summary>
        /// Contient les chaînes qu'il est interdit d'écrire.
        /// </summary>
        public static ICollection<string> GetAllowedStrings(TextBox textBox) => (ICollection<string>)textBox.GetValue(AllowedStringsProperty);
        public static void SetAllowedStrings(TextBox textBox, ICollection<string> value) => textBox.SetValue(AllowedStringsProperty, value);

        public static readonly DependencyProperty AllowedStringsProperty = DependencyProperty.RegisterAttached("AllowedStrings", typeof(ICollection<string>), typeof(TypedTextBox));

        private ICollection<string> AllowedStrings { get => (ICollection<string>)m_textBox.GetValue(AllowedStringsProperty); set => m_textBox.SetValue(AllowedStringsProperty, value); }

        private static readonly ConditionalWeakTable<TextBox, TypedTextBox> m_attachedControls = new ConditionalWeakTable<TextBox, TypedTextBox>();

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

        ~TypedTextBox() => UnRegister();

        private static void ContentTypeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is TextBox textBox)
            {
                if ((ContentTypes)e.NewValue != ContentTypes.Text) //Register
                {
                    if (!m_attachedControls.TryGetValue(textBox, out var _)) m_attachedControls.Add(textBox, new TypedTextBox(textBox));
                }
                else if (GetForbiddenStrings(textBox).IsNullOrEmpty()) //Unregister
                {
                    if (m_attachedControls.TryGetValue(textBox, out var typedTextBox))
                    {
                        m_attachedControls.Remove(textBox);
                    }
                }
            }
        }

        private static void ForbiddenStringsChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is TextBox textBox)
            {
                if (!((ICollection<string>)e.NewValue).IsNullOrEmpty()) //Register
                {
                    if (!m_attachedControls.TryGetValue(textBox, out var _)) m_attachedControls.Add(textBox, new TypedTextBox(textBox));
                }
                else if (GetContentType(textBox) == ContentTypes.Text) //Unregister
                {
                    if (m_attachedControls.TryGetValue(textBox, out var typedTextBox))
                    {
                        m_attachedControls.Remove(textBox);
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

                    if (m_textBox.Text.Replace(ForbiddenStrings, string.Empty) != m_textBox.Text) { m_txtforcechange = true; m_textBox.Text = m_ante; }

                    switch (ContentType)
                    {
                        case ContentTypes.Integer:
                            CheckNumText(Literal.PreviewInteger);
                            break;
                        case ContentTypes.UnsignedInteger:
                            CheckNumText(Literal.PreviewUnsignedInteger);
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
            if (m_textBox.Text.Replace(" ", string.Empty) != m_textBox.Text)
            {
                m_txtforcechange = true;
                m_textBox.Text = m_textBox.Text.Replace(" ", string.Empty);
            }

            if (!regex.IsMatch(m_textBox.Text))
            {
                m_txtforcechange = true;
                m_textBox.Text = m_ante;
            }
        }
    }

    public enum ContentTypes { Text, Integer, UnsignedInteger, Double, UnsignedDouble }
}
