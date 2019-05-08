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
        public static float Trim(this float value, float min = 0, float max = 1) => Max(Min(value, max), min);
        public static int Trim(this int value, int min = 0, int max = 1) => Max(Min(value, max), min);
        public static uint Trim(this uint value, uint min = 0, uint max = 1) => Max(Min(value, max), min);
        public static long Trim(this long value, long min = 0, long max = 1) => Max(Min(value, max), min);
        public static ulong Trim(this ulong value, ulong min = 0, ulong max = 1) => Max(Min(value, max), min);
        public static short Trim(this short value, short min = 0, short max = 1) => Max(Min(value, max), min);
        public static ushort Trim(this ushort value, ushort min = 0, ushort max = 1) => Max(Min(value, max), min);
        public static byte Trim(this byte value, byte min = 0, byte max = 1) => Max(Min(value, max), min);
        public static sbyte Trim(this sbyte value, sbyte min = 0, sbyte max = 1) => Max(Min(value, max), min);

        #endregion

        public static bool IsNullOrEmpty<T>(this Interval<T> interval) where T : IComparable<T> => interval == null || interval.IsEmpty;

        public static int TrimToInt(this double value) => value < int.MinValue ? int.MinValue : value > int.MaxValue ? int.MaxValue : (int)value;
        public static int TrimToInt(this decimal value) => value < int.MinValue ? int.MinValue : value > int.MaxValue ? int.MaxValue : (int)value;
        public static decimal TrimToDecimal(this double value) => value < (double)decimal.MinValue ? decimal.MinValue : value > (double)decimal.MaxValue ? decimal.MaxValue : (decimal)value;

        public static double Magnet(this double d, double value, double tolerance) => Abs(d - value) <= tolerance ? value : d;
    }
}
