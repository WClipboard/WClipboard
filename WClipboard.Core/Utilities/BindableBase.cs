using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace WClipboard.Core.Utilities
{
    public delegate void PropertyChangedCallback<T>(T oldValue);

    public interface IFluentPropertyChanged<T>
    {
        IFluentPropertyChanged<T> OnChanged(Action action);
        IFluentPropertyChanged<T> OnChanged(PropertyChangedCallback<T> action);
    }

    public abstract class BindableBase : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged(params string[] names)
        {
            if (PropertyChanged != null)
            {
                foreach (var name in names)
                {
                    if(name != null)
                        PropertyChanged(this, new PropertyChangedEventArgs(name));
                }
            }
        }

        protected IFluentPropertyChanged<T> SetProperty<T>(ref T property, T value, [CallerMemberName] string? propertyName = null)
            => SetProperty(ref property, value, new[] { propertyName ?? throw new ArgumentNullException(nameof(propertyName)) });

        protected IFluentPropertyChanged<T> SetProperty<T>(ref T property, T value, params string[] propertyNames)
        {
            if (!object.Equals(property, value))
            {
                var oldValue = property;
                property = value;
                OnPropertyChanged(propertyNames);
                return new FluentPropertyChanged<T>(oldValue);
            }
            else
            {
                return FluentPropertyNotChanged<T>.Resolve;
            }
        }

        protected IFluentPropertyChanged<T> SetProperty<T>(ref T property, T value, bool guard, [CallerMemberName] string? propertyName = null)
            => SetProperty(ref property, value, guard, new[] { propertyName ?? throw new ArgumentNullException(nameof(propertyName)) });

        protected IFluentPropertyChanged<T> SetProperty<T>(ref T property, T value, bool guard, params string[] propertyNames)
        {
            if (guard)
            {
                return SetProperty(ref property, value, propertyNames);
            }
            else
            {
                return FluentPropertyNotChanged<T>.Resolve;
            }
        }

        #region IPropertyChanged

        private class FluentPropertyNotChanged<T> : IFluentPropertyChanged<T>
        {
            public IFluentPropertyChanged<T> OnChanged(Action action) => this;

            public IFluentPropertyChanged<T> OnChanged(PropertyChangedCallback<T> action) => this;

            private FluentPropertyNotChanged() { }

            public static readonly IFluentPropertyChanged<T> Resolve = new FluentPropertyNotChanged<T>();
        }

        private class FluentPropertyChanged<T> : IFluentPropertyChanged<T>
        {
            private readonly T oldValue;

            public IFluentPropertyChanged<T> OnChanged(Action action)
            {
                action();
                return this;
            }

            public IFluentPropertyChanged<T> OnChanged(PropertyChangedCallback<T> action)
            {
                action(oldValue);
                return this;
            }

            public FluentPropertyChanged(T oldValue)
            {
                this.oldValue = oldValue;
            }
        }

        #endregion
    }
}
