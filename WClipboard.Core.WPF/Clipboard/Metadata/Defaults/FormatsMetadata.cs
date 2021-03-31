using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using WClipboard.Core.Extensions;
using WClipboard.Core.Utilities;
using WClipboard.Core.WPF.Clipboard.Implementation.ViewModel;
using WClipboard.Core.WPF.Clipboard.ViewModel;
using WClipboard.Core.WPF.Converters;
using WClipboard.Core.WPF.Extensions;
using WClipboard.Core.WPF.Models;
using WClipboard.Core.WPF.Models.Text;
using WClipboard.Core.WPF.Themes;

namespace WClipboard.Core.WPF.Clipboard.Metadata.Defaults
{
    public class FormatsMetadata : ClipboardObjectMetadata
    {
        private readonly ClipboardObjectViewModel clipboardObject;

        public IReadOnlyList<FormatMetadata> Formats { get; }

        public FormatsMetadata(OriginalFormatsInfo originalFormatsInfo, ClipboardObjectViewModel clipboardObject) : base("TypesIcon", "Formats")
        {
            this.clipboardObject = clipboardObject;
            clipboardObject.Implementations.CollectionChanged += Implementations_CollectionChanged;

            Formats = originalFormatsInfo.Value.Select(f => new FormatMetadata(f)).ToList(originalFormatsInfo.Value.Count);
            RefreshFormats();
        }

        private void RefreshFormats()
        {
            var foundFormats = clipboardObject.Implementations.Select(i => i.Model.Format.Format).ToHashSet();
            foreach(var format in Formats)
            {
                format.IsPressent = foundFormats.Contains(format.Format);
            }
            OnPropertyChanged(nameof(Formats));
        }

        private void Implementations_CollectionChanged(object? sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            var dif = e.GetDifferences<ClipboardImplementationViewModel>();

            if(dif.Removed.Count > 0)
            {
                RefreshFormats();
            }
            else
            {
                foreach (var added in dif.Added)
                {
                    var format = Formats.FirstOrDefault(f => f.Format == added.Model.Format.Format);
                    if (!(format is null))
                        format.IsPressent = true;
                }
                OnPropertyChanged(nameof(Formats));
            }
        }
    }

    public class FormatMetadata : BindableBase
    {
        public string Format { get; }

        private bool isPressent = false;
        public bool IsPressent
        {
            get => isPressent;
            set => SetProperty(ref isPressent, value);
        }

        public FormatMetadata(string format)
        {
            Format = format;
        }
    }

    public class FormatsToInlineModelsConverter : BaseConverter<IReadOnlyCollection<FormatMetadata>, IEnumerable<InlineModel>>
    {
        public override IEnumerable<InlineModel> Convert(IReadOnlyCollection<FormatMetadata> value, Type targetType, object? parameter, CultureInfo culture)
        {
            int i = 0;
            foreach (var format in value)
            {
                yield return new RunModel() { 
                    Text = ++i == value.Count ? format.Format : $"{format.Format}, ", 
                    Foreground = format.IsPressent ? null : new FromPalette("GrayBI")
                };
            }
        }
    }
}
