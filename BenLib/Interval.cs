using BenLib;
using System;
using System.Collections.Generic;
using System.Linq;
using static System.Math;

namespace BenLib
{
    public readonly struct Ordinal<T> : IComparable<Ordinal<T>>, IEquatable<Ordinal<T>> where T : IComparable<T>
    {
        public Ordinal(T value, int level) : this()
        {
            Value = value;
            Level = level;
        }

        private Ordinal(T value, int level, bool isNaN, bool isPositiveInfinity, bool isNegativeInfinity) : this(value, level)
        {
            IsNaN = isNaN;
            IsPositiveInfinity = isPositiveInfinity;
            IsNegativeInfinity = isNegativeInfinity;
        }

        public T Value { get; }
        public int Level { get; }

        public bool IsNaN { get; }
        public bool IsPositiveInfinity { get; }
        public bool IsNegativeInfinity { get; }

        public bool IsReal => !(IsNaN || IsPositiveInfinity || IsNegativeInfinity);

        public Ordinal<T> Antecedent => IsNaN ? NaN : IsPositiveInfinity ? PositiveInfinity : IsNegativeInfinity ? NegativeInfinity : new Ordinal<T>(Value, Level - 1, false, false, false);
        public Ordinal<T> Next => IsNaN ? NaN : IsPositiveInfinity ? PositiveInfinity : IsNegativeInfinity ? NegativeInfinity : new Ordinal<T>(Value, Level + 1, false, false, false);

        public int CompareTo(Ordinal<T> other) => CompareTo(in other);
        public int CompareTo(in Ordinal<T> other)
        {
            int result = IsNaN || other.IsNaN ? 0 : (IsPositiveInfinity, IsNegativeInfinity, other.IsPositiveInfinity, other.IsNegativeInfinity) switch
            {
                (true, false, true, false) => 0, //ω == ω 
                (false, true, false, true) => 0, //-ω == -ω

                (false, false, true, false) => -2, //x < ω
                (false, true, false, false) => -2, //-ω < x
                (false, true, true, false) => -2, //-ω < ω

                (true, false, false, false) => 2, //ω > x
                (false, false, false, true) => 2, //x > -ω
                (true, false, false, true) => 2, //ω > -ω

                _ => 3
            };

            if (result == 3)
            {
                int comp = 2 * Value.CompareTo(other.Value);
                result = comp == 0 ? (Level - other.Level).Trim(-2, 2) : comp;
            }

            return result;
        }

        public bool IsFarBefore(in Ordinal<T> other) => CompareTo(in other) == -2;
        public bool IsFarAfter(in Ordinal<T> other) => CompareTo(in other) == 2;
        public bool IsAround(in Ordinal<T> other) => Abs(CompareTo(in other)) < 2;

        public Ordinal<TResult> Convert<TResult>() where TResult : IComparable<TResult> => new Ordinal<TResult>(IsReal ? (TResult)(object)Value : default, Level, IsNaN, IsPositiveInfinity, IsNegativeInfinity);
        public Ordinal<TResult> Convert<TResult>(Func<T, TResult> converter) where TResult : IComparable<TResult> => new Ordinal<TResult>(IsReal ? converter(Value) : default, Level, IsNaN, IsPositiveInfinity, IsNegativeInfinity);

        public static bool operator !=(Ordinal<T> left, Ordinal<T> right) => left.CompareTo(right) != 0;
        public static bool operator ==(Ordinal<T> left, Ordinal<T> right) => left.CompareTo(right) == 0;
        public static bool operator <(Ordinal<T> left, Ordinal<T> right) => left.CompareTo(right) < 0;
        public static bool operator >(Ordinal<T> left, Ordinal<T> right) => left.CompareTo(right) > 0;
        public static bool operator <=(Ordinal<T> left, Ordinal<T> right) => left.CompareTo(right) <= 0;
        public static bool operator >=(Ordinal<T> left, Ordinal<T> right) => left.CompareTo(right) >= 0;

        public static implicit operator Ordinal<T>(T value) => new Ordinal<T>(value, 0);

        public static Ordinal<T> NaN = new Ordinal<T>(default, 0, true, false, false);
        public static Ordinal<T> PositiveInfinity = new Ordinal<T>(default, -1, false, true, false);
        public static Ordinal<T> NegativeInfinity = new Ordinal<T>(default, 1, false, false, true);

        public static Ordinal<T> Min(in Ordinal<T> left, in Ordinal<T> right) => left < right ? left : right;
        public static Ordinal<T> Max(in Ordinal<T> left, in Ordinal<T> right) => left > right ? left : right;

        public override string ToString() => ToString(true);
        public string ToString(bool level) => IsNaN ? "NaN" : IsPositiveInfinity ? "+∞" : IsNegativeInfinity ? "-∞" : Value.ToString() + (!level || Level == 0 ? string.Empty : $"₍{(Level > 0 ? "₊" + Level.ToSubscript() : Level.ToSubscript())}₎");
        public override bool Equals(object obj) => obj is Ordinal<T> ordinal && Equals(ordinal);
        public bool Equals(Ordinal<T> other) => EqualityComparer<T>.Default.Equals(Value, other.Value) && Level == other.Level && IsNaN == other.IsNaN && IsPositiveInfinity == other.IsPositiveInfinity && IsNegativeInfinity == other.IsNegativeInfinity;

        public override int GetHashCode()
        {
            int hashCode = 435058893;
            hashCode = hashCode * -1521134295 + EqualityComparer<T>.Default.GetHashCode(Value);
            hashCode = hashCode * -1521134295 + Level.GetHashCode();
            hashCode = hashCode * -1521134295 + IsNaN.GetHashCode();
            hashCode = hashCode * -1521134295 + IsPositiveInfinity.GetHashCode();
            hashCode = hashCode * -1521134295 + IsNegativeInfinity.GetHashCode();
            return hashCode;
        }
    }

