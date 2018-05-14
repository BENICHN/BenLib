using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Security.Principal;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Controls.Primitives;
using System.Windows.Documents;
using System.Windows.Threading;
using System.Diagnostics;
using System.Threading;
using System.IO;

namespace BenLib
{
    public class Collections
    {

    }

    /// <summary>
    /// <para>Notifies clients that a property value is changing, but includes extended event infomation.</para>
    /// <para>The following NotifyPropertyChanged Interface is employed when you wish to enforce the inclusion of old and
    /// new values. (Users must provide <see cref="PropertyChangedExtendedEventArgs{T}"/>, <see cref="PropertyChangedEventArgs"/> are disallowed.)</para>
    /// </summary>
    /// <typeparam name="T"/>
    public interface INotifyPropertyChangedExtended<T>
    {
        event PropertyChangedExtendedEventHandler<T> PropertyChangedExtended;
    }

    public class PropertyChangedExtendedEventArgs<T> : PropertyChangedEventArgs
    {
        public virtual T OldValue { get; private set; }
        public virtual T NewValue { get; private set; }

        public PropertyChangedExtendedEventArgs(string propertyName, T oldValue, T newValue) : base(propertyName)
        {
            OldValue = oldValue;
            NewValue = newValue;
        }
    }

    public delegate void PropertyChangedExtendedEventHandler<T>(object sender, PropertyChangedExtendedEventArgs<T> e);

    /// <summary>
    /// <para>This class adds the ability to refresh the list when any property of
    /// the objects changes in the list which implements the INotifyPropertyChanged.</para>
    /// <para>Authors : Vijaya Krishna Paruchuri, BenNat</para>
    /// <para>https://www.codeproject.com/Tips/694370/How-to-Listen-to-Property-Chang</para>
    /// </summary>
    /// <typeparam name="T1"/>
    /// <typeparam name="T2"/>
    public class ItemsChangeObservableCollection<T1, T2> : ObservableCollection<T1> where T1 : INotifyPropertyChangedExtended<T2>, INotifyPropertyChanged
    {
        public event PropertyChangedExtendedEventHandler<T2> ItemChangedEvent;

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

        private void item_PropertyChanged(object sender, PropertyChangedExtendedEventArgs<T2> e)
        {
            ItemChangedEvent?.Invoke(sender, e);
        }
    }

    public partial class Extensions
    {
        public static T FindCollectionItem<T>(this ItemCollection collection, string name) where T : FrameworkElement
        {
            try
            {
                for (int i = 0; i < collection.Count; i++)
                {
                    if (collection[i] is T fkelement && fkelement.Name == name) return collection[i] as T;
                }

                return null;
            }
            catch { return null; }
        }

        public static T FindCollectionItem<T>(this ItemCollection collection, Func<T, bool> predicate)
        {
            try { return (from T item in collection select item).FirstOrDefault(predicate); }
            catch { return default; }
        }

        public static T[] SubArray<T>(this T[] source, int index, int length)
        {
            T[] result = new T[length];
            Array.Copy(source, index, result, 0, length);
            return result;
        }

        public static T[] Merge<T>(this T[] x, T[] y)
        {
            var z = new T[x.Length + y.Length];
            x.CopyTo(z, 0);
            y.CopyTo(z, x.Length);
            return z;
        }

