using System;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Shapes;

namespace BenLib.WPF
{
    public static class Misc
    {
        private static Random Random { get; } = new Random();

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

        /// <summary>
        /// Finds a Child of a given item in the visual tree. 
        /// </summary>
        /// <param name="parent">A direct parent of the queried item.</param>
        /// <typeparam name="T">The type of the queried item.</typeparam>
        /// <param name="childName">x:Name or Name of child. </param>
        /// <returns>The first parent item that matches the submitted type parameter. 
        /// If not matching item can be found, 
        /// a null parent is being returned.</returns>
        public static T FindChild<T>(DependencyObject parent, string childName) where T : DependencyObject
        {
            // Confirm parent and childName are valid. 
            if (parent == null) return null;

            T foundChild = null;

            int childrenCount = VisualTreeHelper.GetChildrenCount(parent);
            for (int i = 0; i < childrenCount; i++)
            {
                var child = VisualTreeHelper.GetChild(parent, i);
                // If the child is not of the request child type child
                T childType = child as T;
                if (childType == null)
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
        public static T FindVisualChild<T>(DependencyObject depObj) where T : DependencyObject
        {
            if (depObj != null)
            {
                for (int i = 0; i < VisualTreeHelper.GetChildrenCount(depObj); i++)
                {
                    DependencyObject child = VisualTreeHelper.GetChild(depObj, i);
                    if (child != null && child is T) return (T)child;

                    T childItem = FindVisualChild<T>(child);
                    if (childItem != null) return childItem;
                }
            }
            return null;
        }

        public static T FindParent<T>(DependencyObject child) where T : DependencyObject
        {
            //get parent item
            DependencyObject parentObject = VisualTreeHelper.GetParent(child);

            //we've reached the end of the tree
            if (parentObject == null) return null;

            //check if the parent matches the type we're looking for
            if (parentObject is T parent) return parent;
            else return FindParent<T>(parentObject);
        }

        public static Color GetRandomColor() => Color.FromRgb((byte)Num.RandomInt(255), (byte)Num.RandomInt(255), (byte)Num.RandomInt(255));

        public static Color GetRandomColor(byte alpha) => Color.FromArgb(alpha, (byte)Num.RandomInt(255), (byte)Num.RandomInt(255), (byte)Num.RandomInt(255));

        public static Color GetRandomColor(byte minAlpha, byte maxAlpha) => Color.FromArgb((byte)Num.RandomInt(minAlpha, minAlpha), (byte)Num.RandomInt(255), (byte)Num.RandomInt(255), (byte)Num.RandomInt(255));

        public static Ellipse CreateCircle(Point center, double radius, Brush fill = null, Brush stroke = null, double strokeThickness = 0)
        {
            double diameter = 2 * radius;

            var result = new Ellipse { Width = diameter, Height = diameter, Fill = fill ?? Brushes.Black, Stroke = stroke ?? Brushes.Black, StrokeThickness = strokeThickness };

            Canvas.SetTop(result, center.Y - radius);
            Canvas.SetLeft(result, center.X - radius);

            return result;
        }
    }

    public static partial class Extensions
    {
        public static TryResult TryClose(this Window window)
        {
            try
            {
                window.Close();
                return new TryResult(true);
            }
            catch (Exception ex) { return new TryResult(false, ex); }
        }

        public static T GetAsTypedFrozen<T>(this T freezable) where T : Freezable
        {
            if (freezable.CanFreeze) freezable.Freeze();
            return freezable;
        }

        public static T Edit<T>(this T freezable, Action<T> edition) where T : Freezable
        {
            var isFrozen = freezable.IsFrozen;

            var result = (T)freezable.CloneCurrentValue();
            edition(result);

            if (isFrozen && result.CanFreeze) result.Freeze();
            return result;
        }

        public static double Width(this GlyphRun glyphRun) => glyphRun.AdvanceWidths.Sum();
        public static double Height(this GlyphRun glyphRun) => glyphRun.GlyphTypeface.Baseline * glyphRun.FontRenderingEmSize;
        public static Size Size(this GlyphRun glyphRun) => new Size(glyphRun.Width(), glyphRun.Height());
    }

    public class BoolToValueConverter<T> : DependencyObject, IValueConverter
    {
        public T FalseValue { get => (T)GetValue(FalseValueProperty); set => SetValue(FalseValueProperty, value); }
        public T TrueValue { get => (T)GetValue(TrueValueProperty); set => SetValue(TrueValueProperty, value); }

        public static readonly DependencyProperty FalseValueProperty = DependencyProperty.RegisterAttached("FalseValue", typeof(T), typeof(BoolToValueConverter<T>), new PropertyMetadata(null));
        public static readonly DependencyProperty TrueValueProperty = DependencyProperty.RegisterAttached("TrueValue", typeof(T), typeof(BoolToValueConverter<T>), new PropertyMetadata(null));

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
                return FalseValue;
            else
                return (bool)value ? TrueValue : FalseValue;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value != null ? value.Equals(TrueValue) : false;
        }
    }

    public class MultiValueConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture) => values.Clone();
        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture) => null;
    }
}
