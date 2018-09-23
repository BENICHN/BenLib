using System.Drawing;

namespace BenLib
{
    public class Imaging
    {
        public static Color FromScARGB(double scA, double scR, double scG, double scB) => Color.FromArgb((int)(scA * 255), (int)(scR * 255), (int)(scG * 255), (int)(scB * 255));
    }

    public static partial class Extensions
    {
        public static string ToHex(this Color color)
        {
            return "#" + color.R.ToString("X2") + color.G.ToString("X2") + color.B.ToString("X2");
        }

        public static string ToRGB(this Color color)
        {
            return "RGB(" + color.R.ToString() + "," + color.G.ToString() + "," + color.B.ToString() + ")";
        }

        public static double ScR(this Color color) => color.R / 255.0;
        public static double ScG(this Color color) => color.G / 255.0;
        public static double ScB(this Color color) => color.B / 255.0;
        public static double ScA(this Color color) => color.A / 255.0;

        public static Color Multiply(Color color, double coefficient)
        {
            coefficient /= 255.0;
            return Imaging.FromScARGB(color.A * coefficient, color.R * coefficient, color.G * coefficient, color.B * coefficient);
        }
    }
}
