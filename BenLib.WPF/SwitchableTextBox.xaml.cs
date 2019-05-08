using BenLib.Standard;
using System;
using System.Collections.Generic;
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
        private string m_checkedText;

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

        //public Point InRanderTransformOrigin
        //{
        //    get => (Point)GetValue(InRanderTransformOriginProperty);
        //    set => SetValue(InRanderTransformOriginProperty, value);
        //}
        //public static readonly DependencyProperty InRanderTransformOriginProperty = DependencyProperty.Register("InRanderTransformOrigin", typeof(Point), typeof(SwitchableTextBox), new PropertyMetadata(OnTranformOriginChanged));

        //public Transform InRanderTransform
        //{
        //    get => (Transform)GetValue(InRanderTransformProperty);
        //    set => SetValue(InRanderTransformProperty, value);
        //}
        //public static readonly DependencyProperty InRanderTransformProperty = DependencyProperty.Register("InRanderTransform", typeof(Transform), typeof(SwitchableTextBox), new PropertyMetadata(OnTransformChanged));

        //private static void OnTranformOriginChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) { if (d is SwitchableTextBox switchableTextBox && e.NewValue is Point origin) switchableTextBox.grid.RenderTransformOrigin = origin; }
        //private static void OnTransformChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) { if (d is SwitchableTextBox switchableTextBox && e.NewValue is Transform transform) switchableTextBox.grid.RenderTransform = transform; }
        private static void OnSBackgroundChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) { if (d is SwitchableTextBox switchableTextBox) switchableTextBox.tb.Background = (Brush)e.NewValue; }
        private static void OnSForegroundChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) { if (d is SwitchableTextBox switchableTextBox) switchableTextBox.tb.Foreground = switchableTextBox.lb.Foreground = (Brush)e.NewValue; }

        public string Text
        {
            get => (string)GetValue(TextProperty);
            set => SetValue(TextProperty, value);
        }
        public static readonly DependencyProperty TextProperty = DependencyProperty.Register("Text", typeof(string), typeof(SwitchableTextBox), new PropertyMetadata(OnTextChanged));

        private static void OnTextChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is SwitchableTextBox switchableTextBox && e.NewValue is string s)
            {
                if (switchableTextBox.m_checkedText == s) switchableTextBox.tb.Text = switchableTextBox.lb.Text = s;
                else switch (switchableTextBox.CheckText(s))
                {
                    case true:
                        switchableTextBox.tb.Text = switchableTextBox.lb.Text = s;
                        break;
                    case false:
                        switchableTextBox.Activate(false);
                        break;
                    default:
                        switchableTextBox.tb.Text = switchableTextBox.lb.Text = switchableTextBox.m_tmp;
                        break;
                }
            }
        }

        public bool Resistant
        {
            get => (bool)GetValue(ResistantProperty);
            set => SetValue(ResistantProperty, value);
        }
        public static readonly DependencyProperty ResistantProperty = DependencyProperty.Register("Resistant", typeof(bool), typeof(SwitchableTextBox));

        public bool IsEmpty { get => Text.IsNullOrEmpty(); set => Text = string.Empty; }
        public bool CancelWhenEmpty { get; set; }

        public Predicate<string> CoerceText { get; set; }

        public TextBox TextBox => tb;

        public event EventHandler Activated;
        public event EventHandler<EventArgs<IInputElement>> Desactivated;

        public bool IsActivated => tb.Visibility == Visibility.Visible;

        #endregion

        #region Constructeur

        public SwitchableTextBox() => InitializeComponent();

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
            bd.Visibility = Visibility.Collapsed;
            lb.Visibility = Visibility.Collapsed;
        }
        private void DesactivateUI()
        {
            tb.Visibility = Visibility.Collapsed;
            bd.Visibility = Visibility.Visible;
            lb.Visibility = Visibility.Visible;
        }

        private bool Activate(bool notify)
        {
            ActivateUI();
            tb.Focus();
            if (tb.IsKeyboardFocused)
            {
                m_checkedText = m_tmp = Text;
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
            if (CheckText(tb.Text) != false)
            {
                Text = m_checkedText;
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

        private bool? CheckText(string text)
        {
            if (text.IsNullOrEmpty() && CancelWhenEmpty) return null;
            else
            {
                m_isTextChecking = true;
                if ((AllowedStrings?.Contains(text) ?? false) || (CoerceText?.Invoke(text) ?? true))
                {
                    m_checkedText = text;
                    m_isTextChecking = false;
                    return true;
                }
                else return false;
            }
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

        private void Tb_KeyDown(object sender, KeyEventArgs e) { if (e.Key == Key.Enter) Desactivate(null); }
        private void Tb_LostKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            if (e.NewFocus == null || e.NewFocus == lbc || e.NewFocus is ContextMenu) return;
            if (!Desactivate(e.NewFocus)) e.Handled = true;
        }

        #endregion
    }
}