    public abstract class Interval<T> where T : IComparable<T>
    {
        public abstract bool IsEmpty { get; }
        public abstract Interval<T> Invert { get; }
        public abstract IEnumerable<Range<T>> Ranges { get; }

        public Range<T> Container => new Range<T>(Ranges.First().m_start, Ranges.Last().m_end);

        public abstract bool Contains(Ordinal<T> value);
        public abstract bool Contains(Interval<T> value);

        public abstract Interval<T> Union(Interval<T> value);
        public abstract Interval<T> Inter(Interval<T> value);
        public abstract Interval<T> Except(Interval<T> value);

        public Interval<TResult> Convert<TResult>() where TResult : IComparable<TResult> => MultiRange<TResult>.Create(Ranges.Select(r => r.Convert<TResult>()));
        public Interval<TResult> Convert<TResult>(Func<T, TResult> converter) where TResult : IComparable<TResult> => MultiRange<TResult>.Create(Ranges.Select(r => r.Convert(converter)));

        public static Interval<T> operator +(Interval<T> left, Interval<T> right) => left.Union(right);
        public static Interval<T> operator *(Interval<T> left, Interval<T> right) => left.Inter(right);
        public static Interval<T> operator /(Interval<T> left, Interval<T> right) => left.Except(right);

        public static Range<T> OO(in Ordinal<T> start, in Ordinal<T> end) => new Range<T>(start.Next, end.Antecedent);
        public static Range<T> OC(in Ordinal<T> start, in Ordinal<T> end) => new Range<T>(start.Next, end);
        public static Range<T> CO(in Ordinal<T> start, in Ordinal<T> end) => new Range<T>(start, end.Antecedent);
        public static Range<T> CC(in Ordinal<T> start, in Ordinal<T> end) => new Range<T>(start, end);

        public static Range<T> OO(in Ordinal<T>? start, in Ordinal<T>? end) => new Range<T>(start?.Next ?? Ordinal<T>.NegativeInfinity, end?.Antecedent ?? Ordinal<T>.PositiveInfinity);
        public static Range<T> OC(in Ordinal<T>? start, in Ordinal<T>? end) => new Range<T>(start?.Next ?? Ordinal<T>.NegativeInfinity, end ?? Ordinal<T>.PositiveInfinity);
        public static Range<T> CO(in Ordinal<T>? start, in Ordinal<T>? end) => new Range<T>(start ?? Ordinal<T>.NegativeInfinity, end?.Antecedent ?? Ordinal<T>.PositiveInfinity);
        public static Range<T> CC(in Ordinal<T>? start, in Ordinal<T>? end) => new Range<T>(start ?? Ordinal<T>.NegativeInfinity, end ?? Ordinal<T>.PositiveInfinity);

        public static implicit operator Interval<T>((Ordinal<T> start, Ordinal<T> end) ends) => CC(ends.start, ends.end);
        public static implicit operator Interval<T>((Ordinal<T>? start, Ordinal<T>? end) ends) => CC(ends.start, ends.end);

