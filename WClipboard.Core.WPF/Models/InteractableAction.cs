using System;
using System.Linq;
using System.Threading;
using System.Windows.Input;
using WClipboard.Core.Extensions;

#nullable enable

namespace WClipboard.Core.WPF.Models
{
    public abstract class InteractableAction : ICommand
    {
        public string Name { get; }
        public MouseGesture MouseGesture { get; }
        public KeyGesture? KeyGesture { get; }

        protected bool canExecute = true;

        public event EventHandler? CanExecuteChanged;

        protected InteractableAction(string name, MouseGesture mouseGesture, KeyGesture? keyGesture = null)
        {
            Name = name;
            MouseGesture = mouseGesture;
            KeyGesture = keyGesture;
        }

        protected InteractableAction(string name, KeyGesture? keyGesture = null) : this(name, new MouseGesture(MouseAction.LeftClick, ModifierKeys.None), keyGesture) { }

        public bool CanExecute(object parameter) => canExecute;

        protected void OnCanExecutedChanged()
        {
            CanExecuteChanged?.Invoke(this, new EventArgs());
        }

        public abstract void Execute(object parameter);

        internal string GetTooltip()
        {
            string? keyPart = KeyGesture?.GetDisplayStringForCulture(Thread.CurrentThread.CurrentUICulture);
            if (keyPart != null)
                keyPart = $"({keyPart})";
            string? mousePart = string.Join(" + ", new[] { MouseGesture.MouseAction.ToString() }.Concat(MouseGesture.Modifiers.GetFlags().Where(f => f != ModifierKeys.None).Select(m => m.ToString())));
            if (mousePart == MouseAction.LeftClick.ToString())
                mousePart = null;
            else if (mousePart != null)
                mousePart = $"({mousePart})";
            return string.Join(" ", new[] { Name, mousePart, keyPart }.NotNull());
        }
    }

    public abstract class InteractableAction<TViewModel> : InteractableAction {
        protected InteractableAction(string name, KeyGesture? keyGesture = null) : base(name, keyGesture)
        {
        }

        protected InteractableAction(string name, MouseGesture mouseGesture, KeyGesture? keyGesture = null) : base(name, mouseGesture, keyGesture)
        {
        }

        public override void Execute(object parameter) => Execute((TViewModel)parameter);
        protected abstract void Execute(TViewModel parameter);
    }
}
