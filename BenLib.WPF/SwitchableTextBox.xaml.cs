using BenLib.Framework;
using BenLib.Standard;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
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
        private bool m_clicked;
        private Point m_previousPosition;

        public double DragValue { get; private set; }
        public bool IsDragging { get; private set; }

        public Regex Regex { get => tb.Regex; set => tb.Regex = value; }
        public ContentType ContentType { get => tb.ContentType; set => tb.ContentType = value; }
        public ICollection<string> ForbiddenStrings { get => tb.ForbiddenStrings; set => tb.ForbiddenStrings = value; }
        public ICollection<string> AllowedStrings { get => tb.AllowedStrings; set => tb.AllowedStrings = value; }

        public Brush SBorderBrush { get => (Brush)GetValue(SBorderBrushProperty); set => SetValue(SBorderBrushProperty, value); }
        public static readonly DependencyProperty SBorderBrushProperty = DependencyProperty.Register("SBorderBrush", typeof(Brush), typeof(SwitchableTextBox), new PropertyMetadata(SystemColors.ActiveBorderBrush, (d, e) => { if (d is SwitchableTextBox switchableTextBox && switchableTextBox.IsMouseOver) switchableTextBox.bd.BorderBrush = (Brush)e.NewValue; }));

        public Brush SBackground { get => (Brush)GetValue(SBackgroundProperty); set => SetValue(SBackgroundProperty, value); }
        public static readonly DependencyProperty SBackgroundProperty = DependencyProperty.Register("SBackground", typeof(Brush), typeof(SwitchableTextBox), new PropertyMetadata(Brushes.White, (d, e) => { if (d is SwitchableTextBox switchableTextBox) switchableTextBox.tb.Background = (Brush)e.NewValue; }));

        public Brush SForeground { get => (Brush)GetValue(SForegroundProperty); set => SetValue(SForegroundProperty, value); }
        public static readonly DependencyProperty SForegroundProperty = DependencyProperty.Register("SForeground", typeof(Brush), typeof(SwitchableTextBox), new PropertyMetadata(Brushes.Black, (d, e) => { if (d is SwitchableTextBox switchableTextBox) switchableTextBox.tb.Foreground = switchableTextBox.lb.Foreground = (Brush)e.NewValue; }));

        public string Text { get => (string)GetValue(TextProperty); set => SetValue(TextProperty, value); }
        public static readonly DependencyProperty TextProperty = DependencyProperty.Register("Text", typeof(string), typeof(SwitchableTextBox), new PropertyMetadata((d, e) => { if (d is SwitchableTextBox switchableTextBox && e.NewValue is string s) switchableTextBox.SetText(s); }));

        public bool Resistant { get => (bool)GetValue(ResistantProperty); set => SetValue(ResistantProperty, value); }
        public static readonly DependencyProperty ResistantProperty = DependencyProperty.Register("Resistant", typeof(bool), typeof(SwitchableTextBox));

        public bool Drag { get => (bool)GetValue(DragProperty); set => SetValue(DragProperty, value); }
        public static readonly DependencyProperty DragProperty = DependencyProperty.Register("Drag", typeof(bool), typeof(SwitchableTextBox));

        public bool CancelWhenEmpty { get => (bool)GetValue(CancelWhenEmptyProperty); set => SetValue(CancelWhenEmptyProperty, value); }
        public static readonly DependencyProperty CancelWhenEmptyProperty = DependencyProperty.Register("CancelWhenEmpty", typeof(bool), typeof(SwitchableTextBox), new PropertyMetadata(true));

        public Predicate<string> TextValidation { get => (Predicate<string>)GetValue(TextValidationProperty); set => SetValue(TextValidationProperty, value); }
        public static readonly DependencyProperty TextValidationProperty = DependencyProperty.Register("TextValidation", typeof(Predicate<string>), typeof(SwitchableTextBox));

        public TypedTextBox TextBox => tb;
        public TextBlock TextBlock => lb;

        public event EventHandler Activated;
        public event EventHandler<EventArgs<IInputElement>> Desactivated;

        public bool IsActivated => tb.Visibility == Visibility.Visible;

        #endregion

        #region Constructeur

        public SwitchableTextBox()
        {
            InitializeComponent();
            SetText(Text);
        }

        #endregion

        #region Méthodes

        public bool Activate() => Activate(true);
        public bool Desactivate(IInputElement newFocus) => Desactivate(newFocus, true);

        private void ActivateOver()
        {
            bd.Background = SBackground;
            bd.BorderBrush = SBorderBrush;
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

        private void SetText(string text)
        {
            if (m_checkedText == text) tb.Text = lb.Text = text;
            else
            {
                switch (CheckText(text))
                {
                    case true:
                        tb.Text = lb.Text = text;
                        break;
                    case false:
                        Activate(false);
                        break;
                    default:
                        tb.Text = lb.Text = m_tmp;
                        break;
                }
            }
        }
        private bool? CheckText(string text)
        {
            if (text.IsNullOrEmpty() && CancelWhenEmpty) return null;
            else
            {
                m_isTextChecking = true;
                if ((AllowedStrings?.Contains(text) ?? false) || (TextValidation?.Invoke(text) ?? true))
                {
                    m_checkedText = text;
                    m_isTextChecking = false;
                    return true;
                }
                else return false;
            }
        }

        protected virtual void OnIncrement(double value) { }

        protected override void OnPropertyChanged(DependencyPropertyChangedEventArgs e)
        {
            if (e.Property == SBorderBrushProperty) { if (IsMouseOver) bd.BorderBrush = (Brush)e.NewValue; }
            else if (e.Property == SBackgroundProperty) { if (IsMouseOver) tb.Background = (Brush)e.NewValue; }
            else if (e.Property == SForegroundProperty) { if (IsMouseOver) tb.Foreground = lb.Foreground = (Brush)e.NewValue; }
            else if (e.Property == TextProperty) SetText((string)e.NewValue);
            base.OnPropertyChanged(e);
        }

        #endregion

        #region Events

        private void Lbc_MouseEnter(object sender, MouseEventArgs e) { if (!Resistant) ActivateOver(); }
        private void Lbc_MouseLeave(object sender, MouseEventArgs e) => DesactivateOver();

        private void Lbc_KeyDown(object sender, KeyEventArgs e) { if (e.Key == Key.Enter) Activate(); }

        private void Lbc_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (Drag && !IsDragging && e.OnlyPressed(MouseButton.Left))
            {
                lbc.CaptureMouse();
                m_previousPosition = e.GetPosition(lbc);
                m_clicked = true;
            }
        }

        private void Lbc_MouseMove(object sender, MouseEventArgs e)
        {
            if (m_clicked)
            {
                m_clicked = false;
                Cursor = Cursors.SizeWE;
                IsDragging = true;
            }
            if (IsDragging)
            {
                var position = e.GetPosition(lbc);
                var offset = position - m_previousPosition;
                double mult = 1.0;
                var k = Keyboard.Modifiers;
                if (k.HasFlag(ModifierKeys.Shift)) mult *= 10.0;
                if (k.HasFlag(ModifierKeys.Control)) mult *= 0.1;
                if (k.HasFlag(ModifierKeys.Alt)) mult *= 0.01;
                double value = mult * (offset.X - offset.Y);
                DragValue += value;
                OnIncrement(value);
                m_previousPosition = position;
            }
        }

        private void Lbc_MouseUp(object sender, MouseButtonEventArgs e)
        {
            m_clicked = false;
            lbc.ReleaseMouseCapture();
            if (IsDragging)
            {
                Cursor = Cursors.Arrow;
                DragValue = 0;
                IsDragging = false;
            }
            else if (!Resistant)
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
