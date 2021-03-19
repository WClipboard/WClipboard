using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls.Primitives;
using System.Windows.Markup;

namespace WClipboard.Core.WPF.CustomControls
{
    [ContentProperty(nameof(ContentDictionary))]
    public class ToggleContentButton : ToggleButton
    {
        public static readonly DependencyProperty ContentDictionaryProperty = DependencyProperty.Register(nameof(ContentDictionary), typeof(Dictionary<bool, object>), typeof(ToggleContentButton), new FrameworkPropertyMetadata(OnPropertyChanged));

        private static void OnPropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e) => (sender as ToggleContentButton)?.UpdateContent();

        public Dictionary<bool, object> ContentDictionary
        {
            get => (Dictionary<bool, object>)GetValue(ContentDictionaryProperty); 
            set => SetValue(ContentDictionaryProperty, value);
        }

        protected void UpdateContent()
        {
            if (ContentDictionary is null)
            {
                Content = IsChecked;
            } 
            else
            {
                Content = ContentDictionary[IsChecked!.Value];
            }
        }

        protected override void OnChecked(RoutedEventArgs e)
        {
            base.OnChecked(e);

            UpdateContent();
        }

        protected override void OnUnchecked(RoutedEventArgs e)
        {
            base.OnUnchecked(e);

            UpdateContent();
        }
    }
}
