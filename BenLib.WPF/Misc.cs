using BenLib.Standard;
using System;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Shapes;
using static BenLib.Standard.Num;

namespace BenLib.WPF
{
    public static class Misc
    {
        /*public static readonly DependencyProperty CommandBindingsProperty = DependencyProperty.RegisterAttached("CommandBindings", typeof(CommandBindingCollection), typeof(Misc), new UIPropertyMetadata(null, CommandBindingsChanged));

        private static void CommandBindingsChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is UIElement element)
            {
                element.CommandBindings.Clear();
                foreach (CommandBinding binding in e.NewValue as CommandBindingCollection)
                {
                    element.CommandBindings.Add(binding);
                }
            }
        }

        public static CommandBindingCollection GetCommandBindings(UIElement element) => (CommandBindingCollection)element.GetValue(CommandBindingsProperty);
        public static void SetCommandBindings(UIElement element, CommandBindingCollection value) => element.SetValue(CommandBindingsProperty, value);*/

        public static Color GetRandomColor() => Color.FromRgb((byte)RandomInt(255), (byte)RandomInt(255), (byte)RandomInt(255));

        public static Color GetRandomColor(byte alpha) => Color.FromArgb(alpha, (byte)RandomInt(255), (byte)RandomInt(255), (byte)RandomInt(255));

        public static Color GetRandomColor(byte minAlpha, byte maxAlpha) => Color.FromArgb((byte)RandomInt(minAlpha, maxAlpha), (byte)RandomInt(255), (byte)RandomInt(255), (byte)RandomInt(255));

        public static Ellipse CreateCircle(Point center, double radius, Brush fill = null, Brush stroke = null, double strokeThickness = 0)
        {
            double diameter = 2 * radius;

            var result = new Ellipse { Width = diameter, Height = diameter, Fill = fill ?? Brushes.Black, Stroke = stroke ?? Brushes.Black, StrokeThickness = strokeThickness };

            Canvas.SetTop(result, center.Y - radius);
            Canvas.SetLeft(result, center.X - radius);

            return result;
        }

        public static SolidColorBrush BrushFromHex(uint hex)
        {
            byte a = (byte)(hex / 0x1000000);
            hex -= (uint)a * 0x1000000;
            byte r = (byte)(hex / 0x10000);
            hex -= (uint)r * 0x10000;
            byte g = (byte)(hex / 0x100);
            hex -= (uint)g * 0x100;
            byte b = (byte)hex;
            return new SolidColorBrush(Color.FromArgb(a, r, g, b));
        }
    }

    public static partial class Extensions
    {
        public static TryResult TryClose(this Window window)
        {
            try
            {
                window.Close();
                return true;
            }
            catch (Exception ex) { return ex; }
        }

        public static T GetAsTypedFrozen<T>(this T freezable) where T : Freezable
        {
            if (freezable.CanFreeze) freezable.Freeze();
            return freezable;
        }

        public static T EditFreezable<T>(this T freezable, Action<T> edition) where T : Freezable
        {
            bool isFrozen = freezable.IsFrozen;

            var result = (T)freezable.CloneCurrentValue();
            edition(result);

            if (isFrozen && result.CanFreeze) result.Freeze();
            return result;
        }

        public static double Width(this GlyphRun glyphRun) => glyphRun.AdvanceWidths.Sum();
        public static double Height(this GlyphRun glyphRun) => glyphRun.GlyphTypeface.Baseline * glyphRun.FontRenderingEmSize;
        public static Size Size(this GlyphRun glyphRun) => new Size(glyphRun.Width(), glyphRun.Height());

        /// <summary>
        /// Finds a Child of a given item in the visual tree. 
        /// </summary>
        /// <param name="parent">A direct parent of the queried item.</param>
        /// <typeparam name="T">The type of the queried item.</typeparam>
        /// <param name="childName">x:Name or Name of child. </param>
        /// <returns>The first parent item that matches the submitted type parameter. 
        /// If not matching item can be found, 
        /// a null parent is being returned.</returns>
        public static T FindChild<T>(this DependencyObject parent, string childName) where T : DependencyObject
        {
            // Confirm parent and childName are valid. 
            if (parent == null) return null;

            T foundChild = null;

            int childrenCount = VisualTreeHelper.GetChildrenCount(parent);
            for (int i = 0; i < childrenCount; i++)
            {
                var child = VisualTreeHelper.GetChild(parent, i);
                // If the child is not of the request child type child
                if (!(child is T))
                {
                    // recursively drill down the tree
                    foundChild = FindChild<T>(child, childName);

                    // If the child is found, break so we do not overwrite the found child. 
                    if (foundChild != null) break;
                }
                else if (!string.IsNullOrEmpty(childName))
                {
                    if (child is FrameworkElement frameworkElement && frameworkElement.Name == childName)
                    {
                        // if the child's name is of the request name
                        foundChild = (T)child;
                        break;
                    }
                }
                else
                {
                    // child element found.
                    foundChild = (T)child;
                    break;
                }
            }

            return foundChild;
        }

        /// <summary>
        /// Obtient un <see cref="DependencyObject"/> contenu dans un autre <see cref="DependencyObject"/>.
        /// </summary>
        /// <typeparam name="T">Type de l'objet cherché.</typeparam>
        /// <param name="depObj">Objet dans lequel chercher.</param>
        /// <returns>L'objet trouvé.</returns>
        public static T FindVisualChild<T>(this DependencyObject depObj) where T : DependencyObject
        {
            if (depObj != null)
            {
                for (int i = 0; i < VisualTreeHelper.GetChildrenCount(depObj); i++)
                {
                    var child = VisualTreeHelper.GetChild(depObj, i);
                    if (child != null && child is T) return (T)child;

                    var childItem = FindVisualChild<T>(child);
                    if (childItem != null) return childItem;
                }
            }
            return null;
        }

        public static T FindParent<T>(this DependencyObject child) where T : DependencyObject
        {
            //get parent item
            var parentObject = VisualTreeHelper.GetParent(child);

            //we've reached the end of the tree
            if (parentObject == null) return null;

            //check if the parent matches the type we're looking for
            return parentObject is T parent ? parent : FindParent<T>(parentObject);
        }
    }

    public class BoolToValueConverter<T> : DependencyObject, IValueConverter
    {
        public T FalseValue { get => (T)GetValue(FalseValueProperty); set => SetValue(FalseValueProperty, value); }
        public T TrueValue { get => (T)GetValue(TrueValueProperty); set => SetValue(TrueValueProperty, value); }

        public static readonly DependencyProperty FalseValueProperty = DependencyProperty.RegisterAttached("FalseValue", typeof(T), typeof(BoolToValueConverter<T>), new PropertyMetadata(null));
        public static readonly DependencyProperty TrueValueProperty = DependencyProperty.RegisterAttached("TrueValue", typeof(T), typeof(BoolToValueConverter<T>), new PropertyMetadata(null));

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture) => value == null ? FalseValue : (object)((bool)value ? TrueValue : FalseValue);

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => value != null ? value.Equals(TrueValue) : false;
    }

    public class MultiValueConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture) => values.Clone();
        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture) => null;
    }
}
