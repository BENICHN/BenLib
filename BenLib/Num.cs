using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using static System.Math;

namespace BenLib
{
    public static class Num
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

            while (Abs(f(result)) > precision)
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

        public static double Sigmoid(double x) => 1.0 / (1 + Exp(-x));

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

        public static Vector VectorFromPolarCoordinates(double magnitude, double phase) => new Vector(magnitude * Cos(phase), magnitude * Sin(phase));

        public static double AngleBetweenVectors(Vector vector1, Vector vector2)
        {
            double sin = vector1.X * vector2.Y - vector2.X * vector1.Y;
            double cos = vector1.X * vector2.X + vector1.Y * vector2.Y;

            return Atan2(sin, cos);
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
            double result = Abs(x).Pow(y);
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

        public static double Trim(this double value, double min = 0, double max = 1) => Max(Min(value, max), min);

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

            return (equation1.Y(progress), equation2.Y(progress));
        }

        public static IEnumerable<double> SplitProgress(this double progress, params double[] splitLocations)
        {
            double previous = 0;

            for (int i = 0; i < splitLocations.Length; i++)
            {
                var location = splitLocations[i];
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
                var location = splitLocations[i];
                if (previous <= progress && location >= progress) return LinearEquation.FromPoints(new Point(previous, 0), new Point(location, 1)).Y(progress);
                else previous = location;
            }
            return LinearEquation.FromPoints(new Point(previous, 0), new Point(1, 1)).Y(progress);
        }

        public static Vector ReLength(this Vector vector, double length)
        {
            var lght = vector.Length;
            return new Vector(vector.X * length / lght, vector.Y * length / lght);
        }

        #endregion

        public static Vector Rotate(this Vector v, double radians)
        {
            var ca = Cos(radians);
            var sa = Sin(radians);
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

        public static bool IsNullOrEmpty(this IntInterval interval) => interval == null || interval.IsEmpty;
        public static bool IsNullOrEmpty(this DoubleInterval interval) => interval == null || interval.IsEmpty;
    }

    public abstract class IntInterval
    {
        public int Index { get; protected set; }
        public int Length { get; protected set; }
        public int LastIndex { get; protected set; }
        public bool FromInfinity { get; protected set; }
        public bool ToInfinity { get; protected set; }
        public bool IsEmpty { get; protected set; }

        public abstract bool Contains(int value);
        public abstract bool Contains(IntInterval value);
        public static IEnumerable<IntRange> GetRanges(IntInterval rg)
        {
            if (rg is IntRange r) return new[] { r };
            if (rg is IntMultiRange cr) return cr.Ranges;
            throw new NotImplementedException();
        }

        public static IntInterval Invert(IntInterval interval)
        {
            var re = GetRanges(interval).GetEnumerator();
            re.MoveNext();

            var c = re.Current;
            IntInterval t = c.FromInfinity ? (0, 0) : (IntRange)(null, c.Index);
            int tli = c.LastIndex;

            while (re.MoveNext())
            {
                c = re.Current;
                t += (tli, c.Index);
                tli = c.Index;
            }

            if (!c.ToInfinity) t += (c.LastIndex, null);

            return t;
        }

        public static IntInterval operator +(IntInterval a, IntInterval b) => IntMultiRange.Create(GetRanges(a).Concat(GetRanges(b)).ToArray());
        public static IntInterval operator *(IntInterval a, IntInterval b)
        {
            if (a is IntRange ra && b is IntRange rb) return ra * rb;
            if (a is IntMultiRange cra) return cra * b;
            if (b is IntMultiRange crb) return crb * a;
            throw new NotImplementedException();
        }
        public static IntInterval operator /(IntInterval a, IntInterval b) => a * Invert(b);

        public static implicit operator IntInterval((int? Index, int? LastIndex) range) => new IntRange(range.Index, range.LastIndex);

        public static IntRange EmptySet => new IntRange(0, 0);
        public static IntRange NSet => new IntRange(0, null);
        public static IntRange ZSet => new IntRange(null, null);
    }

