using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using Microsoft.Extensions.DependencyInjection;
using WClipboard.Core.Utilities;
using WClipboard.Core.WPF.Utilities;

namespace WClipboard.Core.WPF.Managers
{
    public interface ITypeDataTemplateManager : IAddOnlyDictionary<Type, DataTemplate>
    {
    }

    public class TypeDataTemplateManager : AddOnlyDictionaryProxy<Type, DataTemplate>, ITypeDataTemplateManager
    {
    }

    public static class ITypeDateTemplateManagerExtensions
    {
        public static void AddTypeDateTemplate<ForType>(this IServiceProvider serviceProvider, string resourceDictionaryLocation, object templateKey)
        {
            serviceProvider.GetService<ITypeDataTemplateManager>().Add(typeof(ForType), (DataTemplate)
                ResourceDictionaryUtilities.Get(resourceDictionaryLocation, typeof(ForType).Assembly)
                [templateKey]);
        }

        public static void AddTypeDateTemplate<ForType>(this IServiceProvider serviceProvider, string resourceDictionaryLocation)
        {
            var rd = ResourceDictionaryUtilities.Get(resourceDictionaryLocation, typeof(ForType).Assembly);
            foreach (var possibleKey in GetPossibleKeys<ForType>())
            {
                if(rd.Contains(possibleKey))
                {
                    serviceProvider.GetService<ITypeDataTemplateManager>().Add(typeof(ForType), (DataTemplate)rd[possibleKey]);
                    return;
                }
            }
            foreach(var item in rd.Values.OfType<DataTemplate>())
            {
                if(item.DataType is Type dt && dt == typeof(ForType))
                {
                    serviceProvider.GetService<ITypeDataTemplateManager>().Add(typeof(ForType), item);
                    return;
                }
            }
            throw new KeyNotFoundException($"Cannot find a {nameof(DataTemplate)} for {typeof(ForType).Name} inside {resourceDictionaryLocation}");
        }

        private static IEnumerable<object> GetPossibleKeys<ForType>()
        {
            var templateKey = typeof(ForType).Name;
            if (templateKey.EndsWith("Model", StringComparison.OrdinalIgnoreCase))
            {
                templateKey = templateKey.Substring(0, templateKey.Length - "Model".Length);
                yield return templateKey;
            }
            yield return templateKey;
            yield return typeof(ForType);
            yield return templateKey + "View";
        }

        public static void AddTypeDateTemplate<ForType>(this IServiceProvider serviceProvider, DataTemplate template)
        {
            serviceProvider.GetService<ITypeDataTemplateManager>().Add(typeof(ForType), template);
        }
    }
}
