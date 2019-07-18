using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;

namespace BenLib.WPF
{
    public sealed class GrabAnywhere
    {
        public static bool GetEnabled(Slider slider) => (bool)slider.GetValue(EnabledProperty);
        public static void SetEnabled(Slider slider, bool value) => slider.SetValue(EnabledProperty, value);

        public static readonly DependencyProperty EnabledProperty = DependencyProperty.RegisterAttached("Enabled", typeof(bool), typeof(GrabAnywhere), new UIPropertyMetadata(EnabledChanged));

        public bool Enabled { get => (bool)m_slider.GetValue(EnabledProperty); set => m_slider.SetValue(EnabledProperty, value); }

        private static readonly ConditionalWeakTable<Slider, GrabAnywhere> m_attachedControls = new ConditionalWeakTable<Slider, GrabAnywhere>();

        private readonly Slider m_slider;

        private GrabAnywhere(Slider slider)
        {
            m_slider = slider;
            if (m_slider.IsLoaded) Register();
            else m_slider.Loaded += Slider_Loaded;
        }

        ~GrabAnywhere() => UnRegister();

        private void Slider_Loaded(object sender, RoutedEventArgs e)
        {
            if (Register()) m_slider.Loaded -= Slider_Loaded;
        }

        private bool Register()
        {
            if (m_slider != null)
            {
                if (m_slider.Template?.FindName("PART_Track", m_slider) is Track track)
                {
                    track.Thumb.MouseEnter += Thumb_MouseEnter;
                    m_slider.IsMoveToPointEnabled = true;
                }
                else return false;

                return true;
            }
            else return false;
        }

        private void UnRegister() => (m_slider.Template.FindName("PART_Track", m_slider) as Track).Thumb.MouseEnter += Thumb_MouseEnter;

        private static void EnabledChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is Slider slider)
            {
                if ((bool)e.NewValue) //Register
                {
                    if (!m_attachedControls.TryGetValue(slider, out _)) m_attachedControls.Add(slider, new GrabAnywhere(slider));
                }
                else //Unregister
                {
                    if (m_attachedControls.TryGetValue(slider, out _))
                    {
                        m_attachedControls.Remove(slider);
                    }
                }
            }
        }

        private void Thumb_MouseEnter(object sender, MouseEventArgs e)
        {
            if (sender is Thumb thumb && e.LeftButton == MouseButtonState.Pressed && e.MouseDevice.Captured == null)
            {
                var args = new MouseButtonEventArgs(e.MouseDevice, e.Timestamp, MouseButton.Left) { RoutedEvent = UIElement.MouseLeftButtonDownEvent };
                thumb.RaiseEvent(args);
            }
        }
    }
}