    public class IntRange : IntInterval
    {
        public IntRange(int? index, int? lastIndex)
        {
            if (!index.HasValue)
            {
                Index = int.MinValue;
                FromInfinity = true;
                Length = -1;
            }
            else Index = index.Value;

            if (!lastIndex.HasValue)
            {
                LastIndex = int.MaxValue;
                ToInfinity = true;
                Length = -1;
            }
            else LastIndex = lastIndex.Value;

            if (LastIndex < Index)
            {
                Index = 0;
                Length = 0;
                LastIndex = 0;
            }

            if (Length == 0) Length = LastIndex - Index;

            IsEmpty = Length == 0;
        }

        public override bool Contains(int value) => (Index <= value) && (ToInfinity || value < LastIndex);
        public override bool Contains(IntInterval value)
        {
            if (value is IntRange r) return Contains(r);
            if (value is IntMultiRange cr) return Contains(cr);
            return false;
        }

        public bool Contains(IntRange value) => Contains(value.Index) && Contains(value.LastIndex - 1);
        public bool Contains(IntMultiRange value) => Contains(value.Index) && Contains(value.LastIndex - 1);

        public static IntRange operator *(IntRange a, IntRange b) => new IntRange(a.FromInfinity && b.FromInfinity ? null : (int?)Max(a.Index, b.Index), a.ToInfinity && b.ToInfinity ? null : (int?)Min(a.LastIndex, b.LastIndex));

        public override string ToString() => IsEmpty ? "∅" : $"{(FromInfinity ? "]-∞" : "[" + Index.ToString())} ; {(ToInfinity ? "+∞" : LastIndex.ToString())}[";

        public static implicit operator IntRange((int? Index, int? LastIndex) range) => new IntRange(range.Index, range.LastIndex);
    }

    public class IntMultiRange : IntInterval
    {
        private IntMultiRange(IntRange[] ranges)
        {
            Ranges = ranges;
            Index = ranges.First().Index;
            LastIndex = ranges.Last().LastIndex;
            Length = ranges.Sum(range => range.Length);
            IsEmpty = ranges.All(range => range.IsEmpty);
            ToInfinity = ranges.Last().ToInfinity;
            FromInfinity = ranges.First().FromInfinity;
        }

        public IntRange[] Ranges { get; }

        public override bool Contains(int value) => Ranges.Any(range => range.Contains(value));
        public override bool Contains(IntInterval value)
        {
            if (value is IntRange r) return Contains(r);
            if (value is IntMultiRange cr) return Contains(cr);
            return false;
        }

        public bool Contains(IntRange value) => Ranges.Any(range => range.Contains(value));
        public bool Contains(IntMultiRange value)
        {
            var t = this;
            return value.Ranges.All(range => t.Contains(range));
        }

        public static IntInterval operator *(IntMultiRange a, IntInterval b) => Create(GetRanges(a).SelectMany(r => GetRanges(b * r)).ToArray());

        public override string ToString() => IsEmpty ? "∅" : string.Join<IntRange>(" ∪ ", Ranges);

        public static IEnumerable<IntRange> MergeRanges(IEnumerable<IntRange> ranges)
        {
            var re = ranges.OrderBy(r => r.Index).GetEnumerator();
            var t = new IntRange(0, 0);
            while (re.MoveNext())
            {
                var current = re.Current;
                if (t.IsEmpty) t = current;
                else if (current.Index <= t.LastIndex) t = new IntRange(t.FromInfinity || current.FromInfinity ? null : (int?)t.Index, t.ToInfinity || current.ToInfinity ? null : (int?)current.LastIndex);
                else
                {
                    yield return t;
                    t = current;
                }
            }
            if (!t.IsEmpty) yield return t;
        }

        public static IntInterval Create(params IntRange[] ranges)
        {
            var rs = MergeRanges(ranges).ToArray();
            switch (rs.Length)
            {
                case 0: return EmptySet;
                case 1: return rs[0];
                default: return new IntMultiRange(rs);
            }
        }
    }


    public abstract class DoubleInterval
    {
        public double Index { get; protected set; }
        public double Length { get; protected set; }
        public double LastIndex { get; protected set; }
        public bool FromInfinity { get; protected set; }
        public bool ToInfinity { get; protected set; }
        public bool IsEmpty { get; protected set; }

