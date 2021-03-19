using System.Collections.Generic;
using System.Linq;
using System;
using WClipboard.Core.Settings;

namespace WClipboard.Core.WPF.Settings
{
    public interface IUISettingsManager
    {
        IEnumerable<ISetting> Settings { get; }
        SettingViewModel? CreateFor(ISetting setting);
        IEnumerable<SettingViewModel> CreateAll();
    }

    public class UISettingsManager : IUISettingsManager
    {
        private readonly Dictionary<ISetting, BaseUISettingsFactory> directRef;

        public IEnumerable<ISetting> Settings => directRef.Keys;

        public UISettingsManager(IIOSettingsManager ioSettingsManager, IEnumerable<BaseUISettingsFactory> factories)
        {
            directRef = new Dictionary<ISetting, BaseUISettingsFactory>();

            var ioSettingsDir = ioSettingsManager.GetSettings().ToDictionary(s => s.Key, s => s);
            foreach(var factory in factories)
            {
                foreach(var key in factory.SettingKeys)
                {
                    if (!key.Contains('.'))
                        throw new InvalidOperationException($"Every key in {factory.GetType().FullName}.{nameof(BaseUISettingsFactory.SettingKeys)} must contain a '.' i.e. must be in a section");

                    if(ioSettingsDir.TryGetValue(key, out var setting))
                    {
                        directRef.Add(setting, factory);
                    } 
                    else
                    {
                        directRef.Add(new VirtualSetting(key), factory);
                    }
                }
            }
        }

        public SettingViewModel? CreateFor(ISetting setting)
        {
            return directRef[setting].Create(setting);
        }

        public IEnumerable<SettingViewModel> CreateAll()
        {
            foreach(var kv in directRef)
            {
                var viewModel = kv.Value.Create(kv.Key);

                if(!(viewModel is null))
                    yield return viewModel;
            }
        }
    }
}
