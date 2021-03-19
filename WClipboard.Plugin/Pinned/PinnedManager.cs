using System;
using System.IO;
using System.Threading.Tasks;
using WClipboard.Core.Extensions.IO;
using System.Linq;
using WClipboard.Core.Extensions;
using WClipboard.Core.WPF.LifeCycle;
using WClipboard.Core.Clipboard.Trigger;
using WClipboard.Core.Clipboard.Format;
using WClipboard.Core.WPF.Clipboard;
using WClipboard.Core.IO;

namespace WClipboard.Plugin.Pinned
{
    public class PinnedManager : IAfterMainWindowLoadedListener
    {
        private static ClipboardTriggerType pinnedTriggerType;

        private readonly IClipboardFormatsManager formatsManager;
        private readonly IClipboardObjectsManager clipboardObjectsManager;
        private readonly IClipboardObjectManager clipboardObjectManager;

        private readonly string directory;

        public PinnedManager(IClipboardFormatsManager formatsManager, IClipboardObjectsManager clipboardObjectsManager, IClipboardObjectManager clipboardObjectManager, IAppDataManager appDataManager)
        {
            if (pinnedTriggerType == null)
                pinnedTriggerType = new ClipboardTriggerType("Pinned", "PinIcon", ClipboardTriggerSourceType.Custom);

            this.formatsManager = formatsManager;
            this.clipboardObjectsManager = clipboardObjectsManager;
            this.clipboardObjectManager = clipboardObjectManager;

            directory = appDataManager.RoamingPath + "Pins"; 
        }

        private string GetFileName(ClipboardObject clipboardObject)
        {
            if (!Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }

            return @$"{directory}\{clipboardObject.Id}.wco";
        }

        public static bool IsPinned(ClipboardObject clipboardObject)
        {
            return clipboardObject.Properties.OfType<PinnedProperty>().Any();
        }

        public async Task Pin(ClipboardObject clipboardObject)
        {
            if (IsPinned(clipboardObject))
                return;

            var fileName = GetFileName(clipboardObject);
            var encoding = System.Text.Encoding.Unicode;

            using var fs = new BinaryWriter(File.OpenWrite(fileName), encoding);

            fs.Write(1UL); //Serialization version
            fs.Write(clipboardObject.MainTrigger.When.ToBinary());
            fs.Write(clipboardObject.MainTrigger.WindowInfo?.ProcessInfo?.Path ?? "");

            using var ms = new MemoryStream();

            foreach (var implementation in clipboardObject.Implementations)
            {
                ms.SetLength(0);

                fs.Write(implementation.Format.Name);

                await implementation.Factory.Serialize(implementation, ms);

                fs.Write(ms.Length);

                ms.Seek(0, SeekOrigin.Begin);

                ms.CopyTo(fs);
            }

            clipboardObject.Properties.Add(PinnedProperty.Instance);
        }

        public void UnPin(ClipboardObject clipboardObject)
        {
            var fileName = GetFileName(clipboardObject);
            File.Delete(fileName);
            clipboardObject.Properties.RemoveAll(p => p is PinnedProperty);
        }

        private async Task Deserialize(string fileName, Guid id)
        {
            var encoding = System.Text.Encoding.Unicode;

            using var fs = new BinaryReader(File.OpenRead(fileName), encoding);

            var version = fs.ReadUInt64();
            if (version != 1) {
                throw new NotSupportedException($"Pinned version {version} not supported");
            }

            var ticks = fs.ReadInt64();
            var _ = fs.ReadString(); //process path

            var trigger = new ClipboardTrigger(DateTime.FromBinary(ticks), pinnedTriggerType, null, null);
            var resolvedClipboardTrigger = clipboardObjectsManager.CreateResolvedCustomClipboardTrigger(trigger, null, id);

            using var ms = new MemoryStream();

            while (!fs.BaseStream.IsEndOfStream())
            {
                var formatName = fs.ReadString();
                var length = fs.ReadInt64();

                var format = formatsManager[formatName];

                ms.SetLength(length);
                ms.Seek(0, SeekOrigin.Begin);

                fs.CopyTo(ms, length);

                await clipboardObjectManager.AddImplementationsAsync(resolvedClipboardTrigger.Object, ms, format).ConfigureAwait(false);
            }

            resolvedClipboardTrigger.Object.Properties.Add(PinnedProperty.Instance);
            clipboardObjectsManager.ProcessResolvedClipboardTrigger(resolvedClipboardTrigger);
        }

        async void IAfterMainWindowLoadedListener.AfterMainWindowLoaded()
        {
            if (Directory.Exists(directory))
            {
                foreach (var filePath in Directory.EnumerateFiles(directory, "*.wco"))
                {
                    var fileName = Path.GetFileNameWithoutExtension(filePath); ;
                    Guid.TryParse(fileName, out var id);

                    await Deserialize(filePath, id).ConfigureAwait(false);
                }
            }
        }
    }
}
