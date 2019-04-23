using System;
using System.Collections.Generic;
using System.ComponentModel;
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
        private bool m_isTextChecking;

        public ContentTypes ContentType { get => TypedTextBox.GetContentType(tb); set => TypedTextBox.SetContentType(tb, value); }
        public ICollection<string> ForbiddenStrings { get => TypedTextBox.GetForbiddenStrings(tb); set => TypedTextBox.SetForbiddenStrings(tb, value); }
        public ICollection<string> AllowedStrings { get => TypedTextBox.GetAllowedStrings(tb); set => TypedTextBox.SetAllowedStrings(tb, value); }

        public Brush SBorderBrush { get; set; }

        public Brush SBackground
        {
            get => (Brush)GetValue(SBackgroundProperty);
            set => SetValue(SBackgroundProperty, value);
        }
        public static readonly DependencyProperty SBackgroundProperty = DependencyProperty.Register("SBackground", typeof(Brush), typeof(SwitchableTextBox), new PropertyMetadata(Brushes.White, OnSBackgroundChanged));

        public Brush SForeground
        {
            get => (Brush)GetValue(SForegroundProperty);
            set => SetValue(SForegroundProperty, value);
        }
        public static readonly DependencyProperty SForegroundProperty = DependencyProperty.Register("SForeground", typeof(Brush), typeof(SwitchableTextBox), new PropertyMetadata(Brushes.Black, OnSForegroundChanged));

        private static void OnSBackgroundChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) { if (d is SwitchableTextBox switchableTextBox) switchableTextBox.tb.Background = (Brush)e.NewValue; }
        private static void OnSForegroundChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) { if (d is SwitchableTextBox switchableTextBox) switchableTextBox.tb.Foreground = switchableTextBox.lb.Foreground = (Brush)e.NewValue; }

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

        public bool Resistant
        {
            get => (bool)GetValue(ResistantProperty);
            set => SetValue(ResistantProperty, value);
        }
        public static readonly DependencyProperty ResistantProperty = DependencyProperty.Register("Resistant", typeof(bool), typeof(SwitchableTextBox));

        public bool Empty { get => Text.IsNullOrEmpty(); set => Text = string.Empty; }

        public bool CancelWhenEmpty { get; set; }

        public Predicate<string> CoerceText { get; set; }

        public TextBox TextBox => tb;

        public event EventHandler Activated;
        public event EventHandler<EventArgs<IInputElement>> Desactivated;

        public bool IsActivated => tb.Visibility == Visibility.Visible;

        #endregion

        #region Constructeur

        public SwitchableTextBox()
        {
            InitializeComponent();
            Text = string.Empty;
            DependencyPropertyDescriptor.FromProperty(TextBlock.TextProperty, typeof(TextBlock)).AddValueChanged(lb, (sender, e) => SetValue(TextProperty, lb.Text));
            DependencyPropertyDescriptor.FromProperty(FinalTextProperty, typeof(SwitchableTextBox)).AddValueChanged(this, (sender, e) =>
            {
                tb.Text = FinalText;
                SetText();
            });
        }

        #endregion

        #region Méthodes

        public bool Activate() => Activate(true);
        public bool Desactivate(IInputElement newFocus) => Desactivate(newFocus, true);

        private void ActivateOver()
        {
            bd.Background = SBackground ?? Brushes.White;
            bd.BorderBrush = SBorderBrush ?? SystemColors.ActiveBorderBrush;
        }
        private void DesactivateOver()
        {
            bd.Background = Brushes.Transparent;
            bd.BorderBrush = Brushes.Transparent;
        }

        private void ActivateUI()
        {
            tb.Visibility = Visibility.Visible;
            bd.Visibility = Visibility.Hidden;
            lb.Visibility = Visibility.Hidden;
        }
        private void DesactivateUI()
        {
            tb.Visibility = Visibility.Hidden;
            bd.Visibility = Visibility.Visible;
            lb.Visibility = Visibility.Visible;
        }

        private bool Activate(bool notify)
        {
            ActivateUI();
            tb.Focus();
            if (tb.IsKeyboardFocused)
            {
                m_tmp = Text;
                tb.SelectAll();
                m_isTextChecking = false;
                if (notify) Activated?.Invoke(this, EventArgs.Empty);
                return true;
            }
            else
            {
                DesactivateUI();
                return false;
            }
        }
        private bool Desactivate(IInputElement newFocus, bool notify)
        {
            if (m_isTextChecking) return false;
            else if (SetText())
            {
                DesactivateUI();
                var focus = newFocus ?? lbc;
                if (!focus.Focusable) focus = lbc;
                focus.Focus();
                if (notify) Desactivated?.Invoke(this, EventArgsHelper.Create(newFocus));
                return true;
            }
            else
            {
                Activate(false);
                return false;
            }
        }

        private bool SetText()
        {
            m_isTextChecking = true;
            if (!Empty) return !(m_isTextChecking = !(AllowedStrings?.Contains(Text) ?? false) && !(CoerceText?.Invoke(Text) ?? true));
            else if (CancelWhenEmpty) Text = m_tmp;

            FinalText = lb.Text;
            return !(m_isTextChecking = true);
        }

        #endregion

        #region Events

        private void Lbc_MouseEnter(object sender, MouseEventArgs e) { if (!Resistant) ActivateOver(); }
        private void Lbc_MouseLeave(object sender, MouseEventArgs e) => DesactivateOver();

        private void Lbc_KeyDown(object sender, KeyEventArgs e) { if (e.Key == Key.Enter) Activate(); }

        private void Lbc_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (!Resistant)
            {
                Activate();
                e.Handled = true;
            }
        }
        private void Lbc_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (Resistant)
            {
                Activate();
                e.Handled = true;
            }
        }

        private void Tb_TextChanged(object sender, TextChangedEventArgs e) => lb.Text = tb.Text;

        private void Tb_KeyDown(object sender, KeyEventArgs e) { if (e.Key == Key.Enter) Desactivate(null); }

        private void Tb_LostKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            if (e.NewFocus == null || e.NewFocus == lbc || e.NewFocus is ContextMenu) return;
            if (!Desactivate(e.NewFocus)) e.Handled = true;
        }

        #endregion
    }
}
