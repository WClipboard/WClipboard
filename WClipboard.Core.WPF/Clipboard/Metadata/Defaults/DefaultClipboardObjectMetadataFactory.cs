using System.Collections.Generic;
using WClipboard.Core.WPF.Clipboard.ViewModel;
using WClipboard.Core.WPF.Models;

namespace WClipboard.Core.WPF.Clipboard.Metadata.Defaults
{
    public class DefaultClipboardObjectMetadataFactory : ClipboardObjectMetadataFactory
    {
        public override IEnumerable<ClipboardObjectMetadata> Create(ClipboardObjectViewModel clipboardObject)
        {
            yield return new TriggersMetadata(clipboardObject);

            if(clipboardObject.Model.MainTrigger.AdditionalInfo.TryGetValue< OriginalFormatsInfo>(out var originalFormatsInfo))
            {
                yield return new FormatsMetadata(originalFormatsInfo, clipboardObject);
            }
        }
    }
}