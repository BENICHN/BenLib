using BenLib.Standard;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Media;
using static System.Math;

namespace BenLib.Framework
{
    public static class NumFramework
    {
        public static Color Interpolate(Color start, Color end, double progress) => start * (float)(1 - progress) + end * (float)progress;
        public static IEnumerable<Color> Interpolate(IList<Color> start, IList<Color> end, double progress)
        {
            foreach (var (from, to) in start.ExpandOrContract((0, start.Count - 1), end, (0, end.Count - 1)))
            {
                yield return Interpolate(from, to, progress);
            }
        }

        public static Pen Interpolate(Pen start, Pen end, double progress)
        {
            var from = start ?? new Pen(Brushes.Transparent, 0);
            var to = end ?? new Pen(Brushes.Transparent, 0);
            var brush = Interpolate(from.Brush, to.Brush, progress);
            return brush != null ? new Pen(brush, Num.Interpolate(from.Thickness, to.Thickness, progress)) : null;
        }
        public static IEnumerable<Pen> Interpolate(IList<Pen> start, IList<Pen> end, double progress)
        {
            foreach (var (from, to) in start.ExpandOrContract((0, start.Count - 1), end, (0, end.Count - 1)))
            {
                yield return Interpolate(from, to, progress);
            }
        }

        public static Brush Interpolate(Brush start, Brush end, double progress)
        {
            var from = start ?? Brushes.Transparent;
            var to = end ?? Brushes.Transparent;
            return from is SolidColorBrush fromC && to is SolidColorBrush toC
                ? new SolidColorBrush(Interpolate(fromC.Color, toC.Color, progress)) { Opacity = Num.Interpolate(from.Opacity, to.Opacity, progress) }
                : null;
        }
        public static IEnumerable<Brush> Interpolate(IList<Brush> start, IList<Brush> end, double progress)
        {
            foreach (var (from, to) in start.ExpandOrContract((0, start.Count - 1), end, (0, end.Count - 1)))
            {
                yield return Interpolate(from, to, progress);
            }
        }

        public static Point Interpolate(Point start, Point end, double progress) => new Point(Num.Interpolate(start.X, end.X, progress), Num.Interpolate(start.Y, end.Y, progress));
        public static IEnumerable<Point> Interpolate(IList<Point> start, IList<Point> end, double progress)
        {
            foreach (var (from, to) in start.ExpandOrContract((0, start.Count - 1), end, (0, end.Count - 1)))
            {
                yield return Interpolate(from, to, progress);
            }
        }

        public static Vector Interpolate(Vector start, Vector end, double progress) => new Vector(Num.Interpolate(start.X, end.X, progress), Num.Interpolate(start.Y, end.Y, progress));
        public static IEnumerable<Vector> Interpolate(IList<Vector> start, IList<Vector> end, double progress)
        {
            foreach (var (from, to) in start.ExpandOrContract((0, start.Count - 1), end, (0, end.Count - 1)))
            {
                yield return Interpolate(from, to, progress);
            }
        }

        public static Size Interpolate(Size start, Size end, double progress) => new Size(Num.Interpolate(start.Width, end.Width, progress), Num.Interpolate(start.Height, end.Height, progress));
        public static IEnumerable<Size> Interpolate(IList<Size> start, IList<Size> end, double progress)
        {
            foreach (var (from, to) in start.ExpandOrContract((0, start.Count - 1), end, (0, end.Count - 1)))
            {
                yield return Interpolate(from, to, progress);
            }
        }

        public static Rect Interpolate(Rect start, Rect end, double progress) => new Rect(Interpolate(start.TopLeft, end.TopLeft, progress), Interpolate(start.BottomRight, end.BottomRight, progress));
        public static IEnumerable<Rect> Interpolate(IList<Rect> start, IList<Rect> end, double progress)
        {
            foreach (var (from, to) in start.ExpandOrContract((0, start.Count - 1), end, (0, end.Count - 1)))
            {
                yield return Interpolate(from, to, progress);
            }
        }

        public static Vector VectorFromPolarCoordinates(double magnitude, double phase) => new Vector(magnitude * Cos(phase), magnitude * Sin(phase));

        public static double AngleBetweenVectors(Vector vector1, Vector vector2)
        {
            double sin = vector1.X * vector2.Y - vector2.X * vector1.Y;
            double cos = vector1.X * vector2.X + vector1.Y * vector2.Y;

            return Atan2(sin, cos);
        }

        public static double Distance(Point start, Point end) => Sqrt(SquaredDistance(start, end));
        public static double SquaredDistance(Point start, Point end) => Num.SquaredDistance(start.Deconstruct(), end.Deconstruct());
    }

