using WClipboard.Core.WPF.CustomControls;
using WClipboard.App.ViewModels;
using WClipboard.Core.DI;

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

            DataContext = new OverviewWindowViewModel(this, DiContainer.SP!);
        }
    }
}
