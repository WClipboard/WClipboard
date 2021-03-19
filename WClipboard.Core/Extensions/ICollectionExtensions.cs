using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace WClipboard.Core.Extensions
{
    public static class ICollectionExtensions
    {
        public static void AddRange<T>(this ICollection<T> collection, IEnumerable<T> itemsToAdd)
        {
            if (collection is List<T> list)
            {
                list.AddRange(itemsToAdd);
            }
            else
            {
                foreach (var item in itemsToAdd)
                {
                    collection.Add(item);
                }
            }
        }

        public static IEnumerable<(T item, bool isRemoved)> RemoveRange<T>(this ICollection<T> collection, IEnumerable<T> itemsToRemove)
        {
            foreach(var item in itemsToRemove)
            {
                yield return (item, collection.Remove(item));
            }
        }

        public static int RemoveAll<T>(this ICollection<T> collection, Predicate<T> shouldRemove)
        {
            if (collection is List<T> list)
            {
                return list.RemoveAll(shouldRemove);
            }
            else
            {
                var itemsToRemove = collection.Where(i => shouldRemove(i)).ToList();
                foreach (var item in itemsToRemove)
                {
                    collection.Remove(item);
                }
                return itemsToRemove.Count;
            }
        }

        public static int FirstIndexOf<T>(this IList<T> list, Predicate<T> condition)
        {
            for(int i = 0; i < list.Count; i++)
            {
                if(condition(list[i]))
                {
                    return i;
                }
            }

            return -1;
        }

        public static IReadOnlyCollection<T> Cast<T>(this ICollection collection)
        {
            if (collection is IReadOnlyCollection<T> castedCollection)
            {
                return castedCollection;
            }
            else
            {
                return ((IEnumerable)collection).Cast<T>().ToList(collection.Count);
            }
        }
    }
}
