using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Controls;

namespace BenLib.WPF
{
    public sealed class WPFExtensions
    {
        public static bool GetEnabled(FrameworkElement element) => (bool)element.GetValue(EnabledProperty);
        public static void SetEnabled(FrameworkElement element, bool value) => element.SetValue(EnabledProperty, value);

        public static readonly DependencyProperty EnabledProperty = DependencyProperty.RegisterAttached("Enabled", typeof(bool), typeof(WPFExtensions), new UIPropertyMetadata(EnabledChanged));

        public bool Enabled { get => (bool)m_element.GetValue(EnabledProperty); set => m_element.SetValue(EnabledProperty, value); }

        public static bool GetIsPressed(FrameworkElement element) => (bool)element.GetValue(IsPressedProperty);
        public static void SetIsPressed(FrameworkElement element, bool value) => element.SetValue(IsPressedProperty, value);

        public static readonly DependencyProperty IsPressedProperty = DependencyProperty.RegisterAttached("IsPressed", typeof(bool), typeof(WPFExtensions));

        public bool IsPressed { get => (bool)m_element.GetValue(IsPressedProperty); set => m_element.SetValue(IsPressedProperty, value); }

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

        private void Element_PreviewMouseUp(object sender, MouseButtonEventArgs e) => IsPressed = false;