        public static Range<T> EmptySet { get; } = new Range<T>(Ordinal<T>.NaN, Ordinal<T>.NaN);
        public static Range<T> NegativeReals { get; } = new Range<T>(Ordinal<T>.NegativeInfinity, default);
        public static Range<T> PositiveReals { get; } = new Range<T>(default, Ordinal<T>.PositiveInfinity);
        public static Range<T> Reals { get; } = new Range<T>(Ordinal<T>.NegativeInfinity, Ordinal<T>.PositiveInfinity);
        public static Range<T> NegativeRealsNoZero { get; } = CO(Ordinal<T>.NegativeInfinity, default);
        public static Range<T> PositiveRealsNoZero { get; } = OC(default, Ordinal<T>.PositiveInfinity);
        public static MultiRange<T> RealsNoZero { get; } = (MultiRange<T>)(NegativeRealsNoZero + PositiveRealsNoZero);
    }

    public class Range<T> : Interval<T>, IEquatable<Range<T>> where T : IComparable<T>
    {
        internal readonly Ordinal<T> m_start = Ordinal<T>.NaN;
        internal readonly Ordinal<T> m_end = Ordinal<T>.NaN;

        internal Range(in Ordinal<T> start, in Ordinal<T> end)
        {
            int comp = end.CompareTo(in start);
            if (comp >= 0 && (comp != 0 || start.IsReal))
            {
                m_start = start;
                m_end = end;
            }
        }

        public Ordinal<T> Start => m_start;
        public Ordinal<T> End => m_end;
        public override IEnumerable<Range<T>> Ranges { get { yield return this; } }

        public override bool IsEmpty => m_start.IsNaN || m_end.IsNaN;

        public override Interval<T> Invert => IsEmpty ? Reals : MultiRange<T>.Create(new[] { OO(Ordinal<T>.NegativeInfinity, m_start), OO(m_end, Ordinal<T>.PositiveInfinity) });

        public override bool Contains(Ordinal<T> value) => !IsEmpty && m_start.CompareTo(in value) <= 0 && value.CompareTo(in m_end) <= 0;
        public override bool Contains(Interval<T> value) => !IsEmpty && value.Ranges.All(range => Contains(range.m_start) && Contains(range.m_end));

        public override Interval<T> Union(Interval<T> value) => MultiRange<T>.Create(value.Ranges.Concat(this).ToArray());
        public override Interval<T> Except(Interval<T> value) => Inter(value.Invert);
        public override Interval<T> Inter(Interval<T> value) => MultiRange<T>.Create(value.Ranges.Select(r => new Range<T>(Ordinal<T>.Max(m_start, r.m_start), Ordinal<T>.Min(m_end, r.m_end))));

        public new Range<TResult> Convert<TResult>() where TResult : IComparable<TResult> => new Range<TResult>(m_start.Convert<TResult>(), m_end.Convert<TResult>());
        public new Range<TResult> Convert<TResult>(Func<T, TResult> converter) where TResult : IComparable<TResult> => new Range<TResult>(m_start.Convert(converter), m_end.Convert(converter));

        public virtual char LeftBracket => m_start.Level > 0 ? ']' : '[';
        public virtual char RightBracket => m_end.Level < 0 ? '[' : ']';

        public override string ToString() => 
            IsEmpty ? "∅" :
            this == Reals ? "ℝ" :
            this == NegativeReals ? "ℝ₋" :
            this == PositiveReals ? "ℝ₊" :
            this == NegativeRealsNoZero ? "ℝ₋*" :
            this == PositiveRealsNoZero ? "ℝ₊*" :
            m_start == m_end ? "{" + m_start + "}" :
            $"{(m_start.Level > 0 ? $"]{m_start.Antecedent}" : $"[{m_start}")} ; {(m_end.Level < 0 ? $"{m_end.Next}[" : $"{m_end}]")}";

        public override bool Equals(object obj) => Equals(obj as Range<T>);
        public bool Equals(Range<T> other) => other != null && (IsEmpty && other.IsEmpty || m_start.Equals(other.m_start) && m_end.Equals(other.m_end));

        public override int GetHashCode()
        {
            int hashCode = -1676728671;
            hashCode = hashCode * -1521134295 + EqualityComparer<Ordinal<T>>.Default.GetHashCode(m_start);
            hashCode = hashCode * -1521134295 + EqualityComparer<Ordinal<T>>.Default.GetHashCode(m_end);
            return hashCode;
        }

        public static implicit operator Range<T>((Ordinal<T> start, Ordinal<T> end) ends) => CC(ends.start, ends.end);
        public static implicit operator Range<T>((Ordinal<T>? start, Ordinal<T>? end) ends) => CC(ends.start, ends.end);

        public static bool operator ==(Range<T> left, Range<T> right) => EqualityComparer<Range<T>>.Default.Equals(left, right);
        public static bool operator !=(Range<T> left, Range<T> right) => !(left == right);
    }

    public class MultiRange<T> : Interval<T>, IEquatable<MultiRange<T>> where T : IComparable<T>
    {
        private readonly Range<T>[] m_ranges;

