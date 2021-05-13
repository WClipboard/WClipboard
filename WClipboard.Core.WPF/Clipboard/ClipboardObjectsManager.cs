using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Windows;
using WClipboard.Core.Clipboard.Format;
using WClipboard.Core.Clipboard.Trigger;
using WClipboard.Core.DI;
using WClipboard.Core.Utilities.Collections;
using WClipboard.Core.WPF.Clipboard.Implementation;
using WClipboard.Core.WPF.Clipboard.Trigger;
using WClipboard.Core.WPF.Extensions;
using WClipboard.Core.WPF.Models;

namespace WClipboard.Core.WPF.Clipboard
{
    public interface IClipboardObjectsManager : IDisposable
    {
        Task<ResolvedClipboardTrigger> ProcessClipboardTrigger(ClipboardTrigger trigger);
        ResolvedClipboardTrigger ProcessExternalTrigger(ClipboardTrigger trigger, IDataObject externalDataObject);
        ResolvedClipboardTrigger ProcessInternalTrigger(ClipboardTrigger trigger, DataObject internalDataObject);
        ResolvedClipboardTrigger CreateResolvedCustomClipboardTrigger(ClipboardTrigger trigger, ClipboardObject? linked = null, Guid? id = null);
        void ProcessResolvedClipboardTrigger(ResolvedClipboardTrigger resolvedClipboardTrigger);
        void AddListener(IClipboardObjectsListener listener);
        void RemoveListener(IClipboardObjectsListener listener);
        void TryRemove(ClipboardObject clipboardObject, ClipboardObjectRemoveType reason);
        int GetAutoRemoveImpact(Predicate<ClipboardObject> condition);
        void AutoRemove(Predicate<ClipboardObject> condition);
    }

    public sealed class ClipboardObjectsManager : IClipboardObjectsManager
    {
        private readonly IClipboardFormatsManager _formatsManager;
        private readonly IClipboardObjectManager _objectManager;

        private readonly ClipboardClonerThread _clipboardCloner;

        private readonly KeyedCollectionFunc<Guid, ClipboardObject> _allCollection;
        private readonly ConcurrentDictionary<ClipboardObject, List<ClipboardObject>> _linkedCollection;

        private readonly List<IClipboardObjectsListener> _listeners;

        private ResolvedClipboardTrigger? _lastResolvedTrigger;

        public ClipboardObjectsManager(IClipboardFormatsManager formatsManager, IClipboardObjectManager objectManager, IServiceProvider serviceProvider)
        {
            _formatsManager = formatsManager;
            _objectManager = objectManager;

            _allCollection = new KeyedCollectionFunc<Guid, ClipboardObject>(co => co.Id, new ConcurrentDictionary<Guid, ClipboardObject>());
            _linkedCollection = new ConcurrentDictionary<ClipboardObject, List<ClipboardObject>>();

            _listeners = new List<IClipboardObjectsListener>();

            _clipboardCloner = serviceProvider.Create<ClipboardClonerThread>(this);
        }

        public Task<ResolvedClipboardTrigger> ProcessClipboardTrigger(ClipboardTrigger trigger)
        {
            return _clipboardCloner.ProcessClipboardTrigger(trigger);
        }

        public ResolvedClipboardTrigger ProcessExternalTrigger(ClipboardTrigger trigger, IDataObject externalDataObject)
        {
            if (_lastResolvedTrigger?.Trigger?.TryMerge(trigger) ?? false)
            {
                InformListeners(l => l.OnResolvedTriggerUpdated(_lastResolvedTrigger));
                return _lastResolvedTrigger;
            }
            else
            {
                var resolvedTrigger = ResolveExternalTrigger(trigger, externalDataObject);
                ProcessResolvedClipboardTrigger(resolvedTrigger);
                return resolvedTrigger;
            }
        }

