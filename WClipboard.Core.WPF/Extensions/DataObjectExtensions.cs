using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics.CodeAnalysis;
using System.Windows;
using WClipboard.Windows.Helpers;
using SysClipboard = System.Windows.Clipboard;

namespace WClipboard.Core.WPF.Extensions
{
    public static class DataObjectExtensions
    {
        public const string ShellFileIDListFormat = "Shell IDList Array";
        public const string WClipboardIdFormat = "WClipboardId";
        public const string LinkedWClipboardIdFormat = "LinkedWClipboardId";
        public const string PrefferedDropEffectFormat = "Preferred DropEffect";

        public static void SetShellFileIDList(this IDataObject dataObject, IReadOnlyCollection<string> files)
        {
            dataObject.SetData(ShellFileIDListFormat, ShellIDListHelper.Create(files), true);
        }

        public static void SetShellFileIDList(this IDataObject dataObject, params string[] files)
        {
            dataObject.SetData(ShellFileIDListFormat, ShellIDListHelper.Create(files), true);
        }

        public static void SetWClipboardId(this IDataObject dataObject, Guid guid)
        {
            dataObject.SetData(WClipboardIdFormat, guid);
        }

        public static bool TryGetWClipboardId(this IDataObject dataObject, out Guid guid)
        {
            var guidObj = dataObject.GetData(WClipboardIdFormat);
            guid = guidObj is Guid guidCasted ? guidCasted : Guid.Empty;
            return guid != Guid.Empty;
        }

        public static bool TryGetWClipboardId(out Guid guid)
        {
            var guidObj = SysClipboard.GetData(WClipboardIdFormat);
            guid = guidObj is Guid guidCasted ? guidCasted : Guid.Empty;
            return guid != Guid.Empty;
        }

        public static void SetLinkedWClipboardId(this IDataObject dataObject, Guid guid)
        {
            dataObject.SetData(LinkedWClipboardIdFormat, guid);
        }

        public static bool TryGetLinkedWClipboardId(this IDataObject dataObject, out Guid guid)
        {
            var guidObj = dataObject.GetData(LinkedWClipboardIdFormat);
            guid = guidObj is Guid guidCasted ? guidCasted : Guid.Empty;
            return guid != Guid.Empty;
        }

        public static void SetPrefferedDropEffect(this IDataObject dataObject, DragDropEffects dropEffects)
        {
            dataObject.SetData(PrefferedDropEffectFormat, dropEffects);
        }

        public static DragDropEffects GetPrefferedDropEffect(this IDataObject dataObject)
        {
            return (DragDropEffects)dataObject.GetData(PrefferedDropEffectFormat);
        }

        public static bool TryGetLinkedWClipboardId(out Guid guid)
        {
            var guidObj = SysClipboard.GetData(LinkedWClipboardIdFormat);
            guid = guidObj is Guid guidCasted ? guidCasted : Guid.Empty;
            return guid != Guid.Empty;
        }

        public static void SetFileDropList(this DataObject dataObject, IEnumerable<string> fileDropList)
        {
            var files = new StringCollection();
            foreach (string file in fileDropList)
            {
                files.Add(file);
            }
            dataObject.SetFileDropList(files);
        }

        public static void SetFileDropList(this DataObject dataObject, params string[] fileDropList)
        {
            var files = new StringCollection();
            files.AddRange(fileDropList);
            dataObject.SetFileDropList(files);
        }

        public static bool TryGetData(this IDataObject dataObject, string format, [MaybeNullWhen(false)] out object? data)
        {
            return (data = dataObject.GetData(format)) != null;
        }
    }
}
