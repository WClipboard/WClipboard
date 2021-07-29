using System;
using System.Collections.Generic;

namespace WClipboard.Core.Utilities.Collections
{
    public static class RecursiveEnumerable
    {
        public static IEnumerable<T> Get<T>(T start, Func<T, T> next, T end)
        {
            for(var item = start; !Equals(item, end); item = next(item))
            {
                yield return item;
            }
        }

        public static IEnumerable<T> While<T>(T start, Predicate<T> has_next, Func<T, T> next)
        {
            var item = start;
            yield return item;
            while (has_next(item))
            {
                item = next(item);
                yield return item;
            }
        }
    }
}
