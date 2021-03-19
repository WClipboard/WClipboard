using System.Windows.Input;

namespace WClipboard.Core.WPF.Models
{
    public abstract class AbstractCommandBinding : CommandBinding
    {
        protected AbstractCommandBinding(ICommand command) : base(command)
        {
            CanExecute += HandleCanExecute;
            Executed += HandleExecute;
        }

        public abstract void HandleExecute(object sender, ExecutedRoutedEventArgs e);
        public abstract void HandleCanExecute(object sender, CanExecuteRoutedEventArgs e);
    }
}
