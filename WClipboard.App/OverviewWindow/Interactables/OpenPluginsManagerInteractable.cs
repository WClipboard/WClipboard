using System;
using WClipboard.App.PluginsWindow;
using WClipboard.Core.DI;
using WClipboard.Core.WPF.Models;
using WClipboard.Core.WPF.ViewModels;

namespace WClipboard.App.OverviewWindow.Interactables
{
    public class OpenPluginsManagerInteractable : Interactable<IMainWindowViewModel>
    {
        public OpenPluginsManagerInteractable(IServiceProvider serviceProvider) : base("PluginIcon", new OpenPluginsManagerInteractableAction(serviceProvider)) { }

        private class OpenPluginsManagerInteractableAction : InteractableAction<IMainWindowViewModel>
        {
            private readonly IServiceProvider serviceProvider;

            public OpenPluginsManagerInteractableAction(IServiceProvider serviceProvider) : base("Edit settings")
            {
                this.serviceProvider = serviceProvider;
            }

            protected override void Execute(IMainWindowViewModel parameter)
            {
                serviceProvider.Create<PluginsWindowViewModel>(parameter.Window);
            }
        }
    }
}