        private void Element_PreviewMouseDown(object sender, MouseButtonEventArgs e) => IsPressed = true;

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
                    if (m_attachedControls.TryGetValue(element, out var extensions))
                    {
                        m_attachedControls.Remove(element);
                        extensions.UnRegister();
                    }
                }
            }
        }
    }

    public static class GridViewColumnHeaderProperties
    {
        public static Brush GetMouseOverBrush(GridViewColumnHeader obj) => (Brush) obj.GetValue(MouseOverBrushProperty);
        public static void SetMouseOverBrush(GridViewColumnHeader obj, Brush value) => obj.SetValue(MouseOverBrushProperty, value);

        public static readonly DependencyProperty MouseOverBrushProperty = DependencyProperty.RegisterAttached("MouseOverBrush", typeof(Brush), typeof(GridViewColumnHeaderProperties));

        public static Brush GetIsPressedBrush(GridViewColumnHeader obj) => (Brush) obj.GetValue(IsPressedBrushProperty);
        public static void SetIsPressedBrush(GridViewColumnHeader obj, Brush value) => obj.SetValue(IsPressedBrushProperty, value);

        public static readonly DependencyProperty IsPressedBrushProperty = DependencyProperty.RegisterAttached("IsPressedBrush", typeof(Brush), typeof(GridViewColumnHeaderProperties));
    }

    public static class TreeViewItemProperties
    {
        public static Brush GetMouseOverBrush(TreeViewItem obj) => (Brush) obj.GetValue(MouseOverBrushProperty);
        public static void SetMouseOverBrush(TreeViewItem obj, Brush value) => obj.SetValue(MouseOverBrushProperty, value);

        public static readonly DependencyProperty MouseOverBrushProperty = DependencyProperty.RegisterAttached("MouseOverBrush", typeof(Brush), typeof(TreeViewItemProperties));

        public static Brush GetIsSelectedBackgroundBrush(TreeViewItem obj) => (Brush) obj.GetValue(IsSelectedBackgroundBrushProperty);
        public static void SetIsSelectedBackgroundBrush(TreeViewItem obj, Brush value) => obj.SetValue(IsSelectedBackgroundBrushProperty, value);

        public static readonly DependencyProperty IsSelectedBackgroundBrushProperty = DependencyProperty.RegisterAttached("IsSelectedBackgroundBrush", typeof(Brush), typeof(TreeViewItemProperties));

        public static Brush GetIsSelectedForegroundBrush(TreeViewItem obj) => (Brush) obj.GetValue(IsSelectedForegroundBrushProperty);
        public static void SetIsSelectedForegroundBrush(TreeViewItem obj, Brush value) => obj.SetValue(IsSelectedForegroundBrushProperty, value);

        public static readonly DependencyProperty IsSelectedForegroundBrushProperty = DependencyProperty.RegisterAttached("IsSelectedForegroundBrush", typeof(Brush), typeof(TreeViewItemProperties));

        public static Brush GetIsSelectedInactiveBackgroundBrush(TreeViewItem obj) => (Brush) obj.GetValue(IsSelectedInactiveBackgroundBrushProperty);
        public static void SetIsSelectedInactiveBackgroundBrush(TreeViewItem obj, Brush value) => obj.SetValue(IsSelectedInactiveBackgroundBrushProperty, value);

        public static readonly DependencyProperty IsSelectedInactiveBackgroundBrushProperty = DependencyProperty.RegisterAttached("IsSelectedInactiveBackgroundBrush", typeof(Brush), typeof(TreeViewItemProperties));

        public static Brush GetIsSelectedInactiveForegroundBrush(TreeViewItem obj) => (Brush) obj.GetValue(IsSelectedInactiveForegroundBrushProperty);
        public static void SetIsSelectedInactiveForegroundBrush(TreeViewItem obj, Brush value) => obj.SetValue(IsSelectedInactiveForegroundBrushProperty, value);

        public static readonly DependencyProperty IsSelectedInactiveForegroundBrushProperty = DependencyProperty.RegisterAttached("IsSelectedInactiveForegroundBrush", typeof(Brush), typeof(TreeViewItemProperties));
    }

    public static class ListViewItemProperties
    {
        public static Brush GetMouseOverBrush(ListViewItem obj) => (Brush) obj.GetValue(MouseOverBrushProperty);
        public static void SetMouseOverBrush(ListViewItem obj, Brush value) => obj.SetValue(MouseOverBrushProperty, value);

        public static readonly DependencyProperty MouseOverBrushProperty = DependencyProperty.RegisterAttached("MouseOverBrush", typeof(Brush), typeof(ListViewItemProperties));

        public static Brush GetIsSelectedBackgroundBrush(ListViewItem obj) => (Brush) obj.GetValue(IsSelectedBackgroundBrushProperty);
        public static void SetIsSelectedBackgroundBrush(ListViewItem obj, Brush value) => obj.SetValue(IsSelectedBackgroundBrushProperty, value);

        public static readonly DependencyProperty IsSelectedBackgroundBrushProperty = DependencyProperty.RegisterAttached("IsSelectedBackgroundBrush", typeof(Brush), typeof(ListViewItemProperties));

        public static Brush GetIsSelectedForegroundBrush(ListViewItem obj) => (Brush) obj.GetValue(IsSelectedForegroundBrushProperty);
        public static void SetIsSelectedForegroundBrush(ListViewItem obj, Brush value) => obj.SetValue(IsSelectedForegroundBrushProperty, value);

        public static readonly DependencyProperty IsSelectedForegroundBrushProperty = DependencyProperty.RegisterAttached("IsSelectedForegroundBrush", typeof(Brush), typeof(ListViewItemProperties));
    }

    public class ResourceBinding : MarkupExtension
    {
        #region Helper properties

        public static object GetResourceBindingKeyHelper(DependencyObject obj) => obj.GetValue(ResourceBindingKeyHelperProperty);
        public static void SetResourceBindingKeyHelper(DependencyObject obj, object value) => obj.SetValue(ResourceBindingKeyHelperProperty, value);
        public static readonly DependencyProperty ResourceBindingKeyHelperProperty = DependencyProperty.RegisterAttached("ResourceBindingKeyHelper", typeof(object), typeof(ResourceBinding), new PropertyMetadata(null, ResourceKeyChanged));

        private static void ResourceKeyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (!(d is FrameworkElement target && e.NewValue is Tuple<object, DependencyProperty> newVal)) return;

            var dp = newVal.Item2;

            if (newVal.Item1 == null)
            {
                target.SetValue(dp, dp.GetMetadata(target).DefaultValue);
                return;
            }

            target.SetResourceReference(dp, newVal.Item1);
        }

        #endregion

        public ResourceBinding() { }

        public ResourceBinding(string path) => Path = new PropertyPath(path);

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            var provideValueTargetService = (IProvideValueTarget)serviceProvider.GetService(typeof(IProvideValueTarget));
            if (provideValueTargetService == null) return null;
            if (provideValueTargetService.TargetObject != null && provideValueTargetService.TargetObject.GetType().FullName == "System.Windows.SharedDp") return this;
            if (!(provideValueTargetService.TargetObject is FrameworkElement targetObject && provideValueTargetService.TargetProperty is DependencyProperty targetProperty)) return null;

            var binding = new Binding();

            #region binding

            binding.Path = Path;
            binding.XPath = XPath;
            binding.Mode = Mode;
            binding.UpdateSourceTrigger = UpdateSourceTrigger;
            binding.Converter = Converter;
            binding.ConverterParameter = ConverterParameter;
            binding.ConverterCulture = ConverterCulture;

            if (RelativeSource != null) binding.RelativeSource = RelativeSource;
            if (ElementName != null) binding.ElementName = ElementName;
            if (Source != null) binding.Source = Source;

            binding.FallbackValue = FallbackValue;

            #endregion

            var multiBinding = new MultiBinding
            {
                Converter = HelperConverter.Current,
                ConverterParameter = targetProperty
            };

            multiBinding.Bindings.Add(binding);

            multiBinding.NotifyOnSourceUpdated = true;

            targetObject.SetBinding(ResourceBindingKeyHelperProperty, multiBinding);

            return null;
        }

        #region Binding Members

        /// <summary> The source path (for CLR bindings).</summary>
        public object Source { get; set; }

        /// <summary> The source path (for CLR bindings).</summary>
        public PropertyPath Path { get; set; }

        /// <summary> The XPath path (for XML bindings).</summary>
        [DefaultValue(null)]
        public string XPath { get; set; }

        /// <summary> Binding mode </summary>
        [DefaultValue(BindingMode.Default)]
        public BindingMode Mode { get; set; }

        /// <summary> Update type </summary>
        [DefaultValue(UpdateSourceTrigger.Default)]
        public UpdateSourceTrigger UpdateSourceTrigger { get; set; }

        /// <summary> The Converter to apply </summary>
        [DefaultValue(null)]
        public IValueConverter Converter { get; set; }

        /// <summary>
        /// The parameter to pass to converter.
        /// </summary>
        /// <value></value>
        [DefaultValue(null)]
        public object ConverterParameter { get; set; }

        /// <summary> Culture in which to evaluate the converter </summary>
        [DefaultValue(null)]
        [TypeConverter(typeof(CultureInfoIetfLanguageTagConverter))]
        public CultureInfo ConverterCulture { get; set; }

        /// <summary>
        /// Description of the object to use as the source, relative to the target element.
        /// </summary>
        [DefaultValue(null)]
        public RelativeSource RelativeSource { get; set; }

        /// <summary> Name of the element to use as the source </summary>
        [DefaultValue(null)]
        public string ElementName { get; set; }

        #endregion

        #region BindingBase Members

        /// <summary> Value to use when source cannot provide a value </summary>
        /// <remarks>
        ///     Initialized to DependencyProperty.UnsetValue; if FallbackValue is not set, BindingExpression
        ///     will return target property's default when Binding cannot get a real value.
        /// </remarks>
        public object FallbackValue { get; set; }

        #endregion
        
        #region Nested types

        private class HelperConverter : IMultiValueConverter
        {
            public static readonly HelperConverter Current = new HelperConverter();

            public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture) => Tuple.Create(values[0], (DependencyProperty)parameter);
            public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture) => throw new NotImplementedException();
        }

        #endregion
    }
}
