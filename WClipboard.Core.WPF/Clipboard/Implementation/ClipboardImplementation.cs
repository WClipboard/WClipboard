using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using WClipboard.Core.Clipboard.Format;
using WClipboard.Core.Utilities;
using WClipboard.Core.WPF.Utilities;

namespace WClipboard.Core.WPF.Clipboard.Implementation
{
    public abstract class ClipboardImplementation : BindableBase
    {
        public ClipboardFormat Format { get; }
        public ClipboardImplementationFactory Factory { get; }

        public ClipboardImplementation? Parent { get; }

        private readonly ClipboardObject? _clipboardObject;
        public ClipboardObject ClipboardObject
        {
            get => _clipboardObject ?? Parent?.ClipboardObject ?? throw new ApplicationException("Should never happen");
        }

        public ObservableCollection<ClipboardImplementation>? LinkedImplementations { get; }
        public ObservableCollection<object>? LinkedContent { get; }

        protected ClipboardImplementation(ClipboardFormat format, ClipboardImplementationFactory factory, ClipboardImplementation parent)
        {
            Format = format;
            Factory = factory;
            Parent = parent;
        }

        protected ClipboardImplementation(ClipboardFormat format, ClipboardImplementationFactory factory, ClipboardObject clipboardObject)
        {
            Format = format;
            Factory = factory;
            _clipboardObject = clipboardObject;

            LinkedImplementations = new ObservableCollection<ClipboardImplementation>();
            LinkedContent = new ObservableCollection<object>();

            LinkedImplementations.CollectionChanged += CheckParentPropertyOfLinkedImplementations;
            LinkedContent.CollectionChanged += CheckTypeOfLinkedContents;
        }

        private void CheckTypeOfLinkedContents(object sender, NotifyCollectionChangedEventArgs e)
        {
            if(e.NewItems.Cast<object>().Any(o => o is ClipboardImplementation))
            {
                throw new InvalidOperationException($"You cannot add a {nameof(ClipboardImplementation)} in {nameof(LinkedContent)} add it in {nameof(LinkedImplementations)} instead");
            }
        }

        private void CheckParentPropertyOfLinkedImplementations(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.NewItems.Cast<ClipboardImplementation>().Any(c => c.Parent != this))
            {
                throw new InvalidOperationException($"You cannot add a {nameof(ClipboardImplementation)} in {nameof(LinkedImplementations)} where the {Parent} of that {nameof(ClipboardImplementation)} is not the same as the {nameof(ClipboardImplementation)} you added it to");
            }
        }

        public abstract bool IsEqual(EqualtableFormat equaltable);
    }
}
