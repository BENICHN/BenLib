using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace BenLib
{
    public class Num
    {
        private static SemaphoreSlim RandomSemaphore { get; } = new SemaphoreSlim(1);
        private static Random RandomObject { get; } = new Random();

        public static double Random()
        {
            try
            {
                RandomSemaphore.Wait();
                return RandomObject.NextDouble();
            }
            finally { RandomSemaphore.Release(); }
        }

        public static double Random(double scale) => Random() * scale;

        public static int RandomInt()
        {
            try
            {
                RandomSemaphore.Wait();
                return RandomObject.Next();
            }
            finally { RandomSemaphore.Release(); }
        }

        public static int RandomInt(int maxValue)
        {
            try
            {
                RandomSemaphore.Wait();
                return RandomObject.Next(maxValue);
            }
            finally { RandomSemaphore.Release(); }
        }

        public static int RandomInt(int minValue, int maxValue)
        {
            try
            {
                RandomSemaphore.Wait();
                return RandomObject.Next(minValue, maxValue);
            }
            finally { RandomSemaphore.Release(); }
        }

        public static async Task<double> RandomAsync()
        {
            try
            {
                await RandomSemaphore.WaitAsync();
                return RandomObject.NextDouble();
            }
            finally { RandomSemaphore.Release(); }
        }

        public static async Task<double> RandomAsync(double scale) => await RandomAsync() * scale;

        public static async Task<int> RandomIntAsync()
        {
            try
            {
                await RandomSemaphore.WaitAsync();
                return RandomObject.Next();
            }
            finally { RandomSemaphore.Release(); }
        }

        public static async Task<int> RandomIntAsync(int maxValue)
        {
            try
            {
                await RandomSemaphore.WaitAsync();
                return RandomObject.Next(maxValue);
            }
            finally { RandomSemaphore.Release(); }
        }

        public static async Task<int> RandomIntAsync(int minValue, int maxValue)
        {
            try
            {
                await RandomSemaphore.WaitAsync();
                return RandomObject.Next(minValue, maxValue);
            }
            finally { RandomSemaphore.Release(); }
        }

        public static double Solve(Func<double, double> f, double start, double end, double precision)
        {
            double Inter(double x0, double x1)
            {
                double fx1 = f(x1);
                return x1 - fx1 * (x1 - x0) / (fx1 - f(x0));
            }

            double result = double.MaxValue;

            while (Math.Abs(f(result)) > precision)
            {
                result = Inter(start, end);

                if (double.IsNaN(result)) return (start + end) / 2;

                start = end;
                end = result;
            }

            return result;
        }

        public static int LCM(int a, int b)
        {
            int num1, num2;

            if (a > b)
            {
                num1 = a;
                num2 = b;
            }
            else
            {
                num1 = b;
                num2 = a;
            }

            for (int i = 1; i < num2; i++)
            {
                if ((num1 * i) % num2 == 0)
                {
                    return i * num1;
                }
            }
            return num1 * num2;
        }

        public static double Sigmoid(double x) => 1.0 / (1 + Math.Exp(-x));

        public static double Interpolate(double start, double end, double progress) => (1 - progress) * start + progress * end;
        public static IEnumerable<double> Interpolate(IList<double> start, IList<double> end, double progress)
        {
            foreach (var (from, to) in start.ExpandOrContract((0, start.Count - 1), end, (0, end.Count - 1)))
            {
                yield return Interpolate(from, to, progress);
            }
        }

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
            if (brush != null) return new Pen(brush, Interpolate(from.Thickness, to.Thickness, progress));
            else return null;
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
            if (from is SolidColorBrush fromC && to is SolidColorBrush toC) return new SolidColorBrush(Interpolate(fromC.Color, toC.Color, progress)) { Opacity = Interpolate(from.Opacity, to.Opacity, progress) };
            else return null;
        }
        public static IEnumerable<Brush> Interpolate(IList<Brush> start, IList<Brush> end, double progress)
        {
            foreach (var (from, to) in start.ExpandOrContract((0, start.Count - 1), end, (0, end.Count - 1)))
            {
                yield return Interpolate(from, to, progress);
            }
        }

        public static Point Interpolate(Point start, Point end, double progress) => new Point(Interpolate(start.X, end.X, progress), Interpolate(start.Y, end.Y, progress));
        public static IEnumerable<Point> Interpolate(IList<Point> start, IList<Point> end, double progress)
        {
            foreach (var (from, to) in start.ExpandOrContract((0, start.Count - 1), end, (0, end.Count - 1)))
            {
                yield return Interpolate(from, to, progress);
            }
        }

        public static Vector Interpolate(Vector start, Vector end, double progress) => new Vector(Interpolate(start.X, end.X, progress), Interpolate(start.Y, end.Y, progress));
        public static IEnumerable<Vector> Interpolate(IList<Vector> start, IList<Vector> end, double progress)
        {
            foreach (var (from, to) in start.ExpandOrContract((0, start.Count - 1), end, (0, end.Count - 1)))
            {
                yield return Interpolate(from, to, progress);
            }
        }

        public static Size Interpolate(Size start, Size end, double progress) => new Size(Interpolate(start.Width, end.Width, progress), Interpolate(start.Height, end.Height, progress));
        public static IEnumerable<Size> Interpolate(IList<Size> start, IList<Size> end, double progress)
        {
            foreach (var (from, to) in start.ExpandOrContract((0, start.Count - 1), end, (0, end.Count - 1)))
            {
                yield return Interpolate(from, to, progress);
            }
        }
    }

    public static partial class Extensions
    {
        #region Pow

        public static double Pow(this double x, double y) => Math.Pow(x, y);

        public static double Pow(this int x, double y) => Math.Pow(x, y);

        public static double Pow(this decimal x, double y) => Math.Pow((double)x, y);

        public static double Pow(this long x, double y) => Math.Pow(x, y);

        public static double Pow(this float x, double y) => Math.Pow(x, y);

        public static double Pow(this short x, double y) => Math.Pow(x, y);

        public static double Pow(this uint x, double y) => Math.Pow(x, y);

        public static double Pow(this ushort x, double y) => Math.Pow(x, y);

        public static double Pow(this ulong x, double y) => Math.Pow(x, y);

        #endregion

        #region ComplexPow

        public static Complex ComplexPow(this double x, double y) => Complex.Pow(x, y);

        public static Complex ComplexPow(this int x, double y) => Complex.Pow(x, y);

        public static Complex ComplexPow(this decimal x, double y) => Complex.Pow((double)x, y);

        public static Complex ComplexPow(this long x, double y) => Complex.Pow(x, y);

        public static Complex ComplexPow(this float x, double y) => Complex.Pow(x, y);

        public static Complex ComplexPow(this short x, double y) => Complex.Pow(x, y);

        public static Complex ComplexPow(this uint x, double y) => Complex.Pow(x, y);

        public static Complex ComplexPow(this ushort x, double y) => Complex.Pow(x, y);

        public static Complex ComplexPow(this ulong x, double y) => Complex.Pow(x, y);


        public static Complex ComplexPow(this double x, Complex y) => Complex.Pow(x, y);

        public static Complex ComplexPow(this int x, Complex y) => Complex.Pow(x, y);

        public static Complex ComplexPow(this decimal x, Complex y) => Complex.Pow((double)x, y);

        public static Complex ComplexPow(this long x, Complex y) => Complex.Pow(x, y);

        public static Complex ComplexPow(this float x, Complex y) => Complex.Pow(x, y);

        public static Complex ComplexPow(this short x, Complex y) => Complex.Pow(x, y);

        public static Complex ComplexPow(this uint x, Complex y) => Complex.Pow(x, y);

        public static Complex ComplexPow(this ushort x, Complex y) => Complex.Pow(x, y);

        public static Complex ComplexPow(this ulong x, Complex y) => Complex.Pow(x, y);

        #endregion

        #region AbsPow

        private static double AbsPowCore(double x, double y)
        {
            double result = Math.Abs(x).Pow(y);
            if (x < 0.0) result *= -1.0;

            return result;
        }

        public static double AbsPow(this double x, double y) => AbsPowCore(x, y);

        public static double AbsPow(this int x, double y) => AbsPowCore(x, y);

        public static double AbsPow(this decimal x, double y) => AbsPowCore((double)x, y);

        public static double AbsPow(this long x, double y) => AbsPowCore(x, y);

        public static double AbsPow(this float x, double y) => AbsPowCore(x, y);

        public static double AbsPow(this short x, double y) => AbsPowCore(x, y);

        public static double AbsPow(this uint x, double y) => AbsPowCore(x, y);

        public static double AbsPow(this ushort x, double y) => AbsPowCore(x, y);

        public static double AbsPow(this ulong x, double y) => AbsPowCore(x, y);

        #endregion

        #region Progress

        public static double Trim(this double value, double min = 0, double max = 1) => Math.Max(Math.Min(value, max), min);

        public static (double StartProgress, double EndProgress) SplitTrimProgress(this double progress, double splitLocation)
        {
            var (startProgress, endProgress) = SplitProgress(progress, splitLocation);
            return (startProgress.Trim(), endProgress.Trim());
        }
        public static (double StartProgress, double EndProgress) SplitTrimProgress(this double progress, double firstEnd, double lastStart)
        {
            var (startProgress, endProgress) = SplitProgress(progress, firstEnd, lastStart);
            return (startProgress.Trim(), endProgress.Trim());
        }
        public static (double StartProgress, double endProgress) SplitTrimProgress(this double progress, double firstStart, double firstEnd, double lastStart, double lastEnd)
        {
            var (startProgress, endProgress) = SplitProgress(progress, firstStart, firstEnd, lastStart, lastEnd);
            return (startProgress.Trim(), endProgress.Trim());
        }
        public static IEnumerable<double> SplitTrimProgress(this double progress, params double[] splitLocations) => SplitProgress(progress, splitLocations).Select(p => p.Trim());


        public static (double StartProgress, double EndProgress) SplitProgress(this double progress, double splitLocation)
        {
            var equation1 = LinearEquation.FromPoints(new Point(0, 0), new Point(splitLocation, 1));
            var equation2 = LinearEquation.FromPoints(new Point(splitLocation, 0), new Point(1, 1));

            return (equation1.YFunction(progress), equation2.YFunction(progress));
        }

        public static IEnumerable<double> SplitProgress(this double progress, params double[] splitLocations)
        {
            double previous = 0;

            for (int i = 0; i < splitLocations.Length; i++)
            {
                var location = splitLocations[i];
                yield return LinearEquation.FromPoints(new Point(previous, 0), new Point(location, 1)).YFunction(progress);
                previous = location;
            }
            yield return LinearEquation.FromPoints(new Point(previous, 0), new Point(1, 1)).YFunction(progress);
        }
        public static (double StartProgress, double EndProgress) SplitProgress(this double progress, double firstEnd, double lastStart)
        {
            var equation1 = LinearEquation.FromPoints(new Point(0, 0), new Point(firstEnd, 1));
            var equation2 = LinearEquation.FromPoints(new Point(lastStart, 0), new Point(1, 1));

            return (equation1.YFunction(progress), equation2.YFunction(progress));
        }
        public static (double StartProgress, double EndProgress) SplitProgress(this double progress, double firstStart, double firstEnd, double lastStart, double lastEnd)
        {
            var equation1 = LinearEquation.FromPoints(new Point(firstStart, 0), new Point(firstEnd, 1));
            var equation2 = LinearEquation.FromPoints(new Point(lastStart, 0), new Point(lastEnd, 1));

            return (equation1.YFunction(progress), equation2.YFunction(progress));
        }

        public static double GetProgressAfterSplitting(this double progress, params double[] splitLocations)
        {
            double previous = 0;
            for (int i = 0; i < splitLocations.Length; i++)
            {
                var location = splitLocations[i];
                if (previous <= progress && location >= progress) return LinearEquation.FromPoints(new Point(previous, 0), new Point(location, 1)).YFunction(progress);
                else previous = location;
            }
            return LinearEquation.FromPoints(new Point(previous, 0), new Point(1, 1)).YFunction(progress);
        }

        #endregion
    }
}
