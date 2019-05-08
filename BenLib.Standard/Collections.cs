using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using static BenLib.Standard.Interval<int>;

namespace BenLib.Standard
{
    public static class Collections
    {
        public static IEnumerable<T> CreateFilledCollection<T>(int count) where T : new()
        {
            for (int i = 0; i < count; i++) yield return new T();
        }

        public static IEnumerable<double> DoubleRange(double start, double length, double step)
        {
            double end = start + length;
            for (double i = start; i < end; i += step) yield return i;
        }

        public static IEnumerable<decimal> DecimalRange(decimal start, decimal length, decimal step)
        {
            decimal end = start + length;
            for (decimal i = start; i < end; i += step) yield return i;
        }

        public static IEnumerable<int> Range(int start, int count) { for (int i = start; i < start + count; i++) yield return i; }
        public static IEnumerable<uint> Range(uint start, uint count) { for (uint i = start; i < start + count; i++) yield return i; }
        public static IEnumerable<short> Range(short start, short count) { for (short i = start; i < start + count; i++) yield return i; }
        public static IEnumerable<ushort> Range(ushort start, ushort count) { for (ushort i = start; i < start + count; i++) yield return i; }
        public static IEnumerable<long> Range(long start, long count) { for (long i = start; i < start + count; i++) yield return i; }
        public static IEnumerable<ulong> Range(ulong start, ulong count) { for (ulong i = start; i < start + count; i++) yield return i; }

