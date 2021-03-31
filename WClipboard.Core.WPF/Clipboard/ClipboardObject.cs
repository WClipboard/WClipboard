using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using WClipboard.Core.Clipboard.Trigger;
using WClipboard.Core.WPF.Clipboard.Implementation;
using WClipboard.Core.WPF.Extensions;

namespace WClipboard.Core.WPF.Clipboard
{
    public sealed class ClipboardObject
    {
        public Guid Id { get; }

        public ClipboardTrigger MainTrigger { get; }
        public ClipboardObject? Linked { get; }

        public ObservableCollection<ClipboardImplementation> Implementations { get; }
        public ObservableCollection<ClipboardObjectProperty> Properties { get; }
        public ObservableCollection<ClipboardTrigger> Triggers { get; }

        public ClipboardObject(Guid id, ClipboardTrigger mainTrigger, ClipboardObject? linked)
        {
            Id = id;
            MainTrigger = mainTrigger;
            Linked = linked;
            Implementations = new ObservableCollection<ClipboardImplementation>();
            Implementations.CollectionChanged += CheckIfImplementationShouldBeInThis;
            Properties = new ObservableCollection<ClipboardObjectProperty>();
            Triggers = new ObservableCollection<ClipboardTrigger>
            {
                MainTrigger
            };
        }

        private void CheckIfImplementationShouldBeInThis(object? sender, NotifyCollectionChangedEventArgs e)
        {
            if(e.NewItems?.Cast<ClipboardImplementation>().Any(ci => ci.ClipboardObject != this) ?? false)
            {
                throw new InvalidOperationException($"You cannot add an {nameof(ClipboardImplementation)} where its {nameof(ClipboardImplementation.ClipboardObject)} is not set to this {nameof(ClipboardObject)}");
            }
        }

        public bool IsAutoRemovalAllowed()
        {
            return !Properties.Any(p => p.PreventAutoRemoval);
        }

        public override int GetHashCode() => Id.GetHashCode();
    }

    public static class ClipboardObjectExtensions
    {
        public static async Task<DataObject> GetDataObject(this ClipboardObject clipboardObject)
        {
            var dataObject = new DataObject();
            DataObjectExtensions.SetWClipboardId(dataObject, clipboardObject.Id);
            if (clipboardObject.Linked != null)
            {
                DataObjectExtensions.SetLinkedWClipboardId(dataObject, clipboardObject.Linked.Id);
            }

            foreach (var implementation in clipboardObject.Implementations)
            {
                await implementation.Factory.WriteToDataObject(implementation, dataObject);
            }

            return dataObject;
        }
    }
}
