using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using WClipboard.Core.Extensions;

namespace WClipboard.Core.Utilities.Collections
{
    public class ConcurrentObservableList<T> : IList<T>, INotifyPropertyChanged, INotifyCollectionChanged, IReadOnlyCollection<T>, IReadOnlyList<T>, ICollection, IList
    {
        protected readonly List<T> list;
        protected readonly ReadWriteLock readWriteLock;

        public ConcurrentObservableList() {
            list = new List<T>();
            readWriteLock = new ReadWriteLock();
        }

        public ConcurrentObservableList(IEnumerable<T> items)
        { 
            list = new List<T>(items);
            readWriteLock = new ReadWriteLock();
        }

        private O ReadOperation<O>(Func<O> operation)
        {
            readWriteLock.EnterRead();
            try
            {
                return operation();
            }
            finally
            {
                readWriteLock.ExitRead();
            }
        }

        private void ReadOperation(Action operation)
        {
            readWriteLock.EnterRead();
            try
            {
                operation();
            }
            finally
            {
                readWriteLock.ExitRead();
            }
        }

        private void ProcessModification(NotifyCollectionChangedEventArgs e)
        {
            readWriteLock.ExchangeWriteForRead();

            try
            {
                OnPropertyChanged(nameof(Count));
                OnPropertyChanged("Item[]");
                OnCollectionChanged(e);
            }
            finally
            {
                readWriteLock.ExitRead();
            }
        }

        #region ICollection
        bool ICollection.IsSynchronized => false;
        object ICollection.SyncRoot { get; } = new object();

        bool ICollection<T>.IsReadOnly => false;
        #endregion ICollection

        #region IList
        public T this[int index] { get => ReadOperation(() => list[index]); set => throw new NotImplementedException(); }

        public int Count => ReadOperation(() => list.Count);

        bool IList.IsFixedSize => false;

        bool IList.IsReadOnly => false;

        object? IList.this[int index] { get => this[index]; set => this[index] = (T)value!; }

        public void Add(T item)
        {
            readWriteLock.EnterWrite();
            try
            {
                list.Add(item);
            }
            catch (Exception)
            {
                readWriteLock.ExitWrite();
                throw;
            }

            ProcessModification(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, item, list.Count - 1));
        }

        public void Clear()
        {
            readWriteLock.EnterWrite();
            try
            {
                list.Clear();
            }
            catch (Exception)
            {
                readWriteLock.ExitWrite();
                throw;
            }

            ProcessModification(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
        }

        public bool Contains(T item) => ReadOperation(() => list.Contains(item));

        public void CopyTo(T[] array, int arrayIndex) => ReadOperation(() => list.CopyTo(array, arrayIndex));

        public IEnumerator<T> GetEnumerator()
        {
            readWriteLock.EnterRead();
            try
            {
                foreach(var item in list)
                {
                    yield return item;
                }
            }
            finally
            {
                readWriteLock.ExitRead();
            }
        }

        public int IndexOf(T item) => ReadOperation(() => list.IndexOf(item));

        public void Insert(int index, T item)
        {
            readWriteLock.EnterWrite();
            try
            {
                list.Insert(index, item);
            }
            catch (Exception)
            {
                readWriteLock.ExitWrite();
                throw;
            }

            ProcessModification(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, item, index));
        }

        public bool Remove(T item)
        {
            readWriteLock.EnterWrite();
            int index;
            try
            {
                index = list.IndexOf(item);
                if (index >= 0)
                {
                    list.RemoveAt(index);
                } 
                else //Item not found exit
                {
                    readWriteLock.ExitWrite();
                    return false;
                }
            }
            catch (Exception)
            {
                readWriteLock.ExitWrite();
                throw;
            }

            ProcessModification(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, item, index));

