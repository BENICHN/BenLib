using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Threading;
using System.Threading.Tasks;
using static System.Math;

namespace BenLib.Standard
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

        public static double Solve(Func<double, double> f, double start, double end, double precision, double beginValue)
        {
            double Inter(double x0, double x1)
            {
                double fx1 = f(x1);
                return x1 - fx1 * (x1 - x0) / (fx1 - f(x0));
            }

            double result = beginValue;

            while (Abs(f(result)) > precision)
            {
                result = Inter(start, end);

                if (double.IsNaN(result)) return (start + end) / 2;

                start = end;
                end = result;
            }

            return result;
        }

        public static T Solve<T>(Func<double, T> f, Func<T, double> selector, double start, double end, double precision, double beginValue)
        {
            double result = beginValue;
            var current = f(result);

            while (Abs(selector(current)) > precision)
            {
                result = Inter(start, end);

                if (double.IsNaN(result)) return f((start + end) / 2);

                start = end;
                end = result;
                current = f(result);
            }

            return current;

            double val(double x) => selector(f(x));
            double Inter(double x0, double x1)
            {
                double fx1 = val(x1);
                return x1 - fx1 * (x1 - x0) / (fx1 - val(x0));
            }
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
                if (num1 * i % num2 == 0)
                {
                    return i * num1;
                }
            }
            return num1 * num2;
        }

        public static double Sigmoid(double x) => 1.0 / (1 + Exp(-x));

        public static int Interpolate(int start, int end, double progress) => (int)((1 - progress) * start + progress * end);
        public static IEnumerable<int> Interpolate(IList<int> start, IList<int> end, double progress) { foreach (var (from, to) in start.ExpandOrContract((0, start.Count - 1), end, (0, end.Count - 1))) yield return Interpolate(from, to, progress); }

        public static uint Interpolate(uint start, uint end, double progress) => (uint)((1 - progress) * start + progress * end);
        public static IEnumerable<uint> Interpolate(IList<uint> start, IList<uint> end, double progress) { foreach (var (from, to) in start.ExpandOrContract((0, start.Count - 1), end, (0, end.Count - 1))) yield return Interpolate(from, to, progress); }

        public static long Interpolate(long start, long end, double progress) => (long)((1 - progress) * start + progress * end);
        public static IEnumerable<long> Interpolate(IList<long> start, IList<long> end, double progress) { foreach (var (from, to) in start.ExpandOrContract((0, start.Count - 1), end, (0, end.Count - 1))) yield return Interpolate(from, to, progress); }

        public static ulong Interpolate(ulong start, ulong end, double progress) => (ulong)((1 - progress) * start + progress * end);
        public static IEnumerable<ulong> Interpolate(IList<ulong> start, IList<ulong> end, double progress) { foreach (var (from, to) in start.ExpandOrContract((0, start.Count - 1), end, (0, end.Count - 1))) yield return Interpolate(from, to, progress); }

        public static short Interpolate(short start, short end, double progress) => (short)((1 - progress) * start + progress * end);
        public static IEnumerable<short> Interpolate(IList<short> start, IList<short> end, double progress) { foreach (var (from, to) in start.ExpandOrContract((0, start.Count - 1), end, (0, end.Count - 1))) yield return Interpolate(from, to, progress); }

        public static ushort Interpolate(ushort start, ushort end, double progress) => (ushort)((1 - progress) * start + progress * end);
        public static IEnumerable<ushort> Interpolate(IList<ushort> start, IList<ushort> end, double progress) { foreach (var (from, to) in start.ExpandOrContract((0, start.Count - 1), end, (0, end.Count - 1))) yield return Interpolate(from, to, progress); }

        public static byte Interpolate(byte start, byte end, double progress) => (byte)((1 - progress) * start + progress * end);
        public static IEnumerable<byte> Interpolate(IList<byte> start, IList<byte> end, double progress) { foreach (var (from, to) in start.ExpandOrContract((0, start.Count - 1), end, (0, end.Count - 1))) yield return Interpolate(from, to, progress); }

        public static sbyte Interpolate(sbyte start, sbyte end, double progress) => (sbyte)((1 - progress) * start + progress * end);
        public static IEnumerable<sbyte> Interpolate(IList<sbyte> start, IList<sbyte> end, double progress) { foreach (var (from, to) in start.ExpandOrContract((0, start.Count - 1), end, (0, end.Count - 1))) yield return Interpolate(from, to, progress); }

        public static float Interpolate(float start, float end, double progress) => (float)((1 - progress) * start + progress * end);
        public static IEnumerable<float> Interpolate(IList<float> start, IList<float> end, double progress) { foreach (var (from, to) in start.ExpandOrContract((0, start.Count - 1), end, (0, end.Count - 1))) yield return Interpolate(from, to, progress); }

        public static double Interpolate(double start, double end, double progress) => (1 - progress) * start + progress * end;
        public static IEnumerable<double> Interpolate(IList<double> start, IList<double> end, double progress) { foreach (var (from, to) in start.ExpandOrContract((0, start.Count - 1), end, (0, end.Count - 1))) yield return Interpolate(from, to, progress); }

        public static decimal Interpolate(decimal start, decimal end, double progress) => (decimal)(1 - progress) * start + (decimal)progress * end;
        public static IEnumerable<decimal> Interpolate(IList<decimal> start, IList<decimal> end, double progress) { foreach (var (from, to) in start.ExpandOrContract((0, start.Count - 1), end, (0, end.Count - 1))) yield return Interpolate(from, to, progress); }

        public static double Bernstein(double t, double n, double k) => Binom(n, k) * t.Pow(n) * (1 - t).Pow(k - n);
        public static double Binom(double n, double k)
        {
            double result = 1;
            for (int i = 1; i <= k; i++)
            {
                result *= n - (k - i);
                result /= i;
            }
            return result;
        }

        public static IEnumerable<long[]> NegativePascal(long n) => Pascal(n).Select(line => line.Select((n, i) => i % 2 == 0 ? n : -n).ToArray());
        public static IEnumerable<long[]> Pascal(long n)
        {
            long[] current = new long[] { 1 };
            yield return current;

            for (long i = 1; i <= n; i++)
            {
                current = NextLine(current).ToArray();
                yield return current;
            }

            static IEnumerable<long> NextLine(long[] line)
            {
                for (int i = 0; i <= line.Length; i++)
                {
                    yield return
                        i == 0 ? 1 :
                        i == line.Length ? 1 :
                        line[i - 1] + line[i];
                }
            }
        }

        public static (double x, double y) GetBezierPoint(double t, params (double x, double y)[] controlPoints)
        {
            int n = controlPoints.Length - 1;
            double[] bs = Interval<int>.CC(0, n).Numbers().Select(k => Binom(n, k)).ToArray();
            return bs.Select((b, k) =>
            {
                double bernstein = b * t.Pow(k) * (1 - t).Pow(n - k);
                var (x, y) = controlPoints[k];
                return (bernstein * x, bernstein * y);
            }).Operate((p, pn) => (p.Item1 + pn.Item1, p.Item2 + pn.Item2));
        }

        public static IEnumerable<(double x, double y)> GetBezierPoints(double step, params (double x, double y)[] controlPoints)
        {
            var tcoefs = GetTCoefs(controlPoints);
            return Range<double>.CC(0, 1).Numbers(step).Select(t => GetBezierPointFromTCoefs(t, tcoefs));
        }

        public static (double x, double y) GetBezierPointFromX(double x, double precision, params (double x, double y)[] controlPoints)
        {
            var tcoefs = GetTCoefs(controlPoints);
            return Solve(t => GetBezierPointFromTCoefs(t, tcoefs), p => p.x - x, 0, 1, precision, x);
        }

        public static (double x, double y) GetBezierPointFromY(double y, double precision, params (double x, double y)[] controlPoints)
        {
            var tcoefs = GetTCoefs(controlPoints);
            return Solve(t => GetBezierPointFromTCoefs(t, tcoefs), p => p.x - y, 0, 1, precision, y);
        }

        public static (double x, double y) GetBezierPointFromTCoefs(double t, (double cx, double cy)[] tcoefs)
        {
            long n = tcoefs.LongLength - 1;
            return Interval<long>.CC(0, n).Numbers().Select(k =>
            {
                var (cx, cy) = tcoefs[k];
                double tp = t.Pow(n - k);
                return (tp * cx, tp * cy);
            }).Operate((v, vn) => (v.Item1 + vn.Item1, v.Item2 + vn.Item2));
        }

        public static (double cx, double cy)[] GetTCoefs(params (double x, double y)[] controlPoints)
        {
            long n = controlPoints.LongLength - 1;
            long[][] pascal = NegativePascal(n).ToArray();
            long[][] coefs = Interval<long>.CC(0, n).Numbers().Select(k => Abs(pascal[n][k])).Select((k, i) => pascal[n - i].Select(l => k * l).ToArray()).ToArray();
            return coefs.Select((deg, i) => deg.Select((k, j) => { var (x, y) = controlPoints[n - i - j]; return (k * x, k * y); }).Operate((v, vn) => (v.Item1 + vn.Item1, v.Item2 + vn.Item2))).ToArray();
        }

        public static double Distance((double x, double y) start, (double x, double y) end) => Sqrt(SquaredDistance(start, end));
        public static double SquaredDistance((double x, double y) start, (double x, double y) end)
        {
            double ox = end.x - start.x;
            double oy = end.y - start.y;
            return ox * ox + oy * oy;
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

        public static double Trim(this double value, double min = double.NegativeInfinity, double max = double.PositiveInfinity) => Max(Min(value, max), min);
        public static decimal Trim(this decimal value, decimal min = decimal.MinValue, decimal max = decimal.MaxValue) => Max(Min(value, max), min);
        public static float Trim(this float value, float min = float.NegativeInfinity, float max = float.PositiveInfinity) => Max(Min(value, max), min);
        public static int Trim(this int value, int min = int.MinValue, int max = int.MaxValue) => Max(Min(value, max), min);
        public static uint Trim(this uint value, uint min = uint.MinValue, uint max = uint.MaxValue) => Max(Min(value, max), min);
        public static long Trim(this long value, long min = long.MinValue, long max = long.MaxValue) => Max(Min(value, max), min);
        public static ulong Trim(this ulong value, ulong min = ulong.MinValue, ulong max = ulong.MaxValue) => Max(Min(value, max), min);
        public static short Trim(this short value, short min = short.MinValue, short max = short.MaxValue) => Max(Min(value, max), min);
        public static ushort Trim(this ushort value, ushort min = ushort.MinValue, ushort max = ushort.MaxValue) => Max(Min(value, max), min);
        public static byte Trim(this byte value, byte min = byte.MinValue, byte max = byte.MaxValue) => Max(Min(value, max), min);
        public static sbyte Trim(this sbyte value, sbyte min = sbyte.MinValue, sbyte max = sbyte.MaxValue) => Max(Min(value, max), min);

        #endregion

        public static bool IsNullOrEmpty<T>(this Interval<T> interval) where T : IComparable<T> => interval == null || interval.IsEmpty;

        public static short TrimToShort(this int value) => value < short.MinValue ? short.MinValue : value > short.MaxValue ? short.MaxValue : (short)value;
        public static short TrimToShort(this long value) => value < short.MinValue ? short.MinValue : value > short.MaxValue ? short.MaxValue : (short)value;
        public static short TrimToShort(this double value) => value < short.MinValue ? short.MinValue : value > short.MaxValue ? short.MaxValue : (short)value;
        public static short TrimToShort(this decimal value) => value < short.MinValue ? short.MinValue : value > short.MaxValue ? short.MaxValue : (short)value;

        public static int TrimToInt(this long value) => value < int.MinValue ? int.MinValue : value > int.MaxValue ? int.MaxValue : (int)value;
        public static int TrimToInt(this double value) => value < int.MinValue ? int.MinValue : value > int.MaxValue ? int.MaxValue : (int)value;
        public static int TrimToInt(this decimal value) => value < int.MinValue ? int.MinValue : value > int.MaxValue ? int.MaxValue : (int)value;

        public static long TrimToLong(this double value) => value < long.MinValue ? long.MinValue : value > long.MaxValue ? long.MaxValue : (long)value;
        public static long TrimToLong(this decimal value) => value < long.MinValue ? long.MinValue : value > long.MaxValue ? long.MaxValue : (long)value;

        public static decimal TrimToDecimal(this double value) => value < (double)decimal.MinValue ? decimal.MinValue : value > (double)decimal.MaxValue ? decimal.MaxValue : (decimal)value;

        public static double Magnet(this double d, double value, double tolerance) => Abs(d - value) <= tolerance ? value : d;
    }
}
