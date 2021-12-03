using System;
using System.Collections.Generic;
using System.Linq;

namespace WClipboard.Core.Extensions
{
    public static class IEnumerableExtensions
    {
        public static List<T> ToList<T>(this IEnumerable<T> collection, int count)
        {
            var returner = new List<T>(count);
            returner.AddRange(collection);
            return returner;
        }

        /// <summary>
        /// ToDictionary Linq extension, what ignores duplicates, the value of key is set to the last value for that key in collection.
        /// </summary>
        /// <typeparam name="T">Source type</typeparam>
        /// <typeparam name="TKey">Key type of source type</typeparam>
        /// <typeparam name="TValue">Value type of source type</typeparam>
        /// <param name="collection">The source collection</param>
        /// <param name="keySelecter">Function to select the key of a source item</param>
        /// <param name="valueSelecter">Function to select the value of a source item</param>
        /// <returns>A Dictionary, with all keys selected by the <paramref name="keySelecter"/> of each item in <paramref name="collection"/>, with the last value for each key from the <paramref name="valueSelecter"/> in the <paramref name="collection"/></returns>
        public static Dictionary<TKey, TValue> ToDictionaryLast<T, TKey, TValue>(this IEnumerable<T> collection, Func<T, TKey> keySelecter, Func<T, TValue> valueSelecter) where TKey : notnull
        {
            var returner = new Dictionary<TKey, TValue>();
            foreach(var item in collection)
            {
                returner[keySelecter(item)] = valueSelecter(item);
            }
            return returner;
        }

        public static IEnumerable<T> NotNull<T>(this IEnumerable<T?> collection) where T : class
        {
            foreach(var item in collection)
            {
                if (item != null)
                {
                    yield return item;
                }
            }
        }
    }
}
