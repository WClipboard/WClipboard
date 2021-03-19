using WClipboard.Core.WPF.CustomControls;
using WClipboard.Core.WPF.Native.Helpers;
using WClipboard.App.ViewModels;
using System.Windows;

namespace WClipboard.App.Windows
{
    /// <summary>
    /// Interaction logic for OverviewWindow.xaml
    /// </summary>
    public partial class OverviewWindow : CustomWindow
    {
        public OverviewWindow()
        {
            InitializeComponent();

            WindowClipboardViewerHelper.AttachTo(this);

            DataContext = new OverviewWindowViewModel(this);
        }
    }
}