        public ResolvedClipboardTrigger ProcessInternalTrigger(ClipboardTrigger trigger, DataObject internalDataObject)
        {
            //TODO requires (internal/)custom trigger??

            if (_lastResolvedTrigger?.Trigger?.TryMerge(trigger) ?? false)
            {
                InformListeners(l => l.OnResolvedTriggerUpdated(_lastResolvedTrigger));
                return _lastResolvedTrigger;
            }
            else
            {
                var resolvedTrigger = ResolveInternalTrigger(trigger, internalDataObject);
                ProcessResolvedClipboardTrigger(resolvedTrigger);
                return resolvedTrigger;
            }
        }

        public ResolvedClipboardTrigger CreateResolvedCustomClipboardTrigger(ClipboardTrigger trigger, ClipboardObject? linked = null, Guid? id = null)
        {
            if (trigger.Type.Source != ClipboardTriggerSourceType.Custom)
                throw new ArgumentException($"It is not allowed to use {nameof(CreateResolvedCustomClipboardTrigger)} with a {nameof(ClipboardTrigger)} with {nameof(ClipboardTriggerType.Source)}{nameof(ClipboardTrigger.Type)} that is not {nameof(ClipboardTriggerSourceType.Custom)}", nameof(trigger));
            if (id.HasValue && _allCollection.ContainsKey(id.Value))
                throw new ArgumentException($"There is already an {nameof(ClipboardObject)} with this id", nameof(id));


            return CreateResolvedCustomClipboardTriggerInternal(trigger, id ?? NewWClipboardId(), linked);
        }

        private ResolvedClipboardTrigger CreateResolvedCustomClipboardTriggerInternal(ClipboardTrigger trigger, Guid id, ClipboardObject? linked = null)
        {
            var clipboardObject = new ClipboardObject(id, trigger, linked);
            return new ResolvedClipboardTrigger(trigger, clipboardObject, ResolvedClipboardTriggerType.Created);
        }

        public void ProcessResolvedClipboardTrigger(ResolvedClipboardTrigger result)
        {

            if (result.ResolvedType == ResolvedClipboardTriggerType.Invalid || result.Object == null)
            {
                return;
            }
            else if (result.ResolvedType == ResolvedClipboardTriggerType.Created)
            {
                if (!_listeners.Any(l => l.IsInterestedIn(result.Object)))
                {
                    return;
                }

                _allCollection.Add(result.Object);

                if (result.Object.Linked != null)
                {
                    _linkedCollection.GetOrAdd(result.Object.Linked).Add(result.Object);
                }
            }

            _lastResolvedTrigger = result;

            InformListeners(l => l.OnResolvedTrigger(result));
        }

        private ResolvedClipboardTrigger ResolveExternalTrigger(ClipboardTrigger trigger, IDataObject externalDataObject)
        {
            // Check if clipboard object already exists in list
            if (externalDataObject.TryGetWClipboardId(out var guid) && _allCollection.TryGetValue(guid, out var clipboardObject))
            {
                return new ResolvedClipboardTrigger(trigger, clipboardObject, ResolvedClipboardTriggerType.WClipboardId);
            }

            // Clone object to prevent modification and allow fast lookup
            var clonedDataObject = CloneDataObject(externalDataObject);

            //TODO retry exceptions?? Notify user??

            // Check if it contains data
            if (clonedDataObject.DataObject.GetFormats().Length == 0)
            {
                return new ResolvedClipboardTrigger(trigger, null, ResolvedClipboardTriggerType.Invalid);
            }

            trigger.AdditionalInfo.Add(new OriginalFormatsInfo(clonedDataObject.OriginalFormats));

            return CheckForEqualReference(trigger, clonedDataObject.DataObject);
        }