        public abstract bool Contains(double value);
        public abstract bool Contains(DoubleInterval value);
        public static IEnumerable<DoubleRange> GetRanges(DoubleInterval rg)
        {
            if (rg is DoubleRange r) return new[] { r };
            if (rg is DoubleMultiRange cr) return cr.Ranges;
            throw new NotImplementedException();
        }

        public static DoubleInterval Invert(DoubleInterval interval)
        {
            var re = GetRanges(interval).GetEnumerator();
            re.MoveNext();

            var c = re.Current;
            DoubleInterval t = c.FromInfinity ? (0, 0) : (DoubleRange)(null, c.Index);
            double tli = c.LastIndex;

            while (re.MoveNext())
            {
                c = re.Current;
                t += (tli, c.Index);
                tli = c.Index;
            }

            if (!c.ToInfinity) t += (c.LastIndex, null);

            return t;
        }

        public static DoubleInterval operator +(DoubleInterval a, DoubleInterval b) => DoubleMultiRange.Create(GetRanges(a).Concat(GetRanges(b)).ToArray());
        public static DoubleInterval operator *(DoubleInterval a, DoubleInterval b)
        {
            if (a is DoubleRange ra && b is DoubleRange rb) return ra * rb;
            if (a is DoubleMultiRange cra) return cra * b;
            if (b is DoubleMultiRange crb) return crb * a;
            throw new NotImplementedException();
        }
        public static DoubleInterval operator /(DoubleInterval a, DoubleInterval b) => a * Invert(b);

        public static implicit operator DoubleInterval((double? Index, double? LastIndex) range) => new DoubleRange(range.Index, range.LastIndex);

        public static DoubleRange EmptySet => new DoubleRange(0, 0);
        public static DoubleRange NSet => new DoubleRange(0, null);
        public static DoubleRange ZSet => new DoubleRange(null, null);
    }

    public class DoubleRange : DoubleInterval
    {
        public DoubleRange(double? index, double? lastIndex)
        {
            if (!index.HasValue)
            {
                Index = double.MinValue;
                FromInfinity = true;
                Length = -1;
            }
            else Index = index.Value;

            if (!lastIndex.HasValue)
            {
                LastIndex = double.MaxValue;
                ToInfinity = true;
                Length = -1;
            }
            else LastIndex = lastIndex.Value;

            if (LastIndex < Index)
            {
                Index = 0;
                Length = 0;
                LastIndex = 0;
            }

            if (Length == 0) Length = LastIndex - Index;

            IsEmpty = Length == 0;
        }

        public override bool Contains(double value) => (Index <= value) && (ToInfinity || value < LastIndex);
        public override bool Contains(DoubleInterval value)
        {
            if (value is DoubleRange r) return Contains(r);
            if (value is DoubleMultiRange cr) return Contains(cr);
            return false;
        }

        public bool Contains(DoubleRange value) => Contains(value.Index) && Contains(value.LastIndex - 1);
        public bool Contains(DoubleMultiRange value) => Contains(value.Index) && Contains(value.LastIndex - 1);

        public static DoubleRange operator *(DoubleRange a, DoubleRange b) => new DoubleRange(a.FromInfinity && b.FromInfinity ? null : (double?)Max(a.Index, b.Index), a.ToInfinity && b.ToInfinity ? null : (double?)Min(a.LastIndex, b.LastIndex));

        public override string ToString() => IsEmpty ? "∅" : $"{(FromInfinity ? "]-∞" : "[" + Index.ToString())} ; {(ToInfinity ? "+∞" : LastIndex.ToString())}[";

        public static implicit operator DoubleRange((double? Index, double? LastIndex) range) => new DoubleRange(range.Index, range.LastIndex);
    }

    public class DoubleMultiRange : DoubleInterval
    {
        private DoubleMultiRange(DoubleRange[] ranges)
        {
            Ranges = ranges;
            Index = ranges.First().Index;
            LastIndex = ranges.Last().LastIndex;
            Length = ranges.Sum(range => range.Length);
            IsEmpty = ranges.All(range => range.IsEmpty);
            ToInfinity = ranges.Last().ToInfinity;
            FromInfinity = ranges.First().FromInfinity;
        }

