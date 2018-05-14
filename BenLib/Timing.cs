using System;
using System.Text;
using System.Globalization;

namespace BenLib
{
    public class Timing
    {
        
    }

    /// <summary>
    /// Représente intervalle de temps.
    /// </summary>
    public class Time : IComparable<Time>
    {
        private int hrs, min; //1, 60
        private double sec; //70

        public Time(double totalseconds) => TotalSeconds = totalseconds;

        public Time(int hours, int minutes, double seconds)
        {
            hrs = hours; //1
            min = minutes; //60
            sec = seconds; //70
        }

        public int Hours
        {
            get => hrs + Minutes / 60;
            set => hrs = value;
        } //return 1 + 60/60 → 2

        public int Minutes
        {
            get
            {
                while (min >= 60)
                {
                    hrs++; min -= 60;
                }

                int s = (int)sec / 60;

                return min + s;
            }
            set
            {
                min = value;
                while (min >= 60)
                {
                    hrs++; min -= 60;
                }

                while (min < 0)
                {
                    hrs--; min += 60;
                }
            }
        } //return 60-60 + 70 / 60 → 1

        public double Seconds
        {
            get
            {
                while (sec >= 60)
                {
                    min++; sec -= 60;
                }
                return sec;
            }
            set
            {
                sec = value;

                while (sec >= 60)
                {
                    min++; sec -= 60;
                }

                while (sec < 0)
                {
                    min--; sec += 60;
                }
            }
        } //return 70 - 60 → 10

        public double Milliseconds
        {
            get => Seconds * 1000;
            set => Seconds = value / 1000;
        } //return 10 * 1000 → 10000

        public int TotalMinutes { get => hrs * 60 + min + (int)sec / 60; } //return 1 * 60 + 60 + 70 / 60 → 121

        public double TotalSeconds
        {
            get => hrs * 3600 + min * 60 + sec;
            set
            {
                hrs = min = 0;
                Seconds = value;
            }
        } //return 1 * 3600 + 60 * 60 + 70 → 7270

        public double TotalMilliseconds
        {
            get => TotalSeconds * 1000;
            set => TotalSeconds = value / 1000;
        } //return 7270 * 1000 → 7270000

        public int CompareTo(Time value) => TotalSeconds.CompareTo(value.TotalSeconds);

        public override string ToString() => String.Format("{0:00}", Hours) + ":" + String.Format("{0:00}", Minutes) + ":" + Seconds.ToString(new NumberFormatInfo() { NumberDecimalSeparator = "." }); //return 02:01:10

        public string ToString(string format)
        {
            dynamic seconds = (int)Seconds;
            bool started = false, tts = false;
            int current = 0, h = 0, m = 0, s = 0, dec = 0;
            StringBuilder hs = new StringBuilder("{0:"), ms = new StringBuilder("{0:"), ss = new StringBuilder("{0:"), result = new StringBuilder();

            foreach (char c in format)
            {
                switch (c)
                {
                    case 'h':
                        if (!started) { current = 1; started = true; }
                        if (current == 1) { h++; hs.Append('0'); }
                        else throw new FormatException();
                        break;
                    case 'm':
                        if (!started) { current = 2; started = true; }
                        if (current == 2) { m++; ms.Append('0'); }
                        else throw new FormatException();
                        break;
                    case 's':
                        if (!started) { current = 3; started = true; }
                        if (current == 3) { s++; ss.Append('0'); }
                        else if (current == 4) { dec++; ss.Append('0'); }
                        else throw new FormatException();
                        break;
                    case 'f':
                        if (!started) { current = 3; started = true; }
                        if (current == 3) { tts = true; }
                        else throw new FormatException();
                        break;
                    case '.':
                        if (current == 3) { current = 4; ss.Append('.'); seconds = Seconds; }
                        else throw new FormatException();
                        break;
                    case ':':
                        if (started) current++;
                        else throw new FormatException();
                        if (current > 4) throw new FormatException();
                        break;
                    default:
                        throw new FormatException();
                }
                if (tts) break;
            }

            hs.Append('}');
            ms.Append('}');
            ss.Append('}');

            if (h > 0)
            {
                result.Append(String.Format(hs.ToString(), Hours));
                if (m > 0)
                {
                    result.Append(":" + String.Format(ms.ToString(), Minutes));
                    if (tts)
                    {
                        result.Append(":" + seconds.ToString(new NumberFormatInfo() { NumberDecimalSeparator = "." }));
                    }
                    else if (s > 0)
                    {
                        result.Append(":" + String.Format(new NumberFormatInfo() { NumberDecimalSeparator = "." }, ss.ToString(), seconds));
                    }
                }
            }
            else if (m > 0)
            {
                result.Append(String.Format(ms.ToString(), TotalMinutes));
                if (tts)
                {
                    result.Append(":" + seconds.ToString(new NumberFormatInfo() { NumberDecimalSeparator = "." }));
                }
                else if (s > 0)
                {
                    result.Append(":" + String.Format(new NumberFormatInfo() { NumberDecimalSeparator = "." }, ss.ToString(), seconds));
                }
            }
            else if (tts)
            {
                result.Append(seconds.ToString(new NumberFormatInfo() { NumberDecimalSeparator = "." }));
            }
            else if (s > 0)
            {
                result.Append(String.Format(new NumberFormatInfo() { NumberDecimalSeparator = "." }, ss.ToString(), seconds));
            }

            return result.ToString();
        }

        public string ToString(int maxDecimalPlaces, bool unlimitedAtZero = false)
        {
            if (unlimitedAtZero && maxDecimalPlaces == 0) return ToString();
            StringBuilder sb = new StringBuilder("hh:mm:ss");
            if (maxDecimalPlaces > 0) sb.Append('.');
            maxDecimalPlaces.Times(() => sb.Append('s'));
            return ToString(sb.ToString());
        }

        public static Time operator +(Time t1, Time t2) => new Time(t1.TotalSeconds + t2.TotalSeconds);

        public static Time operator -(Time t1, Time t2) => new Time(t1.TotalSeconds - t2.TotalSeconds);

        public static Time operator *(Time t1, double value) => new Time(t1.TotalSeconds * value);

        public static Time operator /(Time t1, double value) => new Time(t1.TotalSeconds / value);

        public static Time operator ^(Time t1, double value) => new Time(t1.TotalSeconds.Pow(value));
    }
}
