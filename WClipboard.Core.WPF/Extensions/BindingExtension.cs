using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Markup;
using Microsoft.Extensions.DependencyInjection;
using WClipboard.Core.Extensions;

namespace WClipboard.Core.WPF.Extensions
{
    public class BindingExtension : MarkupExtension
    {
        public Binding Base { get; set; }
        public PropertyPath? ValidationRulesPath { get; set; }
        public PropertyPath? PathPath { get; set; }

        //public BindingEx() { }
        public BindingExtension(Binding @base)
        {
            Base = @base;
        }

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            var baseReturner = Base.ProvideValue(serviceProvider);
            if(baseReturner is Binding baseReturnerSelf)
            {
                Base = baseReturnerSelf;
            }

            var source = Base.Source;

            if(source == null)
            {
                if (serviceProvider.GetService<IProvideValueTarget>()?.TargetObject is FrameworkElement frameworkElement)
                {
                    source = frameworkElement.DataContext;
                }
            }

            if(source == null)
            {
                return this;
            } 
            else
            {
                if(ValidationRulesPath != null && ValidationRulesPath.Resolve(source) is IEnumerable<ValidationRule> validationRules)
                {
                    Base.ValidationRules.AddRange(validationRules);
                }
                if(PathPath != null && PathPath.Resolve(source) is PropertyPath path)
                {
                    Base.Path = path;
                } 
                return baseReturner;
            }
        }
    }
}