            return true;
        }

        public void RemoveAt(int index)
        {
            readWriteLock.EnterWrite();
            T item;
            try
            {
                item = list[index];
                list.RemoveAt(index);
            }
            catch (Exception)
            {
                readWriteLock.ExitWrite();
                throw;
            }

            ProcessModification(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, item, index));
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
        #endregion IList

        #region INotifyPropertyChanged
        public event PropertyChangedEventHandler? PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        protected PropertyChangedEventHandler? GetPropertyChangedEventHandler() => PropertyChanged;

        #endregion INotifyPropertyChanged

        #region INotifyCollectionChanged
        public event NotifyCollectionChangedEventHandler? CollectionChanged;

        protected virtual void OnCollectionChanged(NotifyCollectionChangedEventArgs args)
        {
            CollectionChanged?.Invoke(this, args);
        }

        protected NotifyCollectionChangedEventHandler? GetCollectionChangedEventHandler() => CollectionChanged;

        #endregion INotifyCollectionChanged

        #region IList
        int IList.Add(object? value)
        {
            readWriteLock.EnterWrite();
            int count;
            try
            {
                list.Add((T)value!);
                count = list.Count;
            }
            catch (Exception)
            {
                readWriteLock.ExitWrite();
                throw;
            }

            ProcessModification(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, value, count - 1));
            return count;
        }

        bool IList.Contains(object? value) => Contains((T)value!);

        int IList.IndexOf(object? value) => IndexOf((T)value!);

        void IList.Insert(int index, object? value) => Insert(index, (T)value!);

        void IList.Remove(object? value) => Remove((T)value!);
        #endregion IList

        #region ICollection
        void ICollection.CopyTo(Array array, int index) => ReadOperation(() => ((ICollection)list).CopyTo(array, index));
        #endregion

        public void AddRange(IEnumerable<T> items)
        {
            var tempItemsList = items.ToList();

            readWriteLock.EnterWrite();
            int count;
            try
            {
                count = list.Count;
                list.AddRange(tempItemsList);
                if (count == list.Count) //No changes, return without event handling
                {
                    readWriteLock.ExitWrite();
                    return;
                }
            }
            catch (Exception)
            {
                readWriteLock.ExitWrite();
                throw;
            }

            ProcessModification(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, tempItemsList, count));
        }

        public void RemoveAll(Predicate<T> predicate)
        {
            readWriteLock.EnterWrite();
            try
            {
                var count = list.Count;
                list.RemoveAll(predicate);
                if (count == list.Count) //No changes, return without event handling
                {
                    readWriteLock.ExitWrite();
                    return;
                }
            }
            catch (Exception)
            {
                readWriteLock.ExitWrite();
                throw;
            }

            ProcessModification(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
        }

        public bool Move(Predicate<T> itemFinder, int newIndex)
        {
            readWriteLock.EnterWrite();
            int oldIndex;
            T item;
            try
            {
                oldIndex = list.FirstIndexOf(itemFinder);
                if (oldIndex == -1)
                {
                    readWriteLock.ExitWrite();
                    return false;
                }
                item = list[oldIndex];
                list.RemoveAt(oldIndex);
                list.Insert(newIndex, item);
            }
            catch (Exception)
            {
                readWriteLock.ExitWrite();
                throw;
            }

            readWriteLock.ExchangeWriteForRead();

            try
            {
                OnPropertyChanged("Item[]");
                OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Move, item, newIndex, oldIndex));
            }
            finally
            {
                readWriteLock.ExitRead();
            }

            return true;
        }

        public bool Remove(Predicate<T> itemFinder)
        {
            readWriteLock.EnterWrite();
            int index;
            T item;
            try
            {
                index = list.FirstIndexOf(itemFinder);
                if (index == -1)
                {
                    readWriteLock.ExitWrite();
                    return false;
                }

                item = list[index];
                list.RemoveAt(index);
            }
            catch (Exception)
            {
                readWriteLock.ExitWrite();
                throw;
            }

            ProcessModification(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, item, index));
            return true;
        }

        public void ReplaceAll(IEnumerable<T> newItems)
        {
            readWriteLock.EnterWrite();
            try
            {
                list.Clear();
                list.AddRange(newItems);
            }
            catch (Exception)
            {
                readWriteLock.ExitWrite();
                throw;
            }

            ProcessModification(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
        }
    }
}