        public DoubleRange[] Ranges { get; }

        public override bool Contains(double value) => Ranges.Any(range => range.Contains(value));
        public override bool Contains(DoubleInterval value)
        {
            if (value is DoubleRange r) return Contains(r);
            if (value is DoubleMultiRange cr) return Contains(cr);
            return false;
        }

        public bool Contains(DoubleRange value) => Ranges.Any(range => range.Contains(value));
        public bool Contains(DoubleMultiRange value)
        {
            var t = this;
            return value.Ranges.All(range => t.Contains(range));
        }

        public static DoubleInterval operator *(DoubleMultiRange a, DoubleInterval b) => Create(GetRanges(a).SelectMany(r => GetRanges(b * r)).ToArray());

        public override string ToString() => IsEmpty ? "∅" : string.Join<DoubleRange>(" ∪ ", Ranges);

        public static IEnumerable<DoubleRange> MergeRanges(IEnumerable<DoubleRange> ranges)
        {
            var re = ranges.OrderBy(r => r.Index).GetEnumerator();
            var t = new DoubleRange(0, 0);
            while (re.MoveNext())
            {
                var current = re.Current;
                if (t.IsEmpty) t = current;
                else if (current.Index <= t.LastIndex) t = new DoubleRange(t.FromInfinity || current.FromInfinity ? null : (double?)t.Index, t.ToInfinity || current.ToInfinity ? null : (double?)current.LastIndex);
                else
                {
                    yield return t;
                    t = current;
                }
            }
            if (!t.IsEmpty) yield return t;
        }

        public static DoubleInterval Create(params DoubleRange[] ranges)
        {
            var rs = MergeRanges(ranges).ToArray();
            switch (rs.Length)
            {
                case 0: return EmptySet;
                case 1: return rs[0];
                default: return new DoubleMultiRange(rs);
            }
        }
    }


    public abstract class DecimalInterval
    {
        public decimal Index { get; protected set; }
        public decimal Length { get; protected set; }
        public decimal LastIndex { get; protected set; }
        public bool FromInfinity { get; protected set; }
        public bool ToInfinity { get; protected set; }
        public bool IsEmpty { get; protected set; }

        public abstract bool Contains(decimal value);
        public abstract bool Contains(DecimalInterval value);
        public static IEnumerable<DecimalRange> GetRanges(DecimalInterval rg)
        {
            if (rg is DecimalRange r) return new[] { r };
            if (rg is DecimalMultiRange cr) return cr.Ranges;
            throw new NotImplementedException();
        }

        public static DecimalInterval Invert(DecimalInterval interval)
        {
            var re = GetRanges(interval).GetEnumerator();
            re.MoveNext();

            var c = re.Current;
            DecimalInterval t = c.FromInfinity ? (0, 0) : (DecimalRange)(null, c.Index);
            decimal tli = c.LastIndex;

            while (re.MoveNext())
            {
                c = re.Current;
                t += (tli, c.Index);
                tli = c.Index;
            }

            if (!c.ToInfinity) t += (c.LastIndex, null);

            return t;
        }

        public static DecimalInterval operator +(DecimalInterval a, DecimalInterval b) => DecimalMultiRange.Create(GetRanges(a).Concat(GetRanges(b)).ToArray());
        public static DecimalInterval operator *(DecimalInterval a, DecimalInterval b)
        {
            if (a is DecimalRange ra && b is DecimalRange rb) return ra * rb;
            if (a is DecimalMultiRange cra) return cra * b;
            if (b is DecimalMultiRange crb) return crb * a;
            throw new NotImplementedException();
        }
        public static DecimalInterval operator /(DecimalInterval a, DecimalInterval b) => a * Invert(b);

        public static implicit operator DecimalInterval((decimal? Index, decimal? LastIndex) range) => new DecimalRange(range.Index, range.LastIndex);

        public static DecimalRange EmptySet => new DecimalRange(0, 0);
        public static DecimalRange NSet => new DecimalRange(0, null);
        public static DecimalRange ZSet => new DecimalRange(null, null);
    }

