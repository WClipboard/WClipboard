using Microsoft.Win32;
using System;
using WClipboard.Core;
using WClipboard.Core.WPF.Settings;
using WClipboard.Core.WPF.Settings.Defaults;
using WClipboard.Core.WPF.ViewModels;

namespace WClipboard.App.Settings
{
    public class OpenOnStartupSettingsApplier : VirtualSettingApplier<bool?>
    {
        private const string CurrentUserRegisteryKey = @"HKEY_CURRENT_USER\";
        private const string StartupRegisteryKey = @"SOFTWARE\Microsoft\Windows\CurrentVersion\Run";
        private const string DisabledRegisteryKey = @"SOFTWARE\Microsoft\Windows\CurrentVersion\Explorer\StartupApproved\Run";

        private readonly IAppInfo appInfo;

        public OpenOnStartupSettingsApplier(IAppInfo appInfo) :base(SettingChangeMode.Direct, SettingChangeEffect.AtOnce)
        {
            this.appInfo = appInfo;
        }

        public override bool? GetCurrentValue(SettingViewModel<bool?> setting)
        {
            setting.MessageBar = null;

            try
            {
                var startupValue = Registry.GetValue(CurrentUserRegisteryKey + StartupRegisteryKey, appInfo.Name, null);

                if (!string.IsNullOrEmpty(startupValue as string))
                {
                    //check for windows 10 disable
                    var disableValue = Registry.GetValue(CurrentUserRegisteryKey + DisabledRegisteryKey, appInfo.Name, null);

                    if (disableValue is byte[] castedDisabledRun && castedDisabledRun[0] != 2)
                    {
                        setting.MessageBar = new MessageBarViewModel(MessageBarType.Information, MessageBarLevel.Low, "Disabled in TaskManager");
                        return null;
                    }
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception ex)
            {
                setting.MessageBar = new MessageBarViewModel(MessageBarType.Error, MessageBarLevel.Medium, $"Could not get current state :( \n {ex.Message}");
                return null;
            }
        }

        public static void RemoveStartup(IAppInfo appInfo)
        {
            using (var systemRunKey = Registry.CurrentUser.OpenSubKey(StartupRegisteryKey, true))
            {
                systemRunKey?.DeleteValue(appInfo.Name, false);
            }
        }

        public override void Apply(SettingViewModel<bool?> setting)
        {
            var newValue = setting.Value;
            var oldValue = GetCurrentValue(setting);

            if (newValue.HasValue)
            {
                try
                {
                    if (newValue.Value)
                    {
                        if (!oldValue.HasValue)
                        {
                            Registry.SetValue(CurrentUserRegisteryKey + DisabledRegisteryKey, appInfo.Name, new byte[] { 2, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 }, RegistryValueKind.Binary);
                        }
                        else if (!oldValue.Value)
                        {
                            Registry.SetValue(CurrentUserRegisteryKey + StartupRegisteryKey, appInfo.Name, $"\"{appInfo.Path}\"", RegistryValueKind.String);
                        }
                    }
                    else
                    {
                        if (!oldValue.HasValue)
                        {
                            using(var systemRunKey = Registry.CurrentUser.OpenSubKey(DisabledRegisteryKey, true))
                            {
                                systemRunKey?.DeleteValue(appInfo.Name, false);
                            }
                        }

                        using (var systemRunKey = Registry.CurrentUser.OpenSubKey(StartupRegisteryKey, true))
                        {
                            systemRunKey?.DeleteValue(appInfo.Name, false);
                        }
                    }
                }
                catch (System.Security.SecurityException)
                {
                    //Try with UAC
                }
                catch (Exception)
                {

                }

                setting.Value = GetCurrentValue(setting);
            }
        }
    }
}
