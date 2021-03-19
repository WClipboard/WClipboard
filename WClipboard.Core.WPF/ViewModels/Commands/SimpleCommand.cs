using System;
using System.Windows.Input;

namespace WClipboard.Core.WPF.ViewModels.Commands
{
    public class SimpleCommand : ICommand
    {
        private readonly Action<object> _execute;
        private bool _canExecute;

        public bool CanExecute
        {
            get => _canExecute;
            set
            {
                if (_canExecute != value)
                {
                    _canExecute = value;
                    CanExecuteChanged?.Invoke(this, new EventArgs());
                }
            }
        }


        public event EventHandler? CanExecuteChanged;

        bool ICommand.CanExecute(object parameter) => CanExecute;

        public void Execute(object parameter) => _execute(parameter);

        public SimpleCommand(Action<object> execute, bool canExecute = true)
        {
            _execute = execute;
            _canExecute = canExecute;
        }
    }
}
