using System.Windows.Input;

namespace BenLib
{
    public class Input
    {
        public static bool IsShiftPressed() => (Keyboard.Modifiers & ModifierKeys.Shift) == ModifierKeys.Shift;
        public static bool IsControlPressed() => (Keyboard.Modifiers & ModifierKeys.Control) == ModifierKeys.Control;
        public static bool IsAltPressed() => (Keyboard.Modifiers & ModifierKeys.Alt) == ModifierKeys.Alt;
        public static bool IsWindowsPressed() => (Keyboard.Modifiers & ModifierKeys.Windows) == ModifierKeys.Windows;
    }

    public static partial class Extensions
    {
        public static bool OnlyPressed(this MouseEventArgs e, MouseButton button) => button switch
        {
            MouseButton.Left => e.LeftButton == MouseButtonState.Pressed && e.MiddleButton == MouseButtonState.Released && e.RightButton == MouseButtonState.Released && e.XButton1 == MouseButtonState.Released && e.XButton2 == MouseButtonState.Released,
            MouseButton.Middle => e.LeftButton == MouseButtonState.Released && e.MiddleButton == MouseButtonState.Pressed && e.RightButton == MouseButtonState.Released && e.XButton1 == MouseButtonState.Released && e.XButton2 == MouseButtonState.Released,
            MouseButton.Right => e.LeftButton == MouseButtonState.Released && e.MiddleButton == MouseButtonState.Released && e.RightButton == MouseButtonState.Pressed && e.XButton1 == MouseButtonState.Released && e.XButton2 == MouseButtonState.Released,
            MouseButton.XButton1 => e.LeftButton == MouseButtonState.Released && e.MiddleButton == MouseButtonState.Released && e.RightButton == MouseButtonState.Released && e.XButton1 == MouseButtonState.Pressed && e.XButton2 == MouseButtonState.Released,
            MouseButton.XButton2 => e.LeftButton == MouseButtonState.Released && e.MiddleButton == MouseButtonState.Released && e.RightButton == MouseButtonState.Released && e.XButton1 == MouseButtonState.Released && e.XButton2 == MouseButtonState.Pressed,
            _ => false
        };

        public static bool OnlyReleased(this MouseEventArgs e, MouseButton button, bool xButtons = true) => button switch
        {
            MouseButton.Left => e.LeftButton == MouseButtonState.Released && e.MiddleButton == MouseButtonState.Pressed && e.RightButton == MouseButtonState.Pressed && (xButtons || e.XButton1 == MouseButtonState.Pressed && e.XButton2 == MouseButtonState.Pressed),
            MouseButton.Middle => e.LeftButton == MouseButtonState.Pressed && e.MiddleButton == MouseButtonState.Released && e.RightButton == MouseButtonState.Pressed && (xButtons || e.XButton1 == MouseButtonState.Pressed && e.XButton2 == MouseButtonState.Pressed),
            MouseButton.Right => e.LeftButton == MouseButtonState.Pressed && e.MiddleButton == MouseButtonState.Pressed && e.RightButton == MouseButtonState.Released && (xButtons || e.XButton1 == MouseButtonState.Pressed && e.XButton2 == MouseButtonState.Pressed),
            MouseButton.XButton1 => e.LeftButton == MouseButtonState.Pressed && e.MiddleButton == MouseButtonState.Pressed && e.RightButton == MouseButtonState.Pressed && e.XButton1 == MouseButtonState.Released && e.XButton2 == MouseButtonState.Pressed,
            MouseButton.XButton2 => e.LeftButton == MouseButtonState.Pressed && e.MiddleButton == MouseButtonState.Pressed && e.RightButton == MouseButtonState.Pressed && e.XButton1 == MouseButtonState.Pressed && e.XButton2 == MouseButtonState.Released,
            _ => false
        };

        public static bool AllReleased(this MouseEventArgs e) => e.LeftButton == MouseButtonState.Released && e.MiddleButton == MouseButtonState.Released && e.RightButton == MouseButtonState.Released && e.XButton1 == MouseButtonState.Released && e.XButton2 == MouseButtonState.Released;
    }
}
