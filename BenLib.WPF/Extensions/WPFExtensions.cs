using System.Windows;
using System.Collections.Generic;
using System.Windows.Input;

namespace BenLib.WPF
{
    public sealed class WPFExtensions
    {
        public static bool GetEnabled(FrameworkElement element) => (bool)element.GetValue(EnabledProperty);
        public static void SetEnabled(FrameworkElement element, bool value) => element.SetValue(EnabledProperty, value);

        public static readonly DependencyProperty EnabledProperty = DependencyProperty.RegisterAttached("Enabled", typeof(bool), typeof(WPFExtensions), new UIPropertyMetadata(EnabledChanged));

        private bool Enabled { get => (bool)m_element.GetValue(EnabledProperty); set => m_element.SetValue(EnabledProperty, value); }

        public static bool GetIsPressed(FrameworkElement element) => (bool)element.GetValue(IsPressedProperty);
        public static void SetIsPressed(FrameworkElement element, bool value) => element.SetValue(IsPressedProperty, value);

        public static readonly DependencyProperty IsPressedProperty = DependencyProperty.RegisterAttached("IsPressed", typeof(bool), typeof(WPFExtensions));

        private bool IsPressed { get => (bool)m_element.GetValue(IsPressedProperty); set => m_element.SetValue(IsPressedProperty, value); }

        private static readonly Dictionary<FrameworkElement, WPFExtensions> m_attachedControls = new Dictionary<FrameworkElement, WPFExtensions>();

        private readonly FrameworkElement m_element;

        private WPFExtensions(FrameworkElement element)
        {
            m_element = element;
            if (m_element.IsLoaded) Register();
            else m_element.Loaded += Element_Loaded;
        }

        private void Element_Loaded(object sender, RoutedEventArgs e)
        {
            if (Register()) m_element.Loaded -= Element_Loaded;
        }

        private bool Register()
        {
            if (m_element != null)
            {
                m_element.PreviewMouseLeftButtonDown += Element_PreviewMouseDown;
                m_element.PreviewMouseLeftButtonUp += Element_PreviewMouseUp;
                m_element.MouseLeftButtonDown += Element_PreviewMouseDown;
                m_element.MouseLeftButtonUp += Element_PreviewMouseUp;
                return true;
            }
            else return false;
        }

        
        private void Element_PreviewMouseUp(object sender, MouseButtonEventArgs e)
        {
            IsPressed = false;
        }

        private void Element_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            IsPressed = true;
        }

        private void UnRegister()
        {
            m_element.PreviewMouseLeftButtonDown -= Element_PreviewMouseDown;
            m_element.PreviewMouseLeftButtonUp -= Element_PreviewMouseUp;
        }

        private static void EnabledChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is FrameworkElement element)
            {
                if ((bool)e.NewValue) //Register
                {
                    if (!m_attachedControls.ContainsKey(element)) m_attachedControls.Add(element, new WPFExtensions(element));
                }
                else //Unregister
                {
                    if (m_attachedControls.TryGetValue(element, out WPFExtensions extensions))
                    {
                        m_attachedControls.Remove(element);
                        extensions.UnRegister();
                    }
                }
            }
        }
    }
}