        public static NotifyCollectionChangedEventArgs CollectionAdd(object newItem, int index) => new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, newItem, index);
        public static NotifyCollectionChangedEventArgs CollectionAdd(IList newItems, int startingIndex) => new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, newItems, startingIndex);
        public static NotifyCollectionChangedEventArgs CollectionAdd(object newItem) => new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, newItem);
        public static NotifyCollectionChangedEventArgs CollectionAdd(IList newItems) => new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, newItems);

        public static NotifyCollectionChangedEventArgs CollectionRemove(object oldItem, int index) => new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, oldItem, index);
        public static NotifyCollectionChangedEventArgs CollectionRemove(IList oldItems, int startingIndex) => new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, oldItems, startingIndex);
        public static NotifyCollectionChangedEventArgs CollectionRemove(object oldItem) => new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, oldItem);
        public static NotifyCollectionChangedEventArgs CollectionRemove(IList oldItems) => new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, oldItems);

        public static NotifyCollectionChangedEventArgs CollectionReplace(object newItem, object oldItem, int index) => new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Replace, newItem, oldItem, index);
        public static NotifyCollectionChangedEventArgs CollectionReplace(IList newItems, IList oldItems, int startingIndex) => new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Replace, newItems, oldItems, startingIndex);
        public static NotifyCollectionChangedEventArgs CollectionReplace(object newItem, object oldItem) => new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Replace, newItem, oldItem);
        public static NotifyCollectionChangedEventArgs CollectionReplace(IList newItems, IList oldItems) => new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Replace, newItems, oldItems);

        public static NotifyCollectionChangedEventArgs CollectionMove(object changedItem, int newIndex, int oldIndex) => new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Move, changedItem, newIndex, oldIndex);
        public static NotifyCollectionChangedEventArgs CollectionMove(IList changedItems, int newStartingIndex, int oldStartingIndex) => new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Move, changedItems, newStartingIndex, oldStartingIndex);

        public static NotifyCollectionChangedEventArgs CollectionReset() => new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset);
    }

    /// <summary>
    /// <para>Notifies clients that a property value is changing, but includes extended event infomation.</para>
    /// <para>The following NotifyPropertyChanged Interface is employed when you wish to enforce the inclusion of old and
    /// new values. (Users must provide <see cref="PropertyChangedExtendedEventArgs{T}"/>, <see cref="PropertyChangedEventArgs"/> are disallowed.)</para>
    /// </summary>
    /// <typeparam name="T"/>
    public interface INotifyPropertyChangedExtended<T> { event PropertyChangedExtendedEventHandler<T> PropertyChangedExtended; }

    public class PropertyChangedExtendedEventArgs<T> : PropertyChangedEventArgs
    {
        public virtual T OldValue { get; }
        public virtual T NewValue { get; }

        public PropertyChangedExtendedEventArgs(string propertyName, T oldValue, T newValue) : base(propertyName)
        {
            OldValue = oldValue;
            NewValue = newValue;
        }
    }

    public interface INotifyItemChanged { event PropertyChangedEventHandler ItemChanged; }
    public interface INotifyItemChangedExtended<T> { event PropertyChangedExtendedEventHandler<T> ItemChanged; }

    public delegate void PropertyChangedExtendedEventHandler<T>(object sender, PropertyChangedExtendedEventArgs<T> e);

    /// <summary>
    /// <para>This class adds the ability to refresh the list when any property of
    /// the objects changes in the list which implements the INotifyPropertyChanged.</para>
    /// <para>Authors : Vijaya Krishna Paruchuri, BenNat</para>
    /// <para>https://www.codeproject.com/Tips/694370/How-to-Listen-to-Property-Chang</para>
    /// </summary>
    /// <typeparam name="T1"/>
    /// <typeparam name="T2"/>
    public class ItemsChangeObservableCollection<T1, T2> : ObservableCollection<T1>, INotifyItemChangedExtended<T2> where T1 : INotifyPropertyChangedExtended<T2>, INotifyPropertyChanged
    {
        public event PropertyChangedExtendedEventHandler<T2> ItemChanged;

        protected override void OnCollectionChanged(NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == NotifyCollectionChangedAction.Add)
            {
                RegisterPropertyChanged(e.NewItems);
            }
            else if (e.Action == NotifyCollectionChangedAction.Remove)
            {
                UnRegisterPropertyChanged(e.OldItems);
            }
            else if (e.Action == NotifyCollectionChangedAction.Replace)
            {
                UnRegisterPropertyChanged(e.OldItems);
                RegisterPropertyChanged(e.NewItems);
            }

            base.OnCollectionChanged(e);
        }

        protected override void ClearItems()
        {
            UnRegisterPropertyChanged(this);
            base.ClearItems();
        }

        private void RegisterPropertyChanged(IList items)
        {
            foreach (INotifyPropertyChangedExtended<T2> item in items)
            {
                if (item != null)
                {
                    item.PropertyChangedExtended += item_PropertyChanged;
                }
            }
        }

        private void UnRegisterPropertyChanged(IList items)
        {
            foreach (INotifyPropertyChangedExtended<T2> item in items)
            {
                if (item != null)
                {
                    item.PropertyChangedExtended -= item_PropertyChanged;
                }
            }
        }

        private void item_PropertyChanged(object sender, PropertyChangedExtendedEventArgs<T2> e) => ItemChanged?.Invoke(sender, e);
    }

    public partial class Extensions
    {
        public static T[] SubArray<T>(this T[] source, int index, int length)
        {
            var result = new T[length];
            Array.Copy(source, index, result, 0, length);
            return result;
        }

        public static IEnumerable<IEnumerable<T>> SubCollections<T>(this IEnumerable<T> source, Interval<int> interval, bool allowExcess)
        {
            var e = source.GetEnumerator();
            int i = 0;
            foreach (var range in interval.Ranges) yield return GetSubCollection(range);

            IEnumerable<T> GetSubCollection(Range<int> range)
            {
                if (range.IsEmpty) yield break;

                if (source is IList<T> list)
                {
                    if (!allowExcess && range.End.IsReal && list.Count <= range.End.Value) throw new ArgumentOutOfRangeException("source");
                    foreach (int index in range.Numbers(0, list.Count - 1)) yield return list[index];
                }
                else
                {
                    int index = Ordinal<int>.Max(Ordinal<int>.Zero, range.Start).Value;

                    while (i < index)
                    {
                        if (!e.MoveNext())
                        {
                            if (allowExcess) yield break;
                            else throw new ArgumentOutOfRangeException("source");
                        }
                        i++;
                    }

                    if (range.End.IsPositiveInfinity) while (e.MoveNext()) yield return e.Current;
                    else
                    {
                        int end = range.End.Value;
                        while (i <= end)
                        {
                            if (e.MoveNext()) yield return e.Current;
                            else
                            {
                                if (allowExcess) yield break;
                                else throw new ArgumentOutOfRangeException("source");
                            }
                            i++;
                        }
                    }
                }
            }
        }
        public static IEnumerable<T> SubCollection<T>(this IEnumerable<T> source, Interval<int> interval, bool allowExcess) => source.SubCollections(interval, allowExcess).SelectMany(collection => collection);
        public static IEnumerable<T> SubCollection<T>(this IEnumerable<T> source, int index, int length, bool allowExcess) => SubCollection(source, CO(index, index + length), allowExcess);

        public static T[] MergeArray<T>(this T[] x, T[] y)
        {
            var z = new T[x.Length + y.Length];
            x.CopyTo(z, 0);
            y.CopyTo(z, x.Length);
            return z;
        }

        public static T[] Reverse<T>(this T[] array)
        {
            Array.Reverse(array);
            return array;
        }

        public static IEnumerable<T> DynamicCast<T>(this IEnumerable source) { foreach (object current in source) yield return (T)current; }

        public static bool IsNullOrEmpty<T>(this IEnumerable<T> collection) => collection == null || !collection.Any();

        public static bool IsNullOrEmpty<T>(this ICollection<T> collection) => collection == null || collection.Count == 0;

        public static bool Contains<T>(this T[] source, T value) => Array.IndexOf(source, value) >= 0;

        public static IEnumerable<T> Fill<T>(this ICollection<T> collection) where T : new()
        {
            foreach (var t in collection) yield return new T();
        }

        public static IEnumerable<T> Fill<T>(this ICollection<T> collection, Func<int, T> fillFunction)
        {
            int i = 0;
            foreach (var t in collection)
            {
                yield return fillFunction(i);
                i++;
            }
        }

        public static void ForEach<T>(this IEnumerable<T> collection, Action<T, int> action)
        {
            int i = 0;
            foreach (var t in collection)
            {
                action(t, i);
                i++;
            }
        }
        public static void ForEach<T>(this IEnumerable<T> collection, Action<int> action)
        {
            int i = 0;
            foreach (var t in collection)
            {
                action(i);
                i++;
            }
        }
        public static void ForEach<T>(this IEnumerable<T> collection, Action<T> action)
        {
            int i = 0;
            foreach (var t in collection)
            {
                action(t);
                i++;
            }
        }

        public static IEnumerable<(T From, T To)> ExpandOrContract<T>(this IList<T> list1, (int Start, int End) range1, IList<T> list2, (int Start, int End) range2)
        {
            int count1 = range1.End - range1.Start + 1;
            int count2 = range2.End - range2.Start + 1;

            if (count1 == count2)
            {
                int offset = range2.Start - range1.Start;
                for (int i = range1.Start; i <= range1.End; i++) yield return (list1[i], list2[offset + i]);
            }
            else
            {
                int big = Math.Max(count1, count2);
                int small = Math.Min(count1, count2);

                bool range1SmallerThanRange2 = count1 == small;

                int quotient = big / small;
                int rest = big - small * quotient;

                int currentIndex = 0;

                if (range1SmallerThanRange2)
                {
                    int offset = range2.Start;
                    for (int i = range1.Start; i <= range1.End; i++)
                    {
                        for (int j = 0; j < quotient; j++)
                        {
                            yield return (list1[i], list2[offset + currentIndex]);
                            currentIndex++;
                        }
                        if (rest > 0)
                        {
                            yield return (list1[i], list2[offset + currentIndex]);
                            currentIndex++;
                            rest--;
                        }
                    }
                }
                else
                {
                    int offset = range1.Start;
                    for (int i = range2.Start; i <= range2.End; i++)
                    {
                        for (int j = 0; j < quotient; j++)
                        {
                            yield return (list1[offset + currentIndex], list2[i]);
                            currentIndex++;
                        }
                        if (rest > 0)
                        {
                            yield return (list1[offset + currentIndex], list2[i]);
                            currentIndex++;
                            rest--;
                        }
                    }
                }
            }
        }

        public static IEnumerable<T> Replace<T>(this IEnumerable<T> source, Predicate<T> predicate, Func<T, IEnumerable<T>> replacement)
        {
            var enumerator = source.GetEnumerator();

            while (enumerator.MoveNext())
            {
                var current = enumerator.Current;
                if (predicate(current)) foreach (var item in replacement(current)) yield return item;
                else yield return current;
            }
        }

        public static IEnumerable<T> Replace<T>(this IEnumerable<T> source, Predicate<T> predicate, Func<T, T> replacement)
        {
            var enumerator = source.GetEnumerator();

            while (enumerator.MoveNext())
            {
                var current = enumerator.Current;
                yield return predicate(current) ? replacement(current) : current;
            }
        }

        public static IEnumerable<T> Replace<T>(this IEnumerable<T> source, T toReplace, Func<T, IEnumerable<T>> replacement)
        {
            var enumerator = source.GetEnumerator();

            while (enumerator.MoveNext())
            {
                var current = enumerator.Current;
                if (current.Equals(toReplace)) foreach (var item in replacement(current)) yield return item;
                else yield return current;
            }
        }

        public static IEnumerable<T> Replace<T>(this IEnumerable<T> source, T toReplace, Func<T, T> replacement)
        {
            var enumerator = source.GetEnumerator();

            while (enumerator.MoveNext())
            {
                var current = enumerator.Current;
                yield return current.Equals(toReplace) ? replacement(current) : current;
            }
        }

        public static int IndexOf<T>(this T[] array, T value) => Array.IndexOf(array, value);

        public static int IndexOf(this byte[] haystack, byte[] needle)
        {
            int[] lookup = new int[256];
            for (int i = 0; i < lookup.Length; i++) { lookup[i] = needle.Length; }

            for (int i = 0; i < needle.Length; i++)
            {
                lookup[needle[i]] = needle.Length - i - 1;
            }

            int index = needle.Length - 1;
            byte lastByte = needle.Last();
            while (index < haystack.Length)
            {
                byte checkByte = haystack[index];
                if (checkByte == lastByte)
                {
                    bool found = true;
                    for (int j = needle.Length - 2; j >= 0; j--)
                    {
                        if (haystack[index - needle.Length + j + 1] != needle[j])
                        {
                            found = false;
                            break;
                        }
                    }

                    if (found)
                        return index - needle.Length + 1;
                    else
                        index++;
                }
                else
                {
                    index += lookup[checkByte];
                }
            }
            return -1;
        }

        public static Task<int> IndexOfAsync(this byte[] haystack, byte[] needle, CancellationToken cancellationToken = default) => Task.Run(() =>
        {
            int[] lookup = new int[256];
            for (int i = 0; i < lookup.Length; i++) { lookup[i] = needle.Length; }

            for (int i = 0; i < needle.Length; i++) lookup[needle[i]] = needle.Length - i - 1;

            int index = needle.Length - 1;
            byte lastByte = needle.Last();
            while (index < haystack.Length)
            {
                byte checkByte = haystack[index];
                if (haystack[index] == lastByte)
                {
                    bool found = true;
                    for (int j = needle.Length - 2; j >= 0; j--)
                    {
                        if (haystack[index - needle.Length + j + 1] != needle[j])
                        {
                            found = false;
                            break;
                        }
                    }

                    if (found)
                        return index - needle.Length + 1;
                    else
                        index++;
                }
                else
                {
                    index += lookup[checkByte];
                }
            }
            return -1;
        }, cancellationToken);

        public static IEnumerable<int> AllIndexesOf(this byte[] haystack, byte[] needle)
        {
            int offset = 0;
            while (true)
            {
                int index = haystack.SubArray(offset, haystack.Length - offset).IndexOf(needle) + offset;
                if (index < offset) break;
                else
                {
                    offset = index + needle.Length;
                    yield return index;
                }
            }
        }

        public static async Task<List<int>> AllIndexesOfAsync(this byte[] haystack, byte[] needle, CancellationToken cancellationToken = default)
        {
            var indexes = new List<int>();
            int offset = 0;
            while (true)
            {
                int index = await haystack.SubArray(offset, haystack.Length - offset).IndexOfAsync(needle, cancellationToken) + offset;
                if (index < offset) break;
                else
                {
                    offset = index + needle.Length;
                    indexes.Add(index);
                }
            }
            return indexes;
        }

        public static IEnumerable<int> AllIndexesOf<T>(this IList<T> collection, IList<T> subCollection)
        {
            int length = subCollection.Count;
            int i = 0;

            while (i + length <= collection.Count)
            {
                if (collection.SubCollection(i, length, false).SequenceEqual(subCollection))
                {
                    yield return i;
                    i += length;
                }
                else i++;
            }
        }

        public static IEnumerable<int> AllIndexesOf<T>(this IList<T> collection, IList<T> subCollection, Func<T, T, bool> predicate)
        {
            int length = subCollection.Count;
            int i = 0;

            while (i + length <= collection.Count)
            {
                int j = 0;
                if (subCollection.All(t => predicate(t, collection[i + j++])))
                {
                    yield return i;
                    i += length;
                }
                else i++;
            }
        }

        public static IEnumerable<T> TakeLast<T>(this IEnumerable<T> source, int count)
        {
            if (source == null) throw new ArgumentNullException("source");
            if (count < 0) throw new ArgumentOutOfRangeException("count");

            if (count == 0) return null;

            if (source is ICollection<T> collection)
            {
                int index = Math.Max(collection.Count - count, 0);
                return source.SubCollection(CC(index, null), false);
            }

            var queue = new Queue<T>(count);

            foreach (var t in source)
            {
                if (queue.Count == count) queue.Dequeue();
                queue.Enqueue(t);
            }

            return queue.ToArray();
        }

        public static void RemoveAll<T>(this ICollection<T> source, Predicate<T> match)
        {
            if (source is List<T> list) list.RemoveAll(match);
            if (source is IList<T> ilist) for (int i = ilist.Count - 1; i >= 0; i--) if (match(ilist[i])) ilist.RemoveAt(i);
                    else foreach (var item in source.ToArray()) if (match(item)) source.Remove(item);
        }

        public static void AddRange<T>(this ICollection<T> source, IEnumerable<T> collection)
        {
            if (source is List<T> list) list.AddRange(collection);
            else foreach (var item in collection) source.Add(item);
        }

        #region ObservableCollection.Sort

        public static void Sort<T>(this ObservableCollection<T> collection)
        {
            var li = collection.ToList();
            li.Sort();

            for (int i = 0; i < li.Count; i++)
            {
                collection.Move(collection.IndexOf(li[i]), i);
            }
        }

        public static void Sort<T>(this ObservableCollection<T> collection, Comparison<T> comparison)
        {
            var li = collection.ToList();
            li.Sort(comparison);

            for (int i = 0; i < li.Count; i++)
            {
                collection.Move(collection.IndexOf(li[i]), i);
            }
        }

        public static void Sort<T>(this ObservableCollection<T> collection, IComparer<T> comparer)
        {
            var li = collection.ToList();
            li.Sort(comparer);

            for (int i = 0; i < li.Count; i++)
            {
                collection.Move(collection.IndexOf(li[i]), i);
            }
        }

        public static void Sort<T>(this ObservableCollection<T> collection, int index, int count, IComparer<T> comparer)
        {
            var li = collection.ToList();
            li.Sort(index, count, comparer);

            for (int i = 0; i < li.Count; i++)
            {
                collection.Move(collection.IndexOf(li[i]), i);
            }
        }

        #endregion

        #region ObservableCollection.OrderByVoid

        public static void OrderByVoid<T, TKey>(this ObservableCollection<T> collection, Func<T, TKey> keySelector)
        {
            var li = collection.OrderBy(keySelector).ToList();

            for (int i = 0; i < li.Count; i++)
            {
                collection.Move(collection.IndexOf(li[i]), i);
            }
        }

        public static void OrderByVoid<T, TKey>(this ObservableCollection<T> collection, Func<T, TKey> keySelector, IComparer<TKey> comparer)
        {
            var li = collection.OrderBy(keySelector, comparer).ToList();

            for (int i = 0; i < li.Count; i++)
            {
                collection.Move(collection.IndexOf(li[i]), i);
            }
        }

        public static void OrderByDescendingVoid<T, TKey>(this ObservableCollection<T> collection, Func<T, TKey> keySelector)
        {
            var li = collection.OrderByDescending(keySelector).ToList();

            for (int i = 0; i < li.Count; i++)
            {
                collection.Move(collection.IndexOf(li[i]), i);
            }
        }

        public static void OrderByDescendingVoid<T, TKey>(this ObservableCollection<T> collection, Func<T, TKey> keySelector, IComparer<TKey> comparer)
        {
            var li = collection.OrderByDescending(keySelector, comparer).ToList();

            for (int i = 0; i < li.Count; i++)
            {
                collection.Move(collection.IndexOf(li[i]), i);
            }
        }

        #endregion

        #region IsOrderedBy

        public static bool IsOrderedBy<T, TKey>(this IEnumerable<T> collection, Func<T, TKey> keySelector) => collection.OrderBy(keySelector).SequenceEqual(collection);

        public static bool IsOrderedBy<T, TKey>(this IEnumerable<T> collection, Func<T, TKey> keySelector, IComparer<TKey> comparer) => collection.OrderBy(keySelector, comparer).SequenceEqual(collection);

        public static bool IsOrderedByDescending<T, TKey>(this IEnumerable<T> collection, Func<T, TKey> keySelector) => collection.OrderByDescending(keySelector).SequenceEqual(collection);

        public static bool IsOrderedByDescending<T, TKey>(this IEnumerable<T> collection, Func<T, TKey> keySelector, IComparer<TKey> comparer) => collection.OrderByDescending(keySelector, comparer).SequenceEqual(collection);

        #endregion

        #region IsSorted

        public static bool IsSorted<T>(this IList<T> list) where T : IComparable<T>
        {
            int listCount = list.Count;

            for (int i = 0; i < listCount; i++)
            {
                if (i < listCount - 1 && list[i].CompareTo(list[i + 1]) == 1) return false;
            }

            return true;
        }
        public static bool IsSorted<T>(this IList<T> list, IComparer<T> comparer)
        {
            int listCount = list.Count;

            for (int i = 0; i < listCount; i++)
            {
                if (i < listCount - 1 && comparer.Compare(list[i], list[i + 1]) == 1) return false;
            }

            return true;
        }
        public static bool IsSorted<T>(this IList<T> list, Comparison<T> comparison)
        {
            int listCount = list.Count;

            for (int i = 0; i < listCount; i++)
            {
                if (i < listCount - 1 && comparison.Invoke(list[i], list[i + 1]) == 1) return false;
            }

            return true;
        }
        public static bool IsSorted<T>(this IList<T> list, int index, int count, IComparer<T> comparer)
        {
            int listCount = list.Count;

            for (int i = index; i < index + count; i++)
            {
                if (i < listCount - 1 && comparer.Compare(list[i], list[i + 1]) == 1) return false;
            }

            return true;
        }

        #endregion

        public static ObservableCollection<T> TrimExcess<T>(this ObservableCollection<T> collection)
        {
            var li = collection.ToList();
            li.TrimExcess();

            return new ObservableCollection<T>(li);
        }

        public static IEnumerable<double> Operate(this IEnumerable<double> source, IEnumerable<double> values, Func<double, double, double> operation)
        {
            var valuesEnumerator = values.GetEnumerator();
            foreach (double val in source)
            {
                valuesEnumerator.MoveNext();
                yield return operation(val, valuesEnumerator.Current);
            }
        }
        public static IEnumerable<double> Operate(this IEnumerable<double> source, double value, Func<double, double, double> operation) { foreach (double val in source) yield return operation(val, value); }

        public static void Enumerate<T>(this IEnumerable<T> source)
        {
            var enumerator = source.GetEnumerator();
            while (enumerator.MoveNext()) ;
        }

        public static Dictionary<TKey, TValue> ToDictionary<TKey, TValue>(this IEnumerable<(TKey key, TValue value)> source) => source.ToDictionary(t => t.key, t => t.value);
    }

    public struct MultiEnumerator<T> : IEnumerator<T>
    {
        public MultiEnumerator(params IEnumerator<T>[] enumeratorsArray) : this(enumerators: enumeratorsArray) { }
        public MultiEnumerator(IEnumerable<IEnumerator<T>> enumerators)
        {
            Enumerators = enumerators;
            m_numeratorsEnumerator = enumerators.GetEnumerator();
            m_numeratorsEnumerator.MoveNext();
        }

        public IEnumerable<IEnumerator<T>> Enumerators { get; }
        private readonly IEnumerator<IEnumerator<T>> m_numeratorsEnumerator;

        public T Current => m_numeratorsEnumerator.Current.Current;
        object IEnumerator.Current => Current;

        public void Dispose()
        {
            foreach (var enumerator in Enumerators) enumerator.Dispose();
            m_numeratorsEnumerator.Dispose();
        }

        public bool MoveNext() => m_numeratorsEnumerator.Current.MoveNext() ? true : m_numeratorsEnumerator.MoveNext() ? MoveNext() : false;

        public void Reset()
        {
            foreach (var enumerator in Enumerators) enumerator.Reset();
            m_numeratorsEnumerator.Reset();
        }
    }

    public struct MultiEnumerator : IEnumerator
    {
        public MultiEnumerator(params IEnumerator[] enumeratorsArray) : this(enumerators: enumeratorsArray) { }
        public MultiEnumerator(IEnumerable<IEnumerator> enumerators) : this()
        {
            Enumerators = enumerators;
            m_numeratorsEnumerator = enumerators.GetEnumerator();
            m_numeratorsEnumerator.MoveNext();
        }

        public IEnumerable<IEnumerator> Enumerators { get; }
        private readonly IEnumerator<IEnumerator> m_numeratorsEnumerator;

        public object Current => m_numeratorsEnumerator.Current.Current;

        public bool MoveNext() => m_numeratorsEnumerator.Current.MoveNext() ? true : m_numeratorsEnumerator.MoveNext() ? MoveNext() : false;

        public void Reset()
        {
            foreach (var enumerator in Enumerators) enumerator.Reset();
            m_numeratorsEnumerator.Reset();
        }
    }
}