        private ResolvedClipboardTrigger ResolveInternalTrigger(ClipboardTrigger trigger, DataObject internalDataObject)
        {
            // Check if clipboard object already exists in list
            if (internalDataObject.TryGetWClipboardId(out var guid) && _allCollection.TryGetValue(guid, out var clipboardObject))
            {
                return new ResolvedClipboardTrigger(trigger, clipboardObject, ResolvedClipboardTriggerType.WClipboardId);
            }

            // Check if it contains data
            if (!_formatsManager.FilterUnknownFormats(internalDataObject.GetFormats(true)).Any())
            {
                return new ResolvedClipboardTrigger(trigger, null, ResolvedClipboardTriggerType.Invalid);
            }

            return CheckForEqualReference(trigger, internalDataObject);
        }

        private ResolvedClipboardTrigger CheckForEqualReference(ClipboardTrigger trigger, DataObject dataObject)
        {
            var equatableFormats = _objectManager.GetEqualtableFormats(dataObject);

            var registrations = new List<ExistsEqualtableFormatContainer>(equatableFormats.Count);
            registrations.AddRange(equatableFormats.Select(ef => new ExistsEqualtableFormatContainer(ef)));

            foreach(var clipboardObject in _allCollection)
            {
                var result = CheckForEqualReference(trigger, registrations, clipboardObject);
                if (result != null)
                    return result;
            }

            return CreateResolvedTrigger(trigger, dataObject, equatableFormats);
        }

        private ResolvedClipboardTrigger? CheckForEqualReference(ClipboardTrigger trigger, List<ExistsEqualtableFormatContainer> registrations, ClipboardObject clipboardObject)
        {
            if (registrations.Count != clipboardObject.Implementations.Count) //Its only a match if the formats are equal hence the formats count should also be equal
                return null;

            ResetRegistrations(registrations);

            foreach (var registration in registrations)
            {
                foreach (var implemention in clipboardObject.Implementations)
                {
                    if (registration.EqualtableFormat.Format == implemention.Format && registration.EqualtableFormat.Factory == implemention.Factory && implemention.GetType() == registration.EqualtableFormat.ImplementationType)
                    {
                        registration.Match = implemention;
                        break; //This one found, find next
                    }
                }

                if (registration.Match == null) //We did not found a match so there not equal
                    return null;
            }

            foreach (var registration in registrations)
            {
                if (!registration.Match!.IsEqual(registration.EqualtableFormat)) //! because if it was null then it will be filtered out in the lines above
                {
                    return null;
                }
            }

            clipboardObject.Triggers.Add(trigger);

            return new ResolvedClipboardTrigger(trigger, clipboardObject, ResolvedClipboardTriggerType.EqualsReference);
        }

        private class ExistsEqualtableFormatContainer
        {
            public EqualtableFormat EqualtableFormat { get; }
            public ClipboardImplementation? Match { get; set; }

            public ExistsEqualtableFormatContainer(EqualtableFormat equaltableFormat)
            {
                EqualtableFormat = equaltableFormat;
                Match = null;
            }
        }

        private void ResetRegistrations(List<ExistsEqualtableFormatContainer> list)
        {
            foreach(var item in list)
            {
                item.Match = null;
            }
        }

        private ResolvedClipboardTrigger CreateResolvedTrigger(ClipboardTrigger trigger, DataObject dataObject, IEnumerable<EqualtableFormat> equaltableFormats)
        {
            // Check if linked
            ClipboardObject? linked = null;
            if (dataObject.TryGetLinkedWClipboardId(out var linkedGuid))
            {
                _allCollection.TryGetValue(linkedGuid, out linked);
            }

            //Create
            var returner = CreateResolvedCustomClipboardTriggerInternal(trigger, NewWClipboardId(), linked: linked);
            _objectManager.AddImplementationsAsync(returner.Object!, equaltableFormats); //returner.Object should never be null from previous method
            return returner;
        }

        private Guid NewWClipboardId()
        {
            Guid id;

            do
            {
                id = Guid.NewGuid();
            }
            while (_allCollection.ContainsKey(id));

            return id;
        }

