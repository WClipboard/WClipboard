using System.Collections.Generic;

namespace WClipboard.App.ViewModels
{
    public class SectionSettingViewModel
    {
        public string Key { get; }
        public List<object> Childs { get; }

        public SectionSettingViewModel(string key)
        {
            Key = key;
            Childs = new List<object>();
        }
    }
}
