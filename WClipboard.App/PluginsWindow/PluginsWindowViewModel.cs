using Microsoft.Win32;
using System.Collections.Generic;
using System.Windows;
using WClipboard.Core.WPF.Utilities;
using WClipboard.Plugin;

namespace WClipboard.App.PluginsWindow
{
    public class PluginsWindowViewModel
    {
        private readonly IPluginManager pluginManager;

        private readonly PluginsWindow pluginsWindow;

        public IEnumerable<IPlugin> Plugins => pluginManager.Plugins;

        public SimpleCommand AddCommand { get; }

        public PluginsWindowViewModel(IPluginManager pluginManager, Window callerWindow)
        {
            this.pluginManager = pluginManager;

            AddCommand = new SimpleCommand(OnAdd, _ => true);

            pluginsWindow = new PluginsWindow()
            {
                DataContext = this,
                Owner = callerWindow
            };
            pluginsWindow.ShowDialog();
        }

        private void OnAdd(object? parameter)
        {
            var openFileDialog = new OpenFileDialog()
            {
                CheckFileExists = true,
                CheckPathExists = true,
                Title = "Select plugin to add",
                Filter = "Zip file (*.zip)|*.zip|All files (*.*)|*"
            };
            if (openFileDialog.ShowDialog() == true)
            {
                pluginManager.TryAddPlugin(openFileDialog.FileName);
            }
        }
    }
}
