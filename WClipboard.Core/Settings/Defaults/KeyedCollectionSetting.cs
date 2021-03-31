using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Microsoft.Extensions.DependencyInjection;
using WClipboard.Core.DI;
using WClipboard.Core.Utilities.Collections;

namespace WClipboard.Core.Settings.Defaults
{
    public interface IKeyedCollectionSetting<TKey, TItem> : IResolveableIOSetting, IEnumerable<TItem>
    {
        new TKey Value { get; set; }
        TKey DefaultValue { get; }
        new TItem GetResolvedValue();
        void SetResolvedValue(TItem value);
    }

    public class KeyedCollectionSetting<TKey, TItem, TService> : IKeyedCollectionSetting<TKey, TItem> where TService : IReadOnlyKeyedCollection<TKey, TItem>
    {
        public Type Type => typeof(TKey);

        object? IIOSetting.Value { get => Value; set => Value = (TKey)value!; }

        public TKey Value { get; set; }

        public string Key { get; }

        public TKey DefaultValue { get; }

        public object? GetDefaultValue() => DefaultValue;

        public KeyedCollectionSetting(string key, TKey defaultValue)
        {
            Key = key;
            DefaultValue = defaultValue;
            Value = default!;
        }

        public KeyedCollectionSetting(string key, TKey defaultValue, TService service) : this(key, defaultValue)
        {
            this.service = service;
        }

        private void CheckService([CallerMemberName] string? caller = null)
        {
            if (service == null)
            {
                if (DiContainer.SP == null)
                    throw new InvalidOperationException($"You cannot call {caller} when there is no {nameof(service)} provided and the {nameof(DiContainer)} is not build yet");

                service = DiContainer.SP.GetService<TService>();
            }
        }

        private IReadOnlyKeyedCollection<TKey, TItem>? service = null;
        public TItem GetResolvedValue()
        {
            CheckService();
            if (service!.TryGetValue(Value, out var returner)) {
                return returner;
            } 
            else
            {
                return service[DefaultValue];
            }
        }

        object? IResolveableIOSetting.GetResolvedValue() => GetResolvedValue();

        public void SetResolvedValue(TItem value)
        {
            CheckService();

            Value = service!.GetKey(value);
        }

        void IResolveableIOSetting.SetResolvedValue(object? value) => SetResolvedValue((TItem)value!);

        public IEnumerator<TItem> GetEnumerator()
        {
            CheckService();
            return ((IEnumerable<TItem>)service!).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }

    public class KeyedCollectionSetting<TKey, TItem> : KeyedCollectionSetting<TKey, TItem, IReadOnlyKeyedCollection<TKey, TItem>>
    {
        public KeyedCollectionSetting(string key, TKey defaultValue) : base(key, defaultValue)
        {
        }

        public KeyedCollectionSetting(string key, TKey defaultValue, IReadOnlyKeyedCollection<TKey, TItem> service) : base(key, defaultValue, service)
        {
        }
    }
}
