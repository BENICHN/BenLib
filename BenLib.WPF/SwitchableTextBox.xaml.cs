using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace BenLib.WPF
{
    /// <summary>
    /// Logique d'interaction pour SwitchableTextBox.xaml
    /// </summary>
    public partial class SwitchableTextBox : UserControl
    {
        #region Champs & Propriétés

        private string m_tmp;
        private Brush m_sBackground;
        private Brush m_sForeground;

        /// <summary>
        /// Type de contenu de la <see cref='SwitchableTextBox'/>.
        /// </summary>
        public ContentTypes ContentType { get => TypedTextBox.GetContentType(tb); set => TypedTextBox.SetContentType(tb, value); }
        public IEnumerable<string> ForbiddenStrings { get => TypedTextBox.GetForbiddenStrings(tb); set => TypedTextBox.SetForbiddenStrings(tb, value); }
        public IEnumerable<string> AllowedStrings { get => TypedTextBox.GetAllowedStrings(tb); set => TypedTextBox.SetAllowedStrings(tb, value); }

        public Brush SBackground { get => m_sBackground; set => m_sBackground = tb.Background = value; }
        public Brush SForeground { get => m_sForeground; set => m_sForeground = tb.Foreground = lb.Foreground = value; }
        public Brush SBorderBrush { get; set; }

        /// <summary>
        /// Contenu de la <see cref='SwitchableTextBox'/>.
        /// </summary>
        public string Text
        {
            get => (string)GetValue(TextProperty);
            set => tb.Text = value;
        }

        public static readonly DependencyProperty TextProperty = DependencyProperty.Register("Text", typeof(string), typeof(SwitchableTextBox));

        public string FinalText
        {
            get => (string)GetValue(FinalTextProperty);
            set => SetValue(FinalTextProperty, value);
        }

        public static readonly DependencyProperty FinalTextProperty = DependencyProperty.Register("FinalText", typeof(string), typeof(SwitchableTextBox));

        /// <summary>
        /// Indique si <see cref='Text'/> est vide.
        /// </summary>
        public bool Empty { get => Text.IsNullOrEmpty(); set => Text = string.Empty; }

        /// <summary>
        /// Indique si le changement de texte doit être annulé quand celui-ci est vide.
        /// </summary>
        public bool CancelWhenEmpty { get; set; }

        public TextBox TextBox => tb;

        #endregion

        #region Constructeur

        public SwitchableTextBox()
        {
            InitializeComponent();
            Text = string.Empty;
            //Mouse.Capture(this, CaptureMode.SubTree);
            //AddHandler(Mouse.PreviewMouseDownOutsideCapturedElementEvent, new MouseButtonEventHandler(HandleClickOutsideOfControl), true);
            DependencyPropertyDescriptor.FromProperty(TextBlock.TextProperty, typeof(TextBlock)).AddValueChanged(lb, (sender, e) => { SetValue(TextProperty, lb.Text); });
            DependencyPropertyDescriptor.FromProperty(FinalTextProperty, typeof(SwitchableTextBox)).AddValueChanged(this, (sender, e) =>
            {
                tb.Text = FinalText;
                SetText();
            });
        }

        #endregion

        #region Events

        /*private void HandleClickOutsideOfControl(object sender, MouseButtonEventArgs e)
        {
            Focus();
            gd.Focus();
            ReleaseMouseCapture();
        }*/

        private void tb_TextChanged(object sender, TextChangedEventArgs e) => lb.Text = tb.Text;

        private void lb_MouseEnter(object sender, MouseEventArgs e)
        {
            bd.Background = SBackground ?? Brushes.White;
            bd.BorderBrush = SBorderBrush ?? SystemColors.ActiveBorderBrush;
        }

        private void lb_MouseDown(object sender, MouseButtonEventArgs e)
        {
            tb.Visibility = Visibility.Visible;
            bd.Visibility = Visibility.Hidden;
            lb.Visibility = Visibility.Hidden;
            m_tmp = Text;
            tb.Focus();
            tb.SelectAll();
            e.Handled = true;
        }

        private void lb_MouseLeave(object sender, MouseEventArgs e)
        {
            bd.Background = Brushes.Transparent;
            bd.BorderBrush = Brushes.Transparent;
        }

        private void tb_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter) gd.Focus();
        }

        private void tb_PreviewLostKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            if (e.NewFocus is ContextMenu) return;

            bool ReFocus = !SetText();

            if (!ReFocus)
            {
                tb.Visibility = Visibility.Hidden;
                bd.Visibility = Visibility.Visible;
                lb.Visibility = Visibility.Visible;
            }
            else
            {
                tb.Focus();
                e.Handled = true;
            }
        }

        private bool SetText()
        {
            if (!Empty)
            {
                switch (ContentType)
                {
                    case ContentTypes.Integer:
                    case ContentTypes.UnsignedInteger:
                        {
                            try
                            {
                                Text = int.Parse(Text).ToString();
                            }
                            catch (Exception ex)
                            {
                                if (!AllowedStrings.Contains(Text))
                                {
                                    MessageBox.Show(ex.Message, string.Empty, MessageBoxButton.OK, MessageBoxImage.Error);
                                    return false;
                                }
                            }
                        }
                        break;

                    case ContentTypes.Double:
                    case ContentTypes.UnsignedDouble:
                        {
                            try
                            {
                                Text = double.Parse(Text.Replace(',', '.'), Literal.DecimalSeparatorPoint).ToString();
                            }
                            catch (Exception ex)
                            {
                                try { Text = double.Parse(Text).ToString(); }
                                catch
                                {

                                    if (!AllowedStrings.Contains(Text))
                                    {
                                        MessageBox.Show(ex.Message, string.Empty, MessageBoxButton.OK, MessageBoxImage.Error);
                                        return false;
                                    }
                                }
                            }
                        }
                        break;
                }
            }
            else if (CancelWhenEmpty) Text = m_tmp;

            SetValue(FinalTextProperty, lb.Text);
            return true;
        }

        private void gd_GotFocus(object sender, RoutedEventArgs e) => RaiseEvent(new RoutedEventArgs(LostFocusEvent, this));

        #endregion
    }
}
