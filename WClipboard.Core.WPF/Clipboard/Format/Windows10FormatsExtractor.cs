using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using WClipboard.Core.Clipboard.Trigger;
using WClipboard.Core.WPF.Extensions;

namespace WClipboard.Core.WPF.Clipboard.Format
{
    public class Windows10FormatsExtractor : IFormatsExtractor
    {
        public const string HistoryFormat = "CanIncludeInClipboardHistory";
        public const string CloudFormat = "CanUploadToCloudClipboard";

        public IEnumerable<EqualtableFormat> Extract(ClipboardTrigger trigger, IDataObject dataObject)
        {
            if(dataObject.TryGetData(HistoryFormat, out var historyObj) && historyObj is MemoryStream historyMemoryStream && historyMemoryStream.Length == 4)
            {
                using(var br = new BinaryReader(historyMemoryStream))
                {
                    trigger.AdditionalInfo.Add(new Windows10HistoryInfo(br.ReadInt32() != 0));
                }
            }
            if (dataObject.TryGetData(CloudFormat, out var cloudObj) && cloudObj is MemoryStream cloudMemoryStream && cloudMemoryStream.Length == 4)
            {
                using (var br = new BinaryReader(cloudMemoryStream))
                {
                    trigger.AdditionalInfo.Add(new Windows10CloudInfo(br.ReadInt32() != 0));
                }
            }

            return Enumerable.Empty<EqualtableFormat>();
        }
    }

    public class Windows10HistoryInfo
    {
        public bool Allowed { get; }

        public Windows10HistoryInfo(bool allowed)
        {
            Allowed = allowed;
        }
    }

    public class Windows10CloudInfo
    {
        public bool Allowed { get; }

        public Windows10CloudInfo(bool allowed)
        {
            Allowed = allowed;
        }
    }
}
