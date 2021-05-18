using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Windows.Input;
using WClipboard.Core.DI;
using WClipboard.Core.Extensions;
using WClipboard.Core.WPF.Clipboard.Implementation.ViewModel;
using WClipboard.Core.WPF.Clipboard.ViewModel;
using WClipboard.Core.WPF.Extensions;
using WClipboard.Core.WPF.Models.Text;
using WClipboard.Core.WPF.Utilities;
using WClipboard.Core.WPF.ViewModels;
using WClipboard.Plugin.ClipboardImplementations.Text.LinkedContent;

#nullable enable

namespace WClipboard.Plugin.ClipboardImplementations.Text
{
    public class LinkedTextClipboardImplementationViewModel : ClipboardImplementationViewModel<TextClipboardImplementation>
    {
        public LinkedTextClipboardImplementationViewModel(TextClipboardImplementation implementation, ClipboardObjectViewModel clipboardObject) : base(implementation, clipboardObject) { }
    }

    public class TextClipboardImplementationViewModel : ClipboardImplementationViewModel<TextClipboardImplementation>
    {
        public BindableObservableCollection<IViewModel> StaticLinkedContent { get; }
        public BindableObservableCollection<IViewModel> DynamicLinkedContent { get; }

        private readonly ICommand linkedContentClickedCommand;

        private IEnumerable<InlineModel> inlines;
        public IEnumerable<InlineModel> Inlines
        {
            get => inlines;
            set => SetProperty(ref inlines, value);
        }

        private readonly Lazy<IViewModelFactoriesManager> viewModelFactoriesManager = DiContainer.SP!.GetLazy<IViewModelFactoriesManager>();

        public TextClipboardImplementationViewModel( TextClipboardImplementation implementation, ClipboardObjectViewModel clipboardObject) : base(implementation, clipboardObject)
        {
            if (!(implementation.Parent is null))
                throw new InvalidOperationException($"Cannot construct a {nameof(TextClipboardImplementationViewModel)} with a linked implementation, use {nameof(LinkedTextClipboardImplementationViewModel)} instead");

            inlines = new List<InlineModel>(1)
            {
                new RunModel() { Text = implementation.Source }
            };

            linkedContentClickedCommand = new SimpleCommand<object>(OnLinkedContentClicked);
            StaticLinkedContent = new BindableObservableCollection<IViewModel>(implementation.LinkedContent!.Where(a => a.Capture.Index == 0 && a.Capture.Length == Model.Source.Length).Select(a => viewModelFactoriesManager.Value.GetViewModel(a.Model, ClipboardObject)).NotNull(), clipboardObject.SynchronizationContext);
            DynamicLinkedContent = new BindableObservableCollection<IViewModel>(clipboardObject.SynchronizationContext);
            implementation.LinkedContent!.CollectionChanged += LinkedContent_CollectionChanged;
            ReupdateInlines();
        }

        private void LinkedContent_CollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
        {
            var dif = e.GetDifferences<ILinkedTextContent>();

            StaticLinkedContent.AddRange(dif.Added.Where(a => a.Capture.Index == 0 && a.Capture.Length == Model.Source.Length).Select(a => viewModelFactoriesManager.Value.GetViewModel(a.Model, ClipboardObject)).NotNull());
            StaticLinkedContent.RemoveAll(lcvm => dif.Removed.Any(r => r.Model == lcvm.Model));
            DynamicLinkedContent.Clear();

            ReupdateInlines();
        }

        private int updateToken = 0;
        private void ReupdateInlines()
        {
            var token = 0;

            lock (this)
            {
                updateToken += 1;
                token = updateToken;
            }

            var linkedContent = Model.LinkedContent!.OrderBy(ltc => ltc.Capture.Index);
            var newInlines = new List<InlineModel>();

            var startIndex = 0;

            foreach (var lc in linkedContent)
            {
                if ((lc.Capture.Index == 0 && lc.Capture.Length == Model.Source.Length) || startIndex > lc.Capture.Index)
                    continue;

                if (startIndex < lc.Capture.Index)
                {
                    newInlines.Add(new RunModel() { Text = Model.Source[startIndex..lc.Capture.Index] });
                }

                var hl = new HyperlinkModel()
                {
                    Command = linkedContentClickedCommand,
                    CommandParameter = lc.Model,
                    ToolTip = $"Detected a {lc.Kind}"
                };
                hl.Inlines.Add(new RunModel() { Text = Model.Source.Substring(lc.Capture.Index, lc.Capture.Length) });
                startIndex = lc.Capture.Index + lc.Capture.Length;
                newInlines.Add(hl);
            }

            if (startIndex < Model.Source.Length)
            {
                newInlines.Add(new RunModel() { Text = Model.Source[startIndex..] });
            }

            lock(this)
            {
                if (token == updateToken)
                {
                    Inlines = newInlines;
                }
            }
        }

        private void OnLinkedContentClicked(object model)
        {
            DynamicLinkedContent.Clear();
            var viewModel = viewModelFactoriesManager.Value.GetViewModel(model, ClipboardObject);
            if (viewModel != null)
            {
                DynamicLinkedContent.Add(viewModel);
            }
        }
    }
}
