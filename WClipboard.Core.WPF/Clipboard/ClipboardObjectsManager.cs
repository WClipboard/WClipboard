using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using WClipboard.Core.Clipboard.Trigger;
using WClipboard.Core.DI;
using WClipboard.Core.Utilities.Collections;
using WClipboard.Core.WPF.Clipboard.Filter;
using WClipboard.Core.WPF.Clipboard.Format;
using WClipboard.Core.WPF.Clipboard.Implementation;
using WClipboard.Core.WPF.Clipboard.Trigger;
using WClipboard.Core.WPF.Extensions;

namespace WClipboard.Core.WPF.Clipboard
{
    public interface IClipboardObjectsManager : IDisposable
    {
        Task ProcessClipboardTrigger(ClipboardTrigger trigger);
        ResolvedClipboardTrigger ProcessClipboardTrigger(ClipboardTrigger trigger, IDataObject dataObject);
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
        private readonly IClipboardObjectManager _objectManager;
        private readonly IEnumerable<IFormatsExtractor> formatsExtractors;
        private readonly IEnumerable<IClipboardFilter> clipboardFilters;

        private readonly ClipboardClonerThread clipboardCloner;
        private readonly ClipboardTriggerScheduler clipboardTriggerScheduler;

        private readonly KeyedCollectionFunc<Guid, ClipboardObject> _allCollection;
        private readonly ConcurrentDictionary<ClipboardObject, List<ClipboardObject>> _linkedCollection;

        private readonly List<IClipboardObjectsListener> _listeners;

        public ClipboardObjectsManager(IClipboardObjectManager objectManager, IEnumerable<IFormatsExtractor> formatsExtractors, IEnumerable<IClipboardFilter> clipboardFilters, IServiceProvider serviceProvider)
        {
            _objectManager = objectManager;
            this.formatsExtractors = formatsExtractors;
            this.clipboardFilters = clipboardFilters;

            _allCollection = new KeyedCollectionFunc<Guid, ClipboardObject>(co => co.Id, new ConcurrentDictionary<Guid, ClipboardObject>());
            _linkedCollection = new ConcurrentDictionary<ClipboardObject, List<ClipboardObject>>();

            _listeners = new List<IClipboardObjectsListener>();

            clipboardCloner = serviceProvider.Create<ClipboardClonerThread>(this);
            clipboardTriggerScheduler = serviceProvider.Create<ClipboardTriggerScheduler>(clipboardCloner);
        }

        public Task ProcessClipboardTrigger(ClipboardTrigger trigger)
        {
            return clipboardTriggerScheduler.Schedule(trigger);
        }

        public ResolvedClipboardTrigger ProcessClipboardTrigger(ClipboardTrigger trigger, IDataObject dataObject)
        {
            var resolvedClipboardTrigger = CreateResolvedClipboardTrigger(trigger, dataObject);
            ProcessResolvedClipboardTrigger(resolvedClipboardTrigger);
            return resolvedClipboardTrigger;
        }

        private ResolvedClipboardTrigger CreateResolvedClipboardTrigger(ClipboardTrigger trigger, IDataObject dataObject)
        {
            // Check if clipboard object already exists in list
            if (dataObject.TryGetWClipboardId(out var guid) && _allCollection.TryGetValue(guid, out var refClipboardObject))
            {
                refClipboardObject.Triggers.Add(trigger);
                return new ResolvedClipboardTrigger(trigger, refClipboardObject, ResolvedClipboardTriggerType.WClipboardId);
            }

            //Extract Formats
            //TODO retry exceptions
            var equatableFormats = formatsExtractors.SelectMany(fe => fe.Extract(trigger, dataObject)).ToList();
            trigger.AdditionalInfo.Add(new OriginalFormatsInfo(dataObject.GetFormats()));

            //Check if it contains any data
            if (equatableFormats.Count == 0)
            {
                return new ResolvedClipboardTrigger(trigger, null, ResolvedClipboardTriggerType.Invalid);
            }

            //Filter
            if (ShouldFilter(trigger, equatableFormats))
            {
                return new ResolvedClipboardTrigger(trigger, null, ResolvedClipboardTriggerType.Filtered);
            }

            //Check for Equals
            return CheckForEqualsReference(trigger, dataObject, equatableFormats);
        }

        private bool ShouldFilter(ClipboardTrigger trigger, IEnumerable<EqualtableFormat> equaltableFormats)
        {
            return clipboardFilters.Any(cf => cf.ShouldFilter(trigger, equaltableFormats));
        }

        public ResolvedClipboardTrigger CreateResolvedCustomClipboardTrigger(ClipboardTrigger trigger, ClipboardObject? linked = null, Guid? id = null)
        {
            if (!(trigger.Type is CustomClipboardTriggerType))
                throw new ArgumentException($"It is not allowed to use {nameof(CreateResolvedCustomClipboardTrigger)} with a {nameof(ClipboardTrigger)} that has a {nameof(ClipboardTrigger.Type)} that is not a {nameof(CustomClipboardTriggerType)}", nameof(trigger));
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
            if (result.ResolvedType == ResolvedClipboardTriggerType.Invalid || 
                result.ResolvedType == ResolvedClipboardTriggerType.Filtered || 
                result.Object is null)
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

            InformListeners(l => l.OnResolvedTrigger(result));
        }

        private ResolvedClipboardTrigger CheckForEqualsReference(ClipboardTrigger trigger, IDataObject dataObject, List<EqualtableFormat> equatableFormats)
        {
            var registrations = new List<ExistsEqualtableFormatContainer>(equatableFormats.Count);
            registrations.AddRange(equatableFormats.Select(ef => new ExistsEqualtableFormatContainer(ef)));

            foreach(var clipboardObject in _allCollection)
            {
                if (CheckForEqualsReference(registrations, clipboardObject))
                {
                    clipboardObject.Triggers.Add(trigger);
                    return new ResolvedClipboardTrigger(trigger, clipboardObject, ResolvedClipboardTriggerType.EqualsReference);
                }
            }

            return CreateResolvedTrigger(trigger, dataObject, equatableFormats);
        }

        private bool CheckForEqualsReference(List<ExistsEqualtableFormatContainer> registrations, ClipboardObject clipboardObject)
        {
            if (registrations.Count != clipboardObject.Implementations.Count) //Its only a match if the formats are equal hence the formats count should also be equal
                return false;

            ResetRegistrations(registrations);

            foreach (var registration in registrations)
            {
                foreach (var implemention in clipboardObject.Implementations)
                {
                    if (registration.EqualtableFormat.Format == implemention.Format)
                    {
                        registration.Match = implemention;
                        break; //This one found, find next
                    }
                }

                if (registration.Match is null) //We did not found a match so there not equal
                    return false;
            }

            foreach (var registration in registrations)
            {
                if (!registration.Match!.IsEqual(registration.EqualtableFormat)) //! because if it was null then it will be filtered out in the lines above
                {
                    return false;
                }
            }

            return true;
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

        private ResolvedClipboardTrigger CreateResolvedTrigger(ClipboardTrigger trigger, IDataObject dataObject, IEnumerable<EqualtableFormat> equaltableFormats)
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
                    clipboardCloner.Dispose();
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