        private MultiRange(params Range<T>[] orderedRanges) => m_ranges = orderedRanges;

        public override IEnumerable<Range<T>> Ranges => m_ranges;
        public override bool IsEmpty => false;

        public override Interval<T> Invert => m_ranges.Select(r => r.Invert).Inter();

        public override bool Contains(Ordinal<T> value) => m_ranges.Any(r => r.Contains(value));
        public override bool Contains(Interval<T> value) => value.Ranges.All(ra => m_ranges.Any(r => r.Contains(ra)));

        public override Interval<T> Except(Interval<T> value) => Inter(value.Invert);
        public override Interval<T> Inter(Interval<T> value) => Create(m_ranges.SelectMany(r => value.Inter(r).Ranges).ToArray());
        public override Interval<T> Union(Interval<T> value) => Create(value.Ranges.Concat(m_ranges).ToArray());

        internal static Interval<T> Create(IEnumerable<Range<T>> ranges)
        {
            var rs = MergeRanges().ToArray();
            return rs.Length switch
            {
                0 => (Interval<T>)EmptySet,
                1 => rs[0],
                _ => new MultiRange<T>(rs)
            };

            IEnumerable<Range<T>> MergeRanges()
            {
                var t = EmptySet;
                foreach (var range in ranges.Where(r => !r.IsEmpty).OrderBy(r => r.m_start))
                {
                    if (t.IsEmpty) t = new Range<T>(range.m_start, range.m_end);
                    else if (range.m_start.CompareTo(in t.m_end) != 2) t = new Range<T>(t.m_start, Ordinal<T>.Max(t.m_end, range.m_end));
                    else
                    {
                        yield return t;
                        t = new Range<T>(range.m_start, range.m_end);
                    }
                }
                if (!t.IsEmpty) yield return t;
            }
        }

        public override string ToString() =>
            this == RealsNoZero ? "ℝ*" :
            string.Join<Range<T>>(" ∪ ", m_ranges);

        public override bool Equals(object obj) => Equals(obj as MultiRange<T>);
        public bool Equals(MultiRange<T> other) => other != null && EqualityComparer<Range<T>[]>.Default.Equals(m_ranges, other.m_ranges);
        public override int GetHashCode() => -1877571317 + EqualityComparer<Range<T>[]>.Default.GetHashCode(m_ranges);

        public static bool operator ==(MultiRange<T> left, MultiRange<T> right) => EqualityComparer<MultiRange<T>>.Default.Equals(left, right);
        public static bool operator !=(MultiRange<T> left, MultiRange<T> right) => !(left == right);
    }

    public class IndexRange : Range<int>
    {
        public int Length { get; }

        internal IndexRange(in Ordinal<int> start, in Ordinal<int> end) : base(start.IsReal ? new Ordinal<int>(start.Value + start.Level, 0) : start, end.IsReal ? new Ordinal<int>(end.Value + end.Level, 0) : end) => Length = m_start.IsNegativeInfinity || m_end.IsPositiveInfinity ? -1 : m_end.Value - m_start.Value;

        public override char LeftBracket => m_start.Level > 0 ? '⟧' : '⟦';
        public override char RightBracket => m_end.Level < 0 ? '⟦' : '⟧';

        public override string ToString() => base.ToString().Replace('[', '⟦').Replace(']', '⟧');
    }

    public static partial class Extensions
    {
        public static Interval<T> Union<T>(this IEnumerable<Interval<T>> source) where T : IComparable<T>
        {
            Interval<T> result = Range<T>.EmptySet;
            foreach (var interval in source) result += interval;
            return result;
        }

        public static Interval<T> Inter<T>(this IEnumerable<Interval<T>> source) where T : IComparable<T>
        {
            Interval<T> result = Range<T>.EmptySet;
            foreach (var interval in source) result *= interval;
            return result;
        }

        public static Interval<T> Union<TSource, T>(this IEnumerable<TSource> source, Func<TSource, Interval<T>> selector) where T : IComparable<T>
        {
            Interval<T> result = Range<T>.EmptySet;
            foreach (var interval in source) result += selector(interval);
            return result;
        }

        public static Interval<T> Inter<TSource, T>(this IEnumerable<TSource> source, Func<TSource, Interval<T>> selector) where T : IComparable<T>
        {
            Interval<T> result = Range<T>.EmptySet;
            foreach (var interval in source) result *= selector(interval);
            return result;
        }

        public static IndexRange IndexContainer(this Interval<int> interval) => new IndexRange(interval.Ranges.First().m_start, interval.Ranges.Last().m_end);
    }
}
