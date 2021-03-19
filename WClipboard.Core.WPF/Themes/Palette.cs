using System;
using System.Linq;
using System.Windows;
using System.Windows.Media;
using WClipboard.Core.WPF.Extensions;

namespace WClipboard.Core.WPF.Themes
{
    public class Palette
    {
        private static readonly string[] _supportedTypes = new[] { nameof(Brush), nameof(Color) };

        public static readonly DependencyProperty ActiveProperty = DependencyProperty.RegisterAttached("Active", typeof(object), typeof(Palette), new FrameworkPropertyMetadata(OnActivePropertyChanged), IsValidPalette);

        private static bool IsValidPalette(object value)
        {
            return value is null || value is Palette || value is string || value is Enum;
        }

        public static object GetActive(FrameworkElement frameworkElement)
        {
            return frameworkElement.GetValue(ActiveProperty);
        }

        public static void SetActive(FrameworkElement frameworkElement, object value)
        {
            frameworkElement.SetValue(ActiveProperty, value);
        }

        private static void OnActivePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is FrameworkElement frameworkElement)
            {
                if (!(e.NewValue is Palette palette))
                {
                    var resource = e.NewValue as string;
                    if (e.NewValue is Enum @enum)
                        resource = @enum.ToString();

                    palette = frameworkElement.TryFindResource<Palette>(resource + nameof(Palette));
                }

                ResetResource(frameworkElement, nameof(Foreground), palette?.Foreground);
                ResetResource(frameworkElement, nameof(Background), palette?.Background);
                ResetResource(frameworkElement, nameof(Border), palette?.Border);
            }
        }

        private static void ResetResource(FrameworkElement frameworkElement, string propertyName, string? propertyValue)
        {
            foreach (var type in _supportedTypes)
            {
                string resourceName = $"{nameof(Palette)}{propertyName}{type}";

                if (propertyValue != null)
                {
                    frameworkElement.Resources[resourceName] = frameworkElement.FindResource($"{propertyValue}{type}");
                }
                else
                {
                    frameworkElement.Resources.Remove(resourceName);
                }
            }
        }

        internal static string GetResourceKeyFromProperty(DependencyProperty dp) =>
            GetResourceKeyFromProperty(dp.Name, dp.PropertyType.Name);

        internal static string GetResourceKeyFromProperty(string propertyName, string propertyType)
        {
            if (!_supportedTypes.Contains(propertyType))
            {
                throw new NotSupportedException($"{propertyType} is not a valid type for a {nameof(Palette)}, supports only: {string.Join(" ", _supportedTypes)}");
            }

            return propertyName switch
            {
                nameof(System.Windows.Controls.Control.Foreground) => $"{nameof(Palette)}{nameof(Foreground)}{propertyType}",
                nameof(System.Windows.Controls.Control.Background) => $"{nameof(Palette)}{nameof(Background)}{propertyType}",
                nameof(System.Windows.Controls.Control.BorderBrush) => $"{nameof(Palette)}{nameof(Border)}{propertyType}",
                _ => throw new NotSupportedException($"{propertyName} is not a valid target for a {nameof(Palette)}, please use the overload"),
            };
        }

        internal static string GetResourceKeyFromOverload(string overload, string propertyType)
        {
            if (!_supportedTypes.Contains(propertyType))
            {
                throw new NotSupportedException($"{propertyType} is not a valid type for a {nameof(Palette)}, supports only: {string.Join(" ", _supportedTypes)}");
            }

            return overload switch
            {
                nameof(Foreground) => $"{nameof(Palette)}{nameof(Foreground)}{propertyType}",
                nameof(Background) => $"{nameof(Palette)}{nameof(Background)}{propertyType}",
                nameof(Border) => $"{nameof(Palette)}{nameof(Border)}{propertyType}",
                _ => $"{overload}{propertyType}",
            };
        }

        internal static bool IsDynamicResourceKey(string resourceKey)
        {
            return resourceKey.StartsWith(nameof(Palette));
        }

        public string? Foreground { get; set; }
        public string? Background { get; set; }
        public string? Border { get; set; }
    }
}
