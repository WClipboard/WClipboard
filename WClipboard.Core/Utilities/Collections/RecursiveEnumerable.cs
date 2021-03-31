using System;
using System.Collections.Generic;

namespace WClipboard.Core.Utilities.Collections
{
    public static class RecursiveEnumerable
    {
        public static IEnumerable<T> Get<T>(T start, Func<T, T> next, T end)
        {
            if (next == null)
            {
                throw new ArgumentNullException(nameof(next));
            }

            for(var item = start; !Equals(item, end); item = next(item))
            {
                yield return item;
            }
        }
    }
}
