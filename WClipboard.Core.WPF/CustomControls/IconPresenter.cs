using System;
using System.ComponentModel;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using WClipboard.Core.Extensions;
using WClipboard.Core.WPF.Extensions;
using WClipboard.Core.WPF.ViewModels.Icons;
//using Symbol = Windows.UI.Xaml.Controls.Symbol;

namespace WClipboard.Core.WPF.CustomControls
{
    public class IconPresenter : Viewbox
    {
        public static readonly DependencyProperty IconProperty = DependencyProperty.Register(nameof(Icon), typeof(object), typeof(IconPresenter), new FrameworkPropertyMetadata(OnIconPropertyChanged), IsValidIcon);

        [Description("IconSource to display."), Category("Common Properties")]
        public object Icon
        {
            get => GetValue(IconProperty);
            set => SetValue(IconProperty, value);
        }

        private static bool IsValidIcon(object value)
        {
            if (value is string iconName)
            {
                if (iconName.StartsWith("WI:"))
                {
                    var symbolName = iconName[3..];
                    if (symbolName.StartsWith("0x"))
                        return int.TryParse(symbolName, NumberStyles.HexNumber, null, out int result) && result < char.MaxValue && result > char.MinValue;
                    //else if (Enum.TryParse<Symbol>(symbolName, out var _))
                    //    return true;
                    else
                        return int.TryParse(symbolName, NumberStyles.Integer, null, out int result) && result < char.MaxValue && result > char.MinValue;
                }
                if (iconName.StartsWith("T:"))
                    return true;

                value = Application.Current.TryFindResource(iconName);
                if (value == null)
                    return false;
            }
            if (value is System.Drawing.Bitmap)
                return true;
            if (value is System.Drawing.Icon)
                return true;
            if (value is ImageSource)
                return true;
            if (value is ControlTemplate controlTemplate && controlTemplate.TargetType == typeof(Icon))
                return true;
            if (value is Uri)
                return true;
            if (value is TextIcon)
                return true;
            if (value is WindowsFontIcon)
                return true;
            if (value == null)
                return true;

            return false;
        }

        private static void OnIconPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is IconPresenter i)
                i.OnIconPropertyChanged();
        }

        private void OnIconPropertyChanged()
        {
            var icon = Icon;

            if (icon is string iconName)
            {
                if (iconName.StartsWith("WI:"))
                {
                    var symbolName = iconName[3..];
                    if (symbolName.StartsWith("0x"))
                        icon = new WindowsFontIcon((char)Convert.ToInt32(symbolName, 16));
                    //else if (Enum.TryParse<Symbol>(symbolName, out var symbol))
                    //    icon = new WindowsFontIcon(symbol);
                    else
                        icon = new WindowsFontIcon((char)Convert.ToInt32(symbolName, 10));
                }
                else if (iconName.StartsWith("T:"))
                    icon = new TextIcon(iconName[2..]);
                else
                    icon = Application.Current.TryFindResource(iconName);
            }

            if (icon is Uri uri)
                icon = BitmapFrame.Create(uri);

            if (icon is System.Drawing.Bitmap bitmap)
                icon = bitmap.ToBitmapSource();

            if (icon is System.Drawing.Icon _icon)
                icon = _icon.ToBitmapSource();

            if (icon is ImageSource imageSource)
            {
                Child = new Image() { Source = imageSource };
            }
            else if (icon is ControlTemplate controlTemplate && controlTemplate.TargetType == typeof(Icon))
            {
                Child = new Icon() { Template = controlTemplate };
            }
            else if (icon is TextIcon)
            {
                Child = new Icon() { Template = Application.Current.FindResource<ControlTemplate>("TextIconTemplate"), DataContext = icon };
            }
            else if (icon is WindowsFontIcon)
            {
                Child = new Icon() { Template = Application.Current.FindResource<ControlTemplate>("WindowsFontIconTemplate"), DataContext = icon };
            }
            else
            {
                Child = null;
            }
        }
    }
}
