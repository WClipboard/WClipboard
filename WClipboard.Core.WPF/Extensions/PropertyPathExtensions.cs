using System.Windows;
using System.Windows.Data;

namespace WClipboard.Core.WPF.Extensions
{
    public static class PropertyPathExtensions
    {
        public static object Resolve(this PropertyPath propertyPath, object source)
        {
            var binding = new Binding
            {
                Source = source,
                Path = propertyPath,
                Mode = BindingMode.OneTime
            };

            var evaluator = new BindingEvaluator();
            BindingOperations.SetBinding(evaluator, BindingEvaluator.TargetProperty, binding);
            var value = evaluator.Target;
            BindingOperations.ClearBinding(evaluator, BindingEvaluator.TargetProperty);
            return value;
        }

        private class BindingEvaluator : DependencyObject
        {
            public static readonly DependencyProperty TargetProperty =
                DependencyProperty.Register(
                    nameof(Target),
                    typeof(object),
                    typeof(BindingEvaluator));

            public object Target
            {
                get { return GetValue(TargetProperty); }
                set { SetValue(TargetProperty, value); }
            }
        }
    }
}
