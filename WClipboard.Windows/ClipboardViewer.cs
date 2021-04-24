using System;
using System.ComponentModel;
using System.Runtime.InteropServices;
using WClipboard.Windows.Native;

namespace WClipboard.Windows
{
    public interface IClipboardViewer
    {
        event EventHandler? ClipboardChanged;
    }

    public class ClipboardViewer : IClipboardViewer, IDisposable
    {
        private IntPtr nextViewer;
        private bool disposedValue;

        private readonly IHiddenWindowMessages hiddenWindow;

        public event EventHandler? ClipboardChanged;

        public ClipboardViewer(IHiddenWindowMessages hiddenWindow)
        {
            this.hiddenWindow = hiddenWindow;

            nextViewer = NativeMethods.SetClipboardViewer(hiddenWindow.Handle);
            if (nextViewer == IntPtr.Zero && Marshal.GetLastWin32Error() != 0)
                throw new Win32Exception();

            hiddenWindow.AddHook(WindowProc);
            hiddenWindow.Disposed += Source_Disposed;
        }

        private void Source_Disposed(object? sender, EventArgs e) => Dispose();

        private IntPtr WindowProc(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            //  do stuff
            switch (msg)
            {
                //
                // The WM_DRAWCLIPBOARD message is sent to the first window 
                // in the clipboard viewer chain when the content of the 
                // clipboard changes. This enables a clipboard viewer 
                // window to display the new content of the clipboard. 
                //
                case (int)NativeConsts.Message.WM_DRAWCLIPBOARD:
                    //
                    // Each window that receives the WM_DRAWCLIPBOARD message 
                    // must call the SendMessage function to pass the message 
                    // on to the next window in the clipboard viewer chain.
                    //
                    NativeMethods.SendMessage(nextViewer, msg, wParam, lParam);
                    ClipboardChanged?.Invoke(this, new EventArgs());
                    handled = true;
                    break;


                //
                // The WM_CHANGECBCHAIN message is sent to the first window 
                // in the clipboard viewer chain when a window is being 
                // removed from the chain. 
                //
                case (int)NativeConsts.Message.WM_CHANGECBCHAIN:


                    // When a clipboard viewer window receives the WM_CHANGECBCHAIN message, 
                    // it should call the SendMessage function to pass the message to the 
                    // next window in the chain, unless the next window is the window 
                    // being removed. In this case, the clipboard viewer should save 
                    // the handle specified by the lParam parameter as the next window in the chain. 

                    //
                    // wParam is the Handle to the window being removed from 
                    // the clipboard viewer chain 
                    // lParam is the Handle to the next window in the chain 
                    // following the window being removed. 
                    if (wParam == nextViewer)
                    {
                        //
                        // If wParam is the next clipboard viewer then it
                        // is being removed so update pointer to the next
                        // window in the clipboard chain
                        //
                        nextViewer = lParam;
                    }
                    else
                    {
                        NativeMethods.SendMessage(nextViewer, msg, wParam, lParam);
                    }
                    handled = true;
                    break;
            }
            return IntPtr.Zero;
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    
                }

                NativeMethods.ChangeClipboardChain(hiddenWindow.Handle, nextViewer);

                disposedValue = true;
            }
        }

        ~ClipboardViewer()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: false);
        }

        public void Dispose()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}