    public static partial class Extensions
    {
        public static (double StartProgress, double EndProgress) SplitTrimProgress(this double progress, double splitLocation)
        {
            var (startProgress, endProgress) = SplitProgress(progress, splitLocation);
            return (startProgress.Trim(0, 1), endProgress.Trim(0, 1));
        }
        public static (double StartProgress, double EndProgress) SplitTrimProgress(this double progress, double firstEnd, double lastStart)
        {
            var (startProgress, endProgress) = SplitProgress(progress, firstEnd, lastStart);
            return (startProgress.Trim(0, 1), endProgress.Trim(0, 1));
        }
        public static (double StartProgress, double endProgress) SplitTrimProgress(this double progress, double firstStart, double firstEnd, double lastStart, double lastEnd)
        {
            var (startProgress, endProgress) = SplitProgress(progress, firstStart, firstEnd, lastStart, lastEnd);
            return (startProgress.Trim(0, 1), endProgress.Trim(0, 1));
        }
        public static IEnumerable<double> SplitTrimProgress(this double progress, params double[] splitLocations) => SplitProgress(progress, splitLocations).Select(p => p.Trim(0, 1));

        public static (double StartProgress, double EndProgress) SplitProgress(this double progress, double splitLocation)
        {
            var equation1 = LinearEquation.FromPoints(new Point(0, 0), new Point(splitLocation, 1));
            var equation2 = LinearEquation.FromPoints(new Point(splitLocation, 0), new Point(1, 1));

            return (equation1.Y(progress), equation2.Y(progress));
        }

        public static IEnumerable<double> SplitProgress(this double progress, params double[] splitLocations)
        {
            double previous = 0;

            for (int i = 0; i < splitLocations.Length; i++)
            {
                double location = splitLocations[i];
                yield return LinearEquation.FromPoints(new Point(previous, 0), new Point(location, 1)).Y(progress);
                previous = location;
            }
            yield return LinearEquation.FromPoints(new Point(previous, 0), new Point(1, 1)).Y(progress);
        }
        public static (double StartProgress, double EndProgress) SplitProgress(this double progress, double firstEnd, double lastStart)
        {
            var equation1 = LinearEquation.FromPoints(new Point(0, 0), new Point(firstEnd, 1));
            var equation2 = LinearEquation.FromPoints(new Point(lastStart, 0), new Point(1, 1));

            return (equation1.Y(progress), equation2.Y(progress));
        }
        public static (double StartProgress, double EndProgress) SplitProgress(this double progress, double firstStart, double firstEnd, double lastStart, double lastEnd)
        {
            var equation1 = LinearEquation.FromPoints(new Point(firstStart, 0), new Point(firstEnd, 1));
            var equation2 = LinearEquation.FromPoints(new Point(lastStart, 0), new Point(lastEnd, 1));

            return (equation1.Y(progress), equation2.Y(progress));
        }

        public static double GetProgressAfterSplitting(this double progress, params double[] splitLocations)
        {
            double previous = 0;
            for (int i = 0; i < splitLocations.Length; i++)
            {
                double location = splitLocations[i];
                if (previous <= progress && location >= progress) return LinearEquation.FromPoints(new Point(previous, 0), new Point(location, 1)).Y(progress);
                else previous = location;
            }
            return LinearEquation.FromPoints(new Point(previous, 0), new Point(1, 1)).Y(progress);
        }

        public static Vector ReLength(this Vector vector, double length)
        {
            double lght = vector.Length;
            return new Vector(vector.X * length / lght, vector.Y * length / lght);
        }

        public static Vector Rotate(this Vector v, double radians)
        {
            double ca = Cos(radians);
            double sa = Sin(radians);
            return new Vector(ca * v.X - sa * v.Y, sa * v.X + ca * v.Y);
        }

        public static (Vector u, Vector v) Decompose(this Vector vector, Vector base1, Vector base2)
        {
            double x = vector.X;
            double y = vector.Y;
            double xu = base1.X;
            double yu = base1.Y;
            double xv = base2.X;
            double yv = base2.Y;

            double c = xu * yv - xv * yu;

            double a = (x * yv - y * xv) / c;
            double b = (y * xu - x * yu) / c;
            return (a * base1, b * base2);
        }

        public static (double x, double y) Deconstruct(this Point point) => (point.X, point.Y);
        public static (double x, double y) Deconstruct(this Vector vector) => (vector.X, vector.Y);

        public static bool IsNaN(this double value) => double.IsNaN(value);
        public static bool IsNaN(this Point point) => double.IsNaN(point.X + point.Y);
        public static bool IsNaN(this Vector vector) => double.IsNaN(vector.X + vector.Y);
    }
}
