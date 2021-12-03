using System.Linq;
using System.Windows;

namespace WClipboard.App.SettingsWindow
{
    public class HeaderViewModel
    {
        public string Key { get; }

        public Thickness Margin { get; }

        public int FontSize { get; }

        public FontWeight FontWeight { get; }
        
        public HeaderViewModel(string key)
        {
            Key = key;
            int dotCount = key.ToCharArray().Count(c => c == '.');

            Margin = new Thickness(0, 20 - (dotCount * 5), 0, 0);
            FontSize = 18 - (dotCount * 2);

            FontWeight = FontWeights.Normal;
            if (dotCount == 1)
            {
                FontWeight = FontWeights.Bold;
            }
        }
    }
}
