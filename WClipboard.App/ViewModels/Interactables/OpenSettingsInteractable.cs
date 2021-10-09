using System;
using WClipboard.Core.DI;
using WClipboard.Core.WPF.Models;
using WClipboard.Core.WPF.ViewModels;

namespace WClipboard.App.ViewModels.Interactables
{
    public class OpenSettingsInteractable : Interactable<IMainWindowViewModel>
    {
        public OpenSettingsInteractable(IServiceProvider serviceProvider) : base("SettingsIcon", new OpenSettingsInteractableAction(serviceProvider)) { }

        private class OpenSettingsInteractableAction : InteractableAction<IMainWindowViewModel>
        {
            private readonly IServiceProvider serviceProvider;

            public OpenSettingsInteractableAction(IServiceProvider serviceProvider) : base("Edit settings")
            {
                this.serviceProvider = serviceProvider;
            }

            protected override void Execute(IMainWindowViewModel parameter)
            {
                serviceProvider.Create<SettingsWindowViewModel>(parameter.Window);
            }
        }
    }
}
