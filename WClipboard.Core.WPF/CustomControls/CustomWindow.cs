using System.ComponentModel;
using System.Windows;
using System.Windows.Input;
using WClipboard.Windows.Helpers;

namespace WClipboard.Core.WPF.CustomControls
{
    public class CustomWindow : Window
    {
        public static RoutedCommand Maximize { get; } = new RoutedCommand(nameof(Maximize), typeof(CustomWindow));
        public static RoutedCommand Minimize { get; } = new RoutedCommand(nameof(Minimize), typeof(CustomWindow));
        public static RoutedCommand Restore { get; } = new RoutedCommand(nameof(Restore), typeof(CustomWindow));

        static CustomWindow()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(CustomWindow), new FrameworkPropertyMetadata(typeof(CustomWindow)));
        }

        public static readonly DependencyProperty HeaderStyleProperty = DependencyProperty.Register(nameof(HeaderStyle), typeof(Style), typeof(CustomWindow), new FrameworkPropertyMetadata(null));

        [Bindable(true), Category("Miscellaneous")]
        public Style HeaderStyle
        {
            get => (Style)GetValue(HeaderStyleProperty);
            set => SetValue(HeaderStyleProperty, value);
        }

        public CustomWindow()
        {
            WindowMaximizeHelper.AttachTo(this);

            var closeCommandBinding = new CommandBinding(ApplicationCommands.Close, Close_Handler);
            var maximizeCommandBinding = new CommandBinding(Maximize, Maximize_Handler, MaximizeRestore_Handler_CanExecute);
            var minimizeCommandBinding = new CommandBinding(Minimize, Minimize_Handler, Minimize_Handler_CanExecute);
            var restoreCommandBinding = new CommandBinding(Restore, Restore_Handler, Restore_Handler_CanExecute);

            var commandBindings = new[] { closeCommandBinding, maximizeCommandBinding, minimizeCommandBinding, restoreCommandBinding };

            foreach(var commandBinding in commandBindings)
            {
                CommandBindings.Add(commandBinding);
            }
        }

        private void Close_Handler(object s, RoutedEventArgs e) => Close();
        private void Maximize_Handler(object s, RoutedEventArgs e) => WindowState = WindowState.Maximized;
        private void Minimize_Handler(object s, RoutedEventArgs e) => WindowState = WindowState.Minimized;
        private void Restore_Handler(object s, RoutedEventArgs e) => WindowState = WindowState.Normal;

        private void MaximizeRestore_Handler_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = (ResizeMode == ResizeMode.CanResize || ResizeMode == ResizeMode.CanResizeWithGrip) && WindowState != WindowState.Maximized;
        }

        private void Minimize_Handler_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = ResizeMode == ResizeMode.CanMinimize || ResizeMode == ResizeMode.CanResize || ResizeMode == ResizeMode.CanResizeWithGrip;
        }

        private void Restore_Handler_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = (ResizeMode == ResizeMode.CanResize || ResizeMode == ResizeMode.CanResizeWithGrip) && WindowState != WindowState.Normal;
        }
    }
}
