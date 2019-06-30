using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace BenLib.Framework
{
    public static partial class Extensions
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

        public static void Reselect(this ListViewItem lvi)
        {
            if (lvi.IsSelected) { lvi.IsSelected = true; lvi.IsSelected = false; }
        }

        private static IEnumerable<KeyValuePair<object, object>> GetKeyValuePairs(this ResourceDictionary resourceDictionary) => resourceDictionary.OfType<DictionaryEntry>().Select(entry => new KeyValuePair<object, object>(entry.Key, entry.Value)).Concat(resourceDictionary.MergedDictionaries.SelectMany(resdict => resdict.GetKeyValuePairs()));
        public static Dictionary<object, object> ToDictionary(this ResourceDictionary resourceDictionary) => resourceDictionary.GetKeyValuePairs().GroupBy(kvp => kvp.Key).ToDictionary(group => group.Key, group => group.Last().Value);
    }
}
