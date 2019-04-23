using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
//using static BenLib.OrdinalDouble;
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
            return brush != null ? new Pen(brush, Interpolate(from.Thickness, to.Thickness, progress)) : null;
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
                ? new SolidColorBrush(Interpolate(fromC.Color, toC.Color, progress)) { Opacity = Interpolate(from.Opacity, to.Opacity, progress) }
                : null;
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
        public static decimal Trim(this decimal value, decimal min = 0, decimal max = 1) => Max(Min(value, max), min);
        public static int Trim(this int value, int min = 0, int max = 1) => Max(Min(value, max), min);

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

        #endregion

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

        public static bool IsNullOrEmpty<T>(this Interval<T> interval) where T : IComparable<T> => interval == null || interval.IsEmpty;

        public static int TrimToInt(this double value) => value < int.MinValue ? int.MinValue : value > int.MaxValue ? int.MaxValue : (int)value;
        public static int TrimToInt(this decimal value) => value < int.MinValue ? int.MinValue : value > int.MaxValue ? int.MaxValue : (int)value;
        public static decimal TrimToDecimal(this double value) => value < (double)decimal.MinValue ? decimal.MinValue : value > (double)decimal.MaxValue ? decimal.MaxValue : (decimal)value;
    }

    //public abstract class IntInterval
    //{
    //    public int? NumericIndex => FromInfinity ? null : (int?)Index;
    //    public int? NumericLastIndex => ToInfinity ? null : (int?)LastIndex;
    //    public int Index { get; protected set; }
    //    public int Length { get; protected set; }
    //    public int LastIndex { get; protected set; }
    //    public bool FromInfinity { get; protected set; }
    //    public bool ToInfinity { get; protected set; }
    //    public bool IsEmpty { get; protected set; }

    //    public abstract bool Contains(int value);
    //    public abstract bool Contains(IntInterval value);
    //    public static IEnumerable<IntRange> GetRanges(IntInterval rg)
    //    {
    //        if (rg is IntRange r) return new[] { r };
    //        if (rg is IntMultiRange cr) return cr.Ranges;
    //        throw new NotImplementedException();
    //    }

    //    public static IntInterval Invert(IntInterval interval)
    //    {
    //        var re = GetRanges(interval).GetEnumerator();
    //        re.MoveNext();

    //        var c = re.Current;
    //        IntInterval t = c.FromInfinity ? (0, 0) : (IntRange)(null, c.Index);
    //        int tli = c.LastIndex;

    //        while (re.MoveNext())
    //        {
    //            c = re.Current;
    //            t += (tli, c.Index);
    //            tli = c.Index;
    //        }

    //        if (!c.ToInfinity) t += (c.LastIndex, null);

    //        return t;
    //    }

    //    public static IntInterval operator +(IntInterval a, IntInterval b) => IntMultiRange.Create(GetRanges(a).Concat(GetRanges(b)).ToArray());
    //    public static IntInterval operator *(IntInterval a, IntInterval b)
    //    {
    //        if (a is IntRange ra && b is IntRange rb) return ra * rb;
    //        if (a is IntMultiRange cra) return cra * b;
    //        if (b is IntMultiRange crb) return crb * a;
    //        throw new NotImplementedException();
    //    }
    //    public static IntInterval operator /(IntInterval a, IntInterval b) => a * Invert(b);

    //    public static implicit operator IntInterval((int? Index, int? LastIndex) range) => new IntRange(range.Index, range.LastIndex);

    //    public static IntRange EmptySet => new IntRange(0, 0);
    //    public static IntRange NSet => new IntRange(0, null);
    //    public static IntRange ZSet => new IntRange(null, null);
    //}

    //public class IntRange : IntInterval
    //{
    //    public IntRange(int? index, int? lastIndex)
    //    {
    //        if (!index.HasValue)
    //        {
    //            Index = int.MinValue;
    //            FromInfinity = true;
    //            Length = -1;
    //        }
    //        else Index = index.Value;

    //        if (!lastIndex.HasValue)
    //        {
    //            LastIndex = int.MaxValue;
    //            ToInfinity = true;
    //            Length = -1;
    //        }
    //        else LastIndex = lastIndex.Value;

    //        if (LastIndex < Index)
    //        {
    //            Index = 0;
    //            Length = 0;
    //            LastIndex = 0;
    //        }

    //        if (Length == 0) Length = LastIndex - Index;

    //        IsEmpty = Length == 0;
    //    }

    //    public override bool Contains(int value) => (Index <= value) && (ToInfinity || value < LastIndex);
    //    public override bool Contains(IntInterval value) => value is IntRange r ? Contains(r) : value is IntMultiRange cr ? Contains(cr) : false;

    //    public bool Contains(IntRange value) => Contains(value.Index) && Contains(value.LastIndex - 1);
    //    public bool Contains(IntMultiRange value) => Contains(value.Index) && Contains(value.LastIndex - 1);

    //    public static IntRange operator *(IntRange a, IntRange b) => new IntRange(a.FromInfinity && b.FromInfinity ? null : (int?)Max(a.Index, b.Index), a.ToInfinity && b.ToInfinity ? null : (int?)Min(a.LastIndex, b.LastIndex));

    //    public override string ToString() => (NumericIndex, NumericLastIndex) switch
    //    {
    //        (null, null) => "ℤ",
    //        (0, null) => "ℕ",
    //        _ => IsEmpty ? "∅" : $"{(FromInfinity ? "]-∞" : "[" + Index.ToString())} ; {(ToInfinity ? "+∞" : LastIndex.ToString())}["
    //    };

    //    public static implicit operator IntRange((int? Index, int? LastIndex) range) => new IntRange(range.Index, range.LastIndex);
    //}

    //public class IntMultiRange : IntInterval
    //{
    //    private IntMultiRange(IntRange[] ranges)
    //    {
    //        Ranges = ranges;
    //        Index = ranges.First().Index;
    //        LastIndex = ranges.Last().LastIndex;
    //        Length = ranges.Sum(range => range.Length);
    //        IsEmpty = ranges.All(range => range.IsEmpty);
    //        ToInfinity = ranges.Last().ToInfinity;
    //        FromInfinity = ranges.First().FromInfinity;
    //    }

    //    public IntRange[] Ranges { get; }

    //    public override bool Contains(int value) => Ranges.Any(range => range.Contains(value));
    //    public override bool Contains(IntInterval value) => value is IntRange r ? Contains(r) : value is IntMultiRange cr ? Contains(cr) : false;

    //    public bool Contains(IntRange value) => Ranges.Any(range => range.Contains(value));
    //    public bool Contains(IntMultiRange value)
    //    {
    //        var t = this;
    //        return value.Ranges.All(range => t.Contains(range));
    //    }

    //    public static IntInterval operator *(IntMultiRange a, IntInterval b) => Create(GetRanges(a).SelectMany(r => GetRanges(b * r)).ToArray());

    //    public override string ToString() => IsEmpty ? "∅" : string.Join<IntRange>(" ∪ ", Ranges);

    //    public static IEnumerable<IntRange> MergeRanges(IEnumerable<IntRange> ranges)
    //    {
    //        var re = ranges.OrderBy(r => r.Index).GetEnumerator();
    //        var t = new IntRange(0, 0);
    //        while (re.MoveNext())
    //        {
    //            var current = re.Current;
    //            if (t.IsEmpty) t = current;
    //            else if (current.Index <= t.LastIndex) t = new IntRange(t.FromInfinity || current.FromInfinity ? null : (int?)t.Index, t.ToInfinity || current.ToInfinity ? null : (int?)Math.Max(t.LastIndex, current.LastIndex));
    //            else
    //            {
    //                yield return t;
    //                t = current;
    //            }
    //        }
    //        if (!t.IsEmpty) yield return t;
    //    }

    //    public static IntInterval Create(params IntRange[] ranges)
    //    {
    //        var rs = MergeRanges(ranges).ToArray();
    //        switch (rs.Length)
    //        {
    //            case 0: return EmptySet;
    //            case 1: return rs[0];
    //            default: return new IntMultiRange(rs);
    //        }
    //    }
    //}

    //public abstract class DecimalInterval
    //{
    //    public decimal? NumericIndex => FromInfinity ? null : (decimal?)Index;
    //    public decimal? NumericLastIndex => ToInfinity ? null : (decimal?)LastIndex;
    //    public decimal Index { get; protected set; }
    //    public decimal Length { get; protected set; }
    //    public decimal LastIndex { get; protected set; }
    //    public bool FromInfinity { get; protected set; }
    //    public bool ToInfinity { get; protected set; }
    //    public bool IsEmpty { get; protected set; }

    //    public abstract bool Contains(decimal value);
    //    public abstract bool Contains(DecimalInterval value);
    //    public static IEnumerable<DecimalRange> GetRanges(DecimalInterval rg)
    //    {
    //        if (rg is DecimalRange r) return new[] { r };
    //        if (rg is DecimalMultiRange cr) return cr.Ranges;
    //        throw new NotImplementedException();
    //    }

    //    public static DecimalInterval Invert(DecimalInterval interval)
    //    {
    //        var re = GetRanges(interval).GetEnumerator();
    //        re.MoveNext();

    //        var c = re.Current;
    //        DecimalInterval t = c.FromInfinity ? (0, 0) : (DecimalRange)(null, c.Index);
    //        decimal tli = c.LastIndex;

    //        while (re.MoveNext())
    //        {
    //            c = re.Current;
    //            t += (tli, c.Index);
    //            tli = c.Index;
    //        }

    //        if (!c.ToInfinity) t += (c.LastIndex, null);

    //        return t;
    //    }

    //    public static DecimalInterval operator +(DecimalInterval a, DecimalInterval b) => DecimalMultiRange.Create(GetRanges(a).Concat(GetRanges(b)).ToArray());
    //    public static DecimalInterval operator *(DecimalInterval a, DecimalInterval b)
    //    {
    //        if (a is DecimalRange ra && b is DecimalRange rb) return ra * rb;
    //        if (a is DecimalMultiRange cra) return cra * b;
    //        if (b is DecimalMultiRange crb) return crb * a;
    //        throw new NotImplementedException();
    //    }
    //    public static DecimalInterval operator /(DecimalInterval a, DecimalInterval b) => a * Invert(b);

    //    public static implicit operator DecimalInterval((decimal? Index, decimal? LastIndex) range) => new DecimalRange(range.Index, range.LastIndex);

    //    public static DecimalRange EmptySet => new DecimalRange(0, 0);
    //    public static DecimalRange NSet => new DecimalRange(0, null);
    //    public static DecimalRange ZSet => new DecimalRange(null, null);
    //}

    //public class DecimalRange : DecimalInterval
    //{
    //    public DecimalRange(decimal? index, decimal? lastIndex)
    //    {
    //        if (!index.HasValue)
    //        {
    //            Index = decimal.MinValue;
    //            FromInfinity = true;
    //            Length = -1;
    //        }
    //        else Index = index.Value;

    //        if (!lastIndex.HasValue)
    //        {
    //            LastIndex = decimal.MaxValue;
    //            ToInfinity = true;
    //            Length = -1;
    //        }
    //        else LastIndex = lastIndex.Value;

    //        if (LastIndex < Index)
    //        {
    //            Index = 0;
    //            Length = 0;
    //            LastIndex = 0;
    //        }

    //        if (Length == 0) Length = LastIndex - Index;

    //        IsEmpty = Length == 0;
    //    }

    //    public override bool Contains(decimal value) => (Index <= value) && (ToInfinity || value < LastIndex);
    //    public override bool Contains(DecimalInterval value) => value is DecimalRange r ? Contains(r) : value is DecimalMultiRange cr ? Contains(cr) : false;

    //    public bool Contains(DecimalRange value) => Contains(value.Index) && Contains(value.LastIndex - 1);
    //    public bool Contains(DecimalMultiRange value) => Contains(value.Index) && Contains(value.LastIndex - 1);

    //    public static DecimalRange operator *(DecimalRange a, DecimalRange b) => new DecimalRange(a.FromInfinity && b.FromInfinity ? null : (decimal?)Max(a.Index, b.Index), a.ToInfinity && b.ToInfinity ? null : (decimal?)Min(a.LastIndex, b.LastIndex));

    //    public override string ToString() => (NumericIndex, NumericLastIndex) switch
    //    {
    //        (null, null) => "ℝ",
    //        (0, null) => "ℝ₊",
    //        (null, 0) => "ℝ₋",
    //        _ => IsEmpty ? "∅" : $"{(FromInfinity ? "]-∞" : "[" + Index.ToString())} ; {(ToInfinity ? "+∞" : LastIndex.ToString())}["
    //    };

    //    public static implicit operator DecimalRange((decimal? Index, decimal? LastIndex) range) => new DecimalRange(range.Index, range.LastIndex);
    //}

    //public class DecimalMultiRange : DecimalInterval
    //{
    //    private DecimalMultiRange(DecimalRange[] ranges)
    //    {
    //        Ranges = ranges;
    //        Index = ranges.First().Index;
    //        LastIndex = ranges.Last().LastIndex;
    //        Length = ranges.Sum(range => range.Length);
    //        IsEmpty = ranges.All(range => range.IsEmpty);
    //        ToInfinity = ranges.Last().ToInfinity;
    //        FromInfinity = ranges.First().FromInfinity;
    //    }

    //    public DecimalRange[] Ranges { get; }

    //    public override bool Contains(decimal value) => Ranges.Any(range => range.Contains(value));
    //    public override bool Contains(DecimalInterval value) => value is DecimalRange r ? Contains(r) : value is DecimalMultiRange cr ? Contains(cr) : false;

    //    public bool Contains(DecimalRange value) => Ranges.Any(range => range.Contains(value));
    //    public bool Contains(DecimalMultiRange value)
    //    {
    //        var t = this;
    //        return value.Ranges.All(range => t.Contains(range));
    //    }

    //    public static DecimalInterval operator *(DecimalMultiRange a, DecimalInterval b) => Create(GetRanges(a).SelectMany(r => GetRanges(b * r)).ToArray());

    //    public override string ToString() => IsEmpty ? "∅" : string.Join<DecimalRange>(" ∪ ", Ranges);

    //    public static IEnumerable<DecimalRange> MergeRanges(IEnumerable<DecimalRange> ranges)
    //    {
    //        var re = ranges.OrderBy(r => r.Index).GetEnumerator();
    //        var t = new DecimalRange(0, 0);
    //        while (re.MoveNext())
    //        {
    //            var current = re.Current;
    //            if (t.IsEmpty) t = current;
    //            else if (current.Index <= t.LastIndex) t = new DecimalRange(t.FromInfinity || current.FromInfinity ? null : (decimal?)t.Index, t.ToInfinity || current.ToInfinity ? null : (decimal?)Math.Max(t.LastIndex, current.LastIndex));
    //            else
    //            {
    //                yield return t;
    //                t = current;
    //            }
    //        }
    //        if (!t.IsEmpty) yield return t;
    //    }

    //    public static DecimalInterval Create(params DecimalRange[] ranges)
    //    {
    //        var rs = MergeRanges(ranges).ToArray();
    //        switch (rs.Length)
    //        {
    //            case 0: return EmptySet;
    //            case 1: return rs[0];
    //            default: return new DecimalMultiRange(rs);
    //        }
    //    }
    //}

    //public interface IInterval<T>
    //{
    //    bool IsEmpty { get; }
    //    IInterval<T> Invert { get; }

    //    bool Contains(T value);
    //    bool Contains(IOrdinal<T> value);
    //    bool Contains(IInterval<T> value);

    //    IInterval<T> Union(IInterval<T> value);
    //    IInterval<T> Inter(IInterval<T> value);
    //    IInterval<T> Except(IInterval<T> value);
    //}

    //public interface IRange<T> : IInterval<T>
    //{
    //    IOrdinal<T> Start { get; }
    //    IOrdinal<T> End { get; }
    //}

    //public interface IMultiRange<T> : IInterval<T>
    //{
    //    IRange<T>[] Ranges { get; }
    //}

    //public static partial class Extensions
    //{
    //    public static IInterval<T> Union<T>(this IEnumerable<IInterval<T>> source)
    //    {
    //        var e = source.GetEnumerator();
    //        if (e.MoveNext())
    //        {
    //            var result = e.Current;
    //            while (e.MoveNext()) result = result.Union(e.Current);
    //            return result;
    //        }
    //        return default;
    //    }

    //    public static IInterval<T> Inter<T>(this IEnumerable<IInterval<T>> source)
    //    {
    //        var e = source.GetEnumerator();
    //        if (e.MoveNext())
    //        {
    //            var result = e.Current;
    //            while (e.MoveNext()) result = result.Inter(e.Current);
    //            return result;
    //        }
    //        return default;
    //    }
    //}

    //public struct DoubleRange : IRange<double>, IEquatable<DoubleRange>
    //{
    //    private DoubleRange(IOrdinal<double> start, IOrdinal<double> end)
    //    {
    //        Start = start;
    //        End = end;
    //    }

    //    public IOrdinal<double> Start { get; }
    //    public IOrdinal<double> End { get; }

    //    public bool IsEmpty => End.CompareTo(Start) <= 0;

    //    public IInterval<double> Invert => Create(OO(NegativeInfinity, Start), OO(End, NegativeInfinity));

    //    public bool Contains(double value) => Contains((OrdinalDouble)value);
    //    public bool Contains(IOrdinal<double> value) => Start.CompareTo(value) <= 0 && value.CompareTo(End) <= 0;
    //    public bool Contains(IInterval<double> value)
    //    {
    //        var start = Start;
    //        var end = End;
    //        return GetRanges(value).All(range => start.CompareTo(range.Start) <= 0 && range.Start.CompareTo(end) <= 0 && start.CompareTo(range.End) <= 0 && range.End.CompareTo(end) <= 0);
    //    }

    //    public IInterval<double> Union(IInterval<double> value) => Create(GetRanges(value).Concat(this).ToArray());
    //    public IInterval<double> Except(IInterval<double> value) => Inter(value.Invert);
    //    public IInterval<double> Inter(IInterval<double> value)
    //    {
    //        var start = Start;
    //        var end = End;
    //        return Create(GetRanges(value).Select(r => (IRange<double>)CC(Max(start, r.Start), Min(end, r.End))));
    //    }

    //    public static IInterval<double> operator +(DoubleRange left, IInterval<double> right) => left.Union(right);
    //    public static IInterval<double> operator *(DoubleRange left, IInterval<double> right) => left.Inter(right);
    //    public static IInterval<double> operator /(DoubleRange left, IInterval<double> right) => left.Except(right);

    //    public static DoubleRange OO(IOrdinal<double> start, IOrdinal<double> end) => new DoubleRange(start.Next, end.Antecedent);
    //    public static DoubleRange OC(IOrdinal<double> start, IOrdinal<double> end) => new DoubleRange(start.Next, end);
    //    public static DoubleRange CO(IOrdinal<double> start, IOrdinal<double> end) => new DoubleRange(start, end.Antecedent);
    //    public static DoubleRange CC(IOrdinal<double> start, IOrdinal<double> end) => new DoubleRange(start, end);
    //    public static DoubleRange OO(OrdinalDouble start, OrdinalDouble end) => new DoubleRange(start.Next, end.Antecedent);
    //    public static DoubleRange OC(OrdinalDouble start, OrdinalDouble end) => new DoubleRange(start.Next, end);
    //    public static DoubleRange CO(OrdinalDouble start, OrdinalDouble end) => new DoubleRange(start, end.Antecedent);
    //    public static DoubleRange CC(OrdinalDouble start, OrdinalDouble end) => new DoubleRange(start, end);

    //    public override bool Equals(object obj) => obj is DoubleRange range && Equals(range);
    //    public bool Equals(DoubleRange other) => IsEmpty && other.IsEmpty || EqualityComparer<IOrdinal<double>>.Default.Equals(Start, other.Start) && EqualityComparer<IOrdinal<double>>.Default.Equals(End, other.End);

    //    public override int GetHashCode()
    //    {
    //        int hashCode = -1676728671;
    //        hashCode = hashCode * -1521134295 + EqualityComparer<IOrdinal<double>>.Default.GetHashCode(Start);
    //        hashCode = hashCode * -1521134295 + EqualityComparer<IOrdinal<double>>.Default.GetHashCode(End);
    //        return hashCode;
    //    }

    //    public override string ToString() =>
    //        IsEmpty ? "∅" :
    //        this == Reals ? "ℝ" :
    //        this == NegativeReals ? "ℝ₋" :
    //        this == PositivesReals ? "ℝ₊" :
    //        this == NegativeRealsNoZero ? "ℝ₋*" :
    //        this == PositivesRealsNoZero ? "ℝ₊*" :
    //        $"{(Start.Level > 0 ? $"]{Start.Antecedent}" : $"[{Start}")} ; {(End.Level < 0 ? $"{End.Next}[" : $"{End}]")}";

    //    public static DoubleRange EmptySet { get; } = CC(0, 0);
    //    public static DoubleRange NegativeReals { get; } = CC(NegativeInfinity, 0);
    //    public static DoubleRange PositivesReals { get; } = CC(0, PositiveInfinity);
    //    public static DoubleRange Reals { get; } = CC(NegativeInfinity, PositiveInfinity);
    //    public static DoubleRange NegativeRealsNoZero { get; } = CO(NegativeInfinity, 0);
    //    public static DoubleRange PositivesRealsNoZero { get; } = OC(0, PositiveInfinity);

    //    public static bool operator ==(DoubleRange left, DoubleRange right) => left.Equals(right);
    //    public static bool operator !=(DoubleRange left, DoubleRange right) => !(left == right);
    //}

    //public struct DoubleMultiRange : IMultiRange<double>, IEquatable<DoubleMultiRange>
    //{
    //    private DoubleMultiRange(params IRange<double>[] orderedRanges) => Ranges = orderedRanges;

    //    public IRange<double>[] Ranges { get; }
    //    public bool IsEmpty => Ranges?.All(r => r.IsEmpty) ?? true;

    //    public IInterval<double> Invert => Ranges.Select(r => r.Invert).Inter();

    //    public bool Contains(double value) => Contains((OrdinalDouble)value);
    //    public bool Contains(IOrdinal<double> value) => Ranges.Any(r => r.Contains(value));
    //    public bool Contains(IInterval<double> value)
    //    {
    //        var ranges = Ranges;
    //        return GetRanges(value).All(ra => ranges.Any(r => r.Contains(ra)));
    //    }

    //    public IInterval<double> Except(IInterval<double> value) => Inter(value.Invert);
    //    public IInterval<double> Inter(IInterval<double> value) => Create(Ranges.SelectMany(r => GetRanges(value.Inter(r))).ToArray());
    //    public IInterval<double> Union(IInterval<double> value) => Create(GetRanges(value).Concat(Ranges).ToArray());

    //    public static IInterval<double> operator +(DoubleMultiRange left, IInterval<double> right) => left.Union(right);
    //    public static IInterval<double> operator *(DoubleMultiRange left, IInterval<double> right) => left.Inter(right);
    //    public static IInterval<double> operator /(DoubleMultiRange left, IInterval<double> right) => left.Except(right);

    //    public static IEnumerable<IRange<double>> MergeRanges(IEnumerable<IRange<double>> ranges)
    //    {
    //        var t = EmptySet;
    //        foreach (var range in ranges.OrderBy(r => r.Start).ToArray())
    //        {
    //            if (!range.IsEmpty)
    //            {
    //                if (t.IsEmpty) t = CC(range.Start, range.End);
    //                else if (range.Start.CompareTo(t.End) != 2) t = CC(t.Start, Max(t.End, range.End));
    //                else
    //                {
    //                    yield return t;
    //                    t = CC(range.Start, range.End);
    //                }
    //            }
    //        }
    //        if (!t.IsEmpty) yield return t;
    //    }

    //    public static IInterval<double> Create(params IRange<double>[] ranges) => Create((IEnumerable<IRange<double>>)ranges);
    //    public static IInterval<double> Create(IEnumerable<IRange<double>> ranges)
    //    {
    //        var rs = MergeRanges(ranges).ToArray();
    //        return rs.Length switch
    //        {
    //            0 => (IInterval<double>)EmptySet,
    //            1 => rs[0],
    //            _ => new DoubleMultiRange(rs)
    //        };
    //    }

    //    public static IRange<double>[] GetRanges(IInterval<double> value) => value is IRange<double> doubleRange ? new[] { doubleRange } : value is IMultiRange<double> doubleMultiRange ? doubleMultiRange.Ranges : Array.Empty<IRange<double>>();

    //    public override string ToString() =>
    //        IsEmpty ? "∅" :
    //        this == RealsNoZero ? "ℝ*" :
    //        string.Join<IRange<double>>(" ∪ ", Ranges);

    //    public override bool Equals(object obj) => obj is DoubleMultiRange range && Equals(range);
    //    public bool Equals(DoubleMultiRange other) => IsEmpty && other.IsEmpty || EqualityComparer<IRange<double>[]>.Default.Equals(Ranges, other.Ranges);
    //    public override int GetHashCode() => -1198097269 + EqualityComparer<IRange<double>[]>.Default.GetHashCode(Ranges);

    //    public static DoubleMultiRange RealsNoZero { get; } = new DoubleMultiRange(NegativeRealsNoZero, PositivesRealsNoZero);

    //    public static bool operator ==(DoubleMultiRange left, DoubleMultiRange right) => left.Equals(right);
    //    public static bool operator !=(DoubleMultiRange left, DoubleMultiRange right) => !(left == right);
    //}
}
