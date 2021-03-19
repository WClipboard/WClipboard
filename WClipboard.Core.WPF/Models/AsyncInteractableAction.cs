using System.Threading.Tasks;
using System.Windows.Input;

#nullable enable

namespace WClipboard.Core.WPF.Models
{
    public abstract class AsyncInteractableAction : InteractableAction
    {
        protected AsyncInteractableAction(string name, KeyGesture? keyGesture = null) : base(name, keyGesture)
        {
        }

        protected AsyncInteractableAction(string name, MouseGesture mouseGesture, KeyGesture? keyGesture = null) : base(name, mouseGesture, keyGesture)
        {
        }

        public override async void Execute(object parameter)
        {
            canExecute = false;
            OnCanExecutedChanged();

            await ExecuteAsync(parameter);

            canExecute = true;
            OnCanExecutedChanged();
        }

        protected abstract Task ExecuteAsync(object parameter);
    }

    public abstract class AsyncInteractableAction<TViewModel> : InteractableAction<TViewModel>
    {
        protected AsyncInteractableAction(string name, KeyGesture? keyGesture = null) : base(name, keyGesture)
        {
        }

        protected AsyncInteractableAction(string name, MouseGesture mouseGesture, KeyGesture? keyGesture = null) : base(name, mouseGesture, keyGesture)
        {
        }

        protected override async void Execute(TViewModel parameter)
        {
            canExecute = false;
            OnCanExecutedChanged();

            await ExecuteAsync(parameter);

            canExecute = true;
            OnCanExecutedChanged();
        }

        protected abstract Task ExecuteAsync(TViewModel parameter);
    }
}
