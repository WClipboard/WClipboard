using System;
using System.Windows;
using System.Windows.Interop;
using WClipboard.Core.DI;
using Microsoft.Extensions.DependencyInjection;
using WClipboard.Core.WPF.Clipboard;
using WClipboard.Core.Clipboard.Trigger;
using WClipboard.Core.Clipboard.Trigger.Defaults;

namespace WClipboard.Core.WPF.Native.Helpers
{
    public class WindowClipboardViewerHelper
    {
        public static void AttachTo(Window window) => new WindowClipboardViewerHelper(window);

        private IntPtr nextViewer = IntPtr.Zero;
        private readonly IClipboardObjectsManager manager;

        private WindowClipboardViewerHelper(Window window)
        {
            manager = DiContainer.SP.GetService<IClipboardObjectsManager>();
            window.SourceInitialized += Window_SourceInitialized;
        }

        private void Window_SourceInitialized(object? sender, EventArgs e)
        {
            if (sender is Window window)
            {
                var handle = (new WindowInteropHelper(window)).Handle;
                nextViewer = NativeMethods.SetClipboardViewer(handle);
                var source = HwndSource.FromHwnd(handle);
                source.AddHook(WindowProc);
                source.Disposed += Source_Disposed;
            }
        }

        private void Source_Disposed(object? sender, EventArgs e)
        {
            if (sender is HwndSource source)
            {
                NativeMethods.ChangeClipboardChain(source.Handle, nextViewer);
            }
        }

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
                    manager.ProcessClipboardTrigger(new ClipboardTrigger(DefaultClipboardTriggerTypes.OS, WindowInfoHelper.GetForegroundWindowInfo()));
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
    }
}
