using WClipboard.App.Windows;
using WClipboard.Core.WPF.Models;
using WClipboard.Core.WPF.ViewModels;

namespace WClipboard.App.ViewModels.Interactables
{
    public class OpenSettingsInteractable : Interactable<IMainWindowViewModel>
    {
        public OpenSettingsInteractable() : base("SettingsIcon", new OpenSettingsInteractableAction()) { }

        private class OpenSettingsInteractableAction : InteractableAction<IMainWindowViewModel>
        {
            public OpenSettingsInteractableAction() : base("Edit settings")
            {
            }

            protected override void Execute(IMainWindowViewModel parameter)
            {
                var settingsWindow = new SettingsWindow
                {
                    Owner = parameter.Window
                };
                settingsWindow.Show();
            }
        }
    }
}
