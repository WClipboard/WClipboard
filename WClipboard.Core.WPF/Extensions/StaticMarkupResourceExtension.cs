using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Markup;

namespace WClipboard.Core.WPF.Extensions
{
    public class StaticMarkupResourceExtension : StaticResourceExtension
    {
        public StaticMarkupResourceExtension()
        {
        }

        public StaticMarkupResourceExtension(object resourceKey) : base(resourceKey)
        {
        }

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            var result = base.ProvideValue(serviceProvider);
            if(result is MarkupExtension markupExtension)
            {
                return markupExtension.ProvideValue(serviceProvider);
            } 
            else
            {
                return result;
            }
        }
    }
}