    public class DecimalRange : DecimalInterval
    {
        public DecimalRange(decimal? index, decimal? lastIndex)
        {
            if (!index.HasValue)
            {
                Index = decimal.MinValue;
                FromInfinity = true;
                Length = -1;
            }
            else Index = index.Value;

            if (!lastIndex.HasValue)
            {
                LastIndex = decimal.MaxValue;
                ToInfinity = true;
                Length = -1;
            }
            else LastIndex = lastIndex.Value;

            if (LastIndex < Index)
            {
                Index = 0;
                Length = 0;
                LastIndex = 0;
            }

            if (Length == 0) Length = LastIndex - Index;

            IsEmpty = Length == 0;
        }

        public override bool Contains(decimal value) => (Index <= value) && (ToInfinity || value < LastIndex);
        public override bool Contains(DecimalInterval value)
        {
            if (value is DecimalRange r) return Contains(r);
            if (value is DecimalMultiRange cr) return Contains(cr);
            return false;
        }

        public bool Contains(DecimalRange value) => Contains(value.Index) && Contains(value.LastIndex - 1);
        public bool Contains(DecimalMultiRange value) => Contains(value.Index) && Contains(value.LastIndex - 1);

        public static DecimalRange operator *(DecimalRange a, DecimalRange b) => new DecimalRange(a.FromInfinity && b.FromInfinity ? null : (decimal?)Max(a.Index, b.Index), a.ToInfinity && b.ToInfinity ? null : (decimal?)Min(a.LastIndex, b.LastIndex));

        public override string ToString() => IsEmpty ? "∅" : $"{(FromInfinity ? "]-∞" : "[" + Index.ToString())} ; {(ToInfinity ? "+∞" : LastIndex.ToString())}[";

        public static implicit operator DecimalRange((decimal? Index, decimal? LastIndex) range) => new DecimalRange(range.Index, range.LastIndex);
    }

    public class DecimalMultiRange : DecimalInterval
    {
        private DecimalMultiRange(DecimalRange[] ranges)
        {
            Ranges = ranges;
            Index = ranges.First().Index;
            LastIndex = ranges.Last().LastIndex;
            Length = ranges.Sum(range => range.Length);
            IsEmpty = ranges.All(range => range.IsEmpty);
            ToInfinity = ranges.Last().ToInfinity;
            FromInfinity = ranges.First().FromInfinity;
        }

        public DecimalRange[] Ranges { get; }

        public override bool Contains(decimal value) => Ranges.Any(range => range.Contains(value));
        public override bool Contains(DecimalInterval value)
        {
            if (value is DecimalRange r) return Contains(r);
            if (value is DecimalMultiRange cr) return Contains(cr);
            return false;
        }

        public bool Contains(DecimalRange value) => Ranges.Any(range => range.Contains(value));
        public bool Contains(DecimalMultiRange value)
        {
            var t = this;
            return value.Ranges.All(range => t.Contains(range));
        }

        public static DecimalInterval operator *(DecimalMultiRange a, DecimalInterval b) => Create(GetRanges(a).SelectMany(r => GetRanges(b * r)).ToArray());

        public override string ToString() => IsEmpty ? "∅" : string.Join<DecimalRange>(" ∪ ", Ranges);

        public static IEnumerable<DecimalRange> MergeRanges(IEnumerable<DecimalRange> ranges)
        {
            var re = ranges.OrderBy(r => r.Index).GetEnumerator();
            var t = new DecimalRange(0, 0);
            while (re.MoveNext())
            {
                var current = re.Current;
                if (t.IsEmpty) t = current;
                else if (current.Index <= t.LastIndex) t = new DecimalRange(t.FromInfinity || current.FromInfinity ? null : (decimal?)t.Index, t.ToInfinity || current.ToInfinity ? null : (decimal?)current.LastIndex);
                else
                {
                    yield return t;
                    t = current;
                }
            }
            if (!t.IsEmpty) yield return t;
        }

        public static DecimalInterval Create(params DecimalRange[] ranges)
        {
            var rs = MergeRanges(ranges).ToArray();
            switch (rs.Length)
            {
                case 0: return EmptySet;
                case 1: return rs[0];
                default: return new DecimalMultiRange(rs);
            }
        }
    }
}
