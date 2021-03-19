using System;
using System.Collections.Generic;

namespace WClipboard.Core.Utilities
{
    public class ObservableObjectTypeCollection<T> : ObservableKeyedCollectionFunc<Type, T>, IServiceProvider where T : notnull
    {
        public ObservableObjectTypeCollection() : base((obj) => obj.GetType())
        {
        }

        public ObservableObjectTypeCollection(IEnumerable<T> items) : base((obj) => obj.GetType(), items)
        {
        }

        public object? GetService(Type serviceType)
        {
            if (TryGetValue(serviceType, out T value))
            {
                return value;
            }
            else
            {
                return null;
            }
        }

        public bool TryGetValue<TValue>(out TValue value) where TValue : T
        {
            var result = TryGetValue(typeof(TValue), out var tValue);
            value = (TValue)tValue;
            return result;
        }
    }

    public class ObservableObjectTypeCollection : ObservableObjectTypeCollection<object>, IServiceProvider
    {
        public ObservableObjectTypeCollection() : base()
        {
        }

        public ObservableObjectTypeCollection(IEnumerable<object> items) : base(items)
        {
        }
    }
}
