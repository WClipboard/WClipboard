using System.ComponentModel;
using System.Diagnostics;
using System.Windows.Input;
using WClipboard.Core.WPF.Models;

namespace WClipboard.Plugin.ClipboardImplementations.Path
{
    public class PathPartOpenCommandBinding : AbstractCommandBinding
    {
        public PathPartOpenCommandBinding() : base(PathPart.OpenCommand) { }

        public override void HandleCanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            if(!e.Handled)
            {
                e.CanExecute = true;
                e.Handled = true;
                e.ContinueRouting = false;
            }
        }
        public override void HandleExecute(object sender, ExecutedRoutedEventArgs e)
        {
            if(!e.Handled && e.Parameter is PathPart pathPart)
            {
                e.Handled = true;

                if (pathPart.Child == null)
                {
                    try
                    {
                        Process.Start(new ProcessStartInfo($"\"{pathPart.FullPath}\"")
                        {
                            UseShellExecute = true
                        });
                    }
                    catch (Win32Exception ex)
                    {
                        //ErrorCode for No application is associated with the specified file for this operation
                        if (ex.ErrorCode == -2147467259)
                        {
                            Process.Start("rundll32.exe", $"shell32.dll, OpenAs_RunDLL {pathPart.FullPath}");
                        }
                    }
                }
                else
                {
                    Process.Start(new ProcessStartInfo("explorer.exe", $"/select, \"{pathPart.Child.FullPath}\""));
                }
            }
        }
    }
}
