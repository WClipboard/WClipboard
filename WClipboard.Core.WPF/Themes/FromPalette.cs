using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Data;
using System.Windows.Markup;
using System.Windows.Media;
using WClipboard.Core.Utilities.Collections;
using Microsoft.Extensions.DependencyInjection;
using System.Windows.Media.Animation;

namespace WClipboard.Core.WPF.Themes
{
    [MarkupExtensionReturnType(typeof(Brush))]
    public class FromPalette : MarkupExtension
    {
        [DefaultValue(null)]
        public string? Overload { get; set; }
        [DefaultValue(null)]
        public IValueConverter? Converter { get; set; }
        [DefaultValue(null)]
        public object? ConverterParameter { get; set; }
        [DefaultValue(null)]
        [TypeConverter(typeof(CultureInfoIetfLanguageTagConverter))]
        public CultureInfo? ConverterCulture { get; set; }

        /// <summary>
        /// The fallback property type if the target property type cannot be identified (can happen by Setters in some cases)
        /// </summary>
        [DefaultValue(typeof(Brush))]
        public Type FallbackPropertyType { get; set; } = typeof(Brush);

        public FromPalette() { }
        public FromPalette(string overload)
        {
            Overload = overload;
        }

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            var provideValueTarget = serviceProvider.GetService<IProvideValueTarget>();
            if (provideValueTarget != null)
            {
                return ProvideValue(provideValueTarget.TargetObject, provideValueTarget.TargetProperty, serviceProvider);
            }
            else 
            {
                throw new NotSupportedException();
            }
        }

        public object ProvideValue(object targetObject, object targetProperty, IServiceProvider? serviceProvider)
        {
            if (targetObject is Setter setter)
            {
                var resourceKey = GetResourceKey(setter.Property);
                if (Palette.IsDynamicResourceKey(resourceKey))
                {
                    Debug.WriteLineIf(Converter != null, $"Warning: {nameof(Converter)} set on {nameof(FromPalette)} but when {nameof(targetObject)} is not a {nameof(FrameworkElement)} and has no Static ${nameof(Overload)} the {nameof(Converter)} is ignored");
                    return new DynamicResourceExtension(resourceKey);
                }
                else //static
                {
                    return ApplyConverter(Application.Current.FindResource(resourceKey), setter.Property?.PropertyType ?? FallbackPropertyType);
                }
            } 
            else if(targetObject is DiscreteObjectKeyFrame)
            {
                var resourceKey = GetResourceKey(null);
                return ApplyConverter(Application.Current.FindResource(resourceKey), FallbackPropertyType);
            }
            else if(targetObject is ColorAnimationUsingKeyFrames)
            {
                FallbackPropertyType = typeof(Color);
                var resourceKey = GetResourceKey(null);
                return ApplyConverter(Application.Current.FindResource(resourceKey), FallbackPropertyType);
            }
            else if (targetProperty is DependencyProperty dp)
            {
                var element = targetObject as FrameworkElement;

                var resourceKey = GetResourceKey(dp);
                if (element != null)
                {
                    return ApplyConverter(element.FindResource(resourceKey), dp.PropertyType);
                }
                else if (targetObject is FrameworkContentElement fce)
                {
                    //Walk up the path to find a framework element
                    var parentTree = RecursiveEnumerable.Get((object?)fce, fce => (fce as FrameworkContentElement)?.Parent, null);
                    element = parentTree.FirstOrDefault(i => i is FrameworkElement) as FrameworkElement;
                    if (element != null)
                    {
                        return ApplyConverter(element.FindResource(resourceKey), dp.PropertyType);
                    }
                    else if (!Palette.IsDynamicResourceKey(resourceKey))
                    {
                        return ApplyConverter(Application.Current.FindResource(resourceKey), dp.PropertyType);
                    }
                    else
                    {
                        throw new NotSupportedException($"For a {nameof(FrameworkContentElement)} target there must be a {nameof(FrameworkElement)} in its parent tree. Or it should not depend on an active {nameof(Palette)}");
                    }
                }
                else if (Palette.IsDynamicResourceKey(resourceKey) && serviceProvider != null)
                {
                    Debug.WriteLineIf(Converter != null, $"Warning: {nameof(Converter)} set on {nameof(FromPalette)} but when {nameof(targetObject)} is not a {nameof(FrameworkElement)} and has no Static ${nameof(Overload)} the {nameof(Converter)} is ignored");
                    return new DynamicResourceExtension(resourceKey).ProvideValue(serviceProvider);
                }
                else //static
                {
                    return ApplyConverter(Application.Current.FindResource(resourceKey), dp.PropertyType);
                }
            }
            else
            {
                var resourceKey = GetResourceKey(null);
                return ApplyConverter(Application.Current.FindResource(resourceKey), FallbackPropertyType);
            }
        }

        private object ApplyConverter(object resource, Type propertyType)
        {
            if (Converter != null)
            {
                resource = Converter.Convert(resource, propertyType, ConverterParameter, ConverterCulture ?? CultureInfo.CurrentUICulture);
            }

            return resource;
        }

        private string GetResourceKey(DependencyProperty? dp)
        {
            if (!string.IsNullOrEmpty(Overload))
            {
                return Palette.GetResourceKeyFromOverload(Overload, (dp?.PropertyType ?? FallbackPropertyType).Name);
            }
            else if (dp != null)
            {
                return Palette.GetResourceKeyFromProperty(dp);
            }
            else
            {
                throw new NotSupportedException($"Must have an {nameof(Overload)} or recieve a {nameof(IProvideValueTarget.TargetProperty)}");
            }
        }
    }
}