        private (DataObject DataObject, string[] OriginalFormats, IReadOnlyDictionary<string, Exception> Exceptions) CloneDataObject(IDataObject oldDataObject)
        {
            var newDataObject = new DataObject();

            var oldFormats = oldDataObject.GetFormats(true);
            var copyFormats = _formatsManager.FilterUnknownFormats(oldFormats);

            var exceptions = new Dictionary<string, Exception>();

            foreach (string format in copyFormats)
            {
                try
                {
                    newDataObject.SetData(format, oldDataObject.GetData(format, true));
                }
                catch (COMException ex)
                {
                    exceptions.Add(format, ex);
                }
            }

            if (newDataObject.ContainsImage())
            {
                var temp = newDataObject.GetImage();
                temp.Freeze();
                newDataObject.SetImage(temp);
            }

            return (newDataObject, oldFormats, exceptions);
        }

        public void AddListener(IClipboardObjectsListener listener)
        {
            _listeners.Add(listener);
        }

        public void RemoveListener(IClipboardObjectsListener listener)
        {
            _listeners.Remove(listener);
        }

        private void InformListeners(Action<IClipboardObjectsListener> informListeners)
        {
            Task.Run(() =>
            {
                for (int i = 0; i < _listeners.Count; i++)
                {
                    informListeners(_listeners[i]);
                }
            });
        }

        public void TryRemove(ClipboardObject clipboardObject, ClipboardObjectRemoveType type)
        {
            if(_linkedCollection.ContainsKey(clipboardObject)) {
                return;
            }
            else if(!_listeners.All(l => l.CanRemove(clipboardObject, type)))
            {
                return;
            }

            if(clipboardObject.Linked != null)
            {
                if(_linkedCollection.TryGetValue(clipboardObject.Linked, out var parentsChilderen)) {

                    parentsChilderen.Remove(clipboardObject);

                    if(parentsChilderen.Count == 0)
                    {
                        _linkedCollection.TryRemove(clipboardObject.Linked, out var _);
                        TryRemove(clipboardObject.Linked, type);
                    }
                }
            }

            _allCollection.Remove(clipboardObject);

            if (_lastResolvedTrigger?.Object == clipboardObject)
            {
                _lastResolvedTrigger = null;
            }

            InformListeners(l => l.OnClipboardObjectRemoved(clipboardObject));

            //TODO
            GC.Collect();
        }

        public int GetAutoRemoveImpact(Predicate<ClipboardObject> condition)
        {
            return FetchAutoRemove(condition).Count;
        }

        public void AutoRemove(Predicate<ClipboardObject> condition)
        {
            var removeQueue = FetchAutoRemove(condition);

            while (removeQueue.Count > 0)
            {
                var removeItem = removeQueue.Dequeue();
                TryRemove(removeItem, ClipboardObjectRemoveType.Auto);
            }
        }

        private Queue<ClipboardObject> FetchAutoRemove(Predicate<ClipboardObject> condition)
        {
            var removeQueue = new Queue<ClipboardObject>();

            foreach (var rootItem in _allCollection)
            {
                if (rootItem.Linked == null) // is rootItem
                {
                    FetchAutoRemoveRecursive(condition, rootItem, removeQueue);
                }
            }

            return removeQueue;
        }

        private bool FetchAutoRemoveRecursive(Predicate<ClipboardObject> condition, ClipboardObject current, Queue<ClipboardObject> removeQueue)
        {
            if(current.IsAutoRemovalAllowed() && condition(current))
            {
                var removeAllowed = true;
                if (_linkedCollection.TryGetValue(current, out var currentChilds))
                {
                    removeAllowed = currentChilds.All(c => FetchAutoRemoveRecursive(condition, c, removeQueue));
                    
                }
                if (removeAllowed)
                    removeQueue.Enqueue(current);
                return removeAllowed;
            }
            return false;
        }

        #region IDisposable Support
        private bool disposedValue = false; // To detect redundant calls

        private void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    _listeners.Clear();
                    _clipboardCloner.Dispose();
                }

                disposedValue = true;
            }
        }

        public void Dispose()
        {
            Dispose(true);
        }
        #endregion
    }
}
