using Microsoft.Extensions.DependencyInjection;
using System.Windows;
using WClipboard.Core.DI;
using WClipboard.Core.WPF.Utilities;

namespace WClipboard.App.Cursors
{
    public class CursorsDictionary : ResourceDictionary
    {
        public CursorsDictionary()
        {
            var cursorManager = DiContainer.SP!.GetRequiredService<ICursorManager>();
            foreach (var cursor in cursorManager) {
                Add(cursor.Key + "Cursor", cursor.Value);
            }
        }
    }
}
