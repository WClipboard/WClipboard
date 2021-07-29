using System;
using System.Windows.Input;

namespace WClipboard.Core.WPF.Utilities
{
    public class SimpleCommand : ICommand
    {
        private readonly Func<object?, bool>? canExecute;
        private readonly Action<object?> execute;

        public event EventHandler? CanExecuteChanged;

        public bool CanExecute(object? parameter) => canExecute?.Invoke(parameter) ?? true;

        public void Execute(object? parameter) => execute(parameter);

        public SimpleCommand(Action<object?> execute, Func<object?, bool>? canExecute)
        {
            this.execute = execute;
            this.canExecute = canExecute;
        }

        public static ICommand Create(Action<object?> execute, Func<object?, bool>? canExecute = null) => new SimpleCommand(execute, canExecute);
        public static ICommand Create(Action execute, Func<bool>? canExecute = null)
        {
            if (canExecute is null)
                return Create((_) => execute());
            else
                return Create((_) => execute(), (_) => canExecute());
        }
        public static ICommand Create<T>(Action<T> execute, Func<T, bool>? canExecute = null) => SimpleCommand<T>.Create(execute, canExecute);

        protected void OnCanExecuteChanged()
        {
            CanExecuteChanged?.Invoke(this, new EventArgs());
        }
    }

    public class SimpleCommand<T> : ICommand
    {
        public event EventHandler? CanExecuteChanged;

        private readonly Func<T, bool>? canExecute;
        private readonly Action<T> execute;

        public bool CanExecute(object? parameter) => parameter is T Tparam && (canExecute?.Invoke(Tparam) ?? true);
        public void Execute(object? parameter) => execute((T)parameter!);

        public SimpleCommand(Action<T> execute, Func<T, bool>? canExecute = null)
        {
            this.execute = execute;
            this.canExecute = canExecute;
        }

        public static ICommand Create(Action<T> execute, Func<T, bool>? canExecute = null) => new SimpleCommand<T>(execute, canExecute);

        protected void OnCanExecuteChanged()
        {
            CanExecuteChanged?.Invoke(this, new EventArgs());
        }
    }
}
