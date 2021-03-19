using System;
using System.Collections.Generic;

namespace WClipboard.Core.Utilities
{
    public class ObjectTypeCollection : KeyedCollectionFunc<Type, object>, IServiceProvider
    {
        public ObjectTypeCollection() : base((obj) => obj.GetType())
        {
        }

        public ObjectTypeCollection(IEnumerable<object> items) : base((obj) => obj.GetType(), items)
        {
        }

        public object? GetService(Type serviceType)
        {
            if (TryGetValue(serviceType, out object value))
            {
                return value;
            }
            else
            {
                return null;
            }
        }
    }
}
