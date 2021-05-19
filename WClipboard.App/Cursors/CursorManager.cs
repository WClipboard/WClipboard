using System.Collections.Generic;
using System.IO;
using System.Windows.Input;
using WClipboard.Core.WPF.Utilities;

namespace WClipboard.App.Cursors
{
    public class CursorManager : Dictionary<string, Cursor>, ICursorManager
    {
        public CursorManager()
        {
            Add("Drag", new Cursor(new MemoryStream(Properties.Resources.drag)));
            Add("Dragable", new Cursor(new MemoryStream(Properties.Resources.dragable)));
            Add("HandFull", new Cursor(new MemoryStream(Properties.Resources.hand_full)));
        }
    }
}