        public static bool IsNullOrEmpty<T>(this IEnumerable<T> collection) => collection == null || collection.Count() == 0;

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
            var lastByte = needle.Last();
            while (index < haystack.Length)
            {
                var checkByte = haystack[index];
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

            for (int i = 0; i < needle.Length; i++)
            {
                lookup[needle[i]] = needle.Length - i - 1;
            }

            int index = needle.Length - 1;
            var lastByte = needle.Last();
            while (index < haystack.Length)
            {
                var checkByte = haystack[index];
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
            while(true)
            {
                var index = haystack.SubArray(offset, haystack.Length - offset).IndexOf(needle) + offset;
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
            List<int> indexes = new List<int>();
            int offset = 0;
            while (true)
            {
                var index = await haystack.SubArray(offset, haystack.Length - offset).IndexOfAsync(needle, cancellationToken) + offset;
                if (index < offset) break;
                else
                {
                    offset = index + needle.Length;
                    indexes.Add(index);
                }
            }
            return indexes;
        }

        #region ObservableCollection.Sort

        public static void Sort<T>(this ObservableCollection<T> collection)
        {
            List<T> li = collection.ToList();
            li.Sort();

            for (int i = 0; i < li.Count; i++)
            {
                collection.Move(collection.IndexOf(li[i]), i);
            }
        }

        public static void Sort<T>(this ObservableCollection<T> collection, Comparison<T> comparison)
        {
            List<T> li = collection.ToList();
            li.Sort(comparison);

            for (int i = 0; i < li.Count; i++)
            {
                collection.Move(collection.IndexOf(li[i]), i);
            }
        }

        public static void Sort<T>(this ObservableCollection<T> collection, IComparer<T> comparer)
        {
            List<T> li = collection.ToList();
            li.Sort(comparer);

            for (int i = 0; i < li.Count; i++)
            {
                collection.Move(collection.IndexOf(li[i]), i);
            }
        }

        public static void Sort<T>(this ObservableCollection<T> collection, int index, int count, IComparer<T> comparer)
        {
            List<T> li = collection.ToList();
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
            List<T> li = collection.OrderBy(keySelector).ToList();

            for (int i = 0; i < li.Count; i++)
            {
                collection.Move(collection.IndexOf(li[i]), i);
            }
        }

        public static void OrderByVoid<T, TKey>(this ObservableCollection<T> collection, Func<T, TKey> keySelector, IComparer<TKey> comparer)
        {
            List<T> li = collection.OrderBy(keySelector, comparer).ToList();

            for (int i = 0; i < li.Count; i++)
            {
                collection.Move(collection.IndexOf(li[i]), i);
            }
        }

        public static void OrderByDescendingVoid<T, TKey>(this ObservableCollection<T> collection, Func<T, TKey> keySelector)
        {
            List<T> li = collection.OrderByDescending(keySelector).ToList();

            for (int i = 0; i < li.Count; i++)
            {
                collection.Move(collection.IndexOf(li[i]), i);
            }
        }

        public static void OrderByDescendingVoid<T, TKey>(this ObservableCollection<T> collection, Func<T, TKey> keySelector, IComparer<TKey> comparer)
        {
            List<T> li = collection.OrderByDescending(keySelector, comparer).ToList();

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

        public static bool IsSorted<T>(this IEnumerable<T> collection, IComparer<T> comparer)
        {
            int collectioncount = collection.Count();

            for (int i = 0; i < collectioncount; i++)
            {
                if (i < collectioncount - 1 && comparer.Compare(collection.ElementAt(i), collection.ElementAt(i + 1)) == 1) return false;
            }

            return true;
        }

        public static bool IsSorted<T>(this IEnumerable<T> collection, Comparison<T> comparison)
        {
            int collectioncount = collection.Count();

            for (int i = 0; i < collectioncount; i++)
            {
                if (i < collectioncount - 1 && comparison.Invoke(collection.ElementAt(i), collection.ElementAt(i + 1)) == 1) return false;
            }

            return true;
        }

        public static bool IsSorted<T>(this IEnumerable<T> collection, int index, int count, IComparer<T> comparer)
        {
            int collectioncount = collection.Count();

            for (int i = index; i < index + count; i++)
            {
                if (i < collectioncount - 1 && comparer.Compare(collection.ElementAt(i), collection.ElementAt(i + 1)) == 1) return false;
            }

            return true;
        }

        #endregion

        public static ObservableCollection<T> TrimExcess<T>(this ObservableCollection<T> collection)
        {
            List<T> li = collection.ToList();
            li.TrimExcess();

            return new ObservableCollection<T>(li);
        }

        public static void Reselect(this ListViewItem lvi)
        {
            if (lvi.IsSelected) { lvi.IsSelected = true; lvi.IsSelected = false; }
        }

        private static List<KeyValuePair<object, object>> KeyValuePairs(this ResourceDictionary resourceDictionary)
        {
            var list = new List<KeyValuePair<object, object>>();

            list.AddRange(resourceDictionary.OfType<DictionaryEntry>().Select(entry => new KeyValuePair<object, object>(entry.Key, entry.Value)));
            foreach (ResourceDictionary resdict in resourceDictionary.MergedDictionaries) list.AddRange(resdict.KeyValuePairs());

            return list;
        }

        public static Dictionary<object, object> ToDictionary(this ResourceDictionary resourceDictionary) => resourceDictionary.KeyValuePairs().GroupBy(kvp => kvp.Key).ToDictionary(group => group.Key, group => group.Last().Value);
    }
}
