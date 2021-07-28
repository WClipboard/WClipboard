using System.Collections.Generic;
using WClipboard.Core.Utilities;
using WClipboard.Core.WPF.Utilities;

namespace WClipboard.Core.WPF.ViewModels
{
    public interface IViewModel
    {
        object Model { get; }
    }
    public interface IViewModel<TModel> : IViewModel
    {
        new TModel Model { get; }
    }

    public abstract class BaseViewModel : BindableBase, IViewModel
    {
        public object Model { get; }

        protected BaseViewModel(object model)
        {
            Model = model;
        }
    }

    public abstract class BaseViewModel<TModel> : BaseViewModel, IViewModel<TModel> where TModel : notnull 
    {

        public new TModel Model => (TModel)base.Model;

        protected BaseViewModel(TModel model) : base(model) { }
    }

    public abstract class BaseViewModelWithInteractables : BaseViewModel, IHasAssignableInteractables
    {
        public ConcurrentBindableList<InteractableState> Interactables { get; }

        protected BaseViewModelWithInteractables(object model) : this(model, new ConcurrentBindableList<InteractableState>())
        {
        }

        protected BaseViewModelWithInteractables(object model, ConcurrentBindableList<InteractableState> interactables) : base(model)
        {
            Interactables = interactables;
        }

        IReadOnlyList<InteractableState> IHasInteractables.Interactables => Interactables;
    }

    public abstract class BaseViewModelWithInteractables<TModel> : BaseViewModel<TModel>, IHasAssignableInteractables where TModel : notnull
    {
        public ConcurrentBindableList<InteractableState> Interactables { get; }

        protected BaseViewModelWithInteractables(TModel model) : this(model, new ConcurrentBindableList<InteractableState>())
        {
        }

        protected BaseViewModelWithInteractables(TModel model, ConcurrentBindableList<InteractableState> interactables) : base(model)
        {
            Interactables = interactables;
        }

        IReadOnlyList<InteractableState> IHasInteractables.Interactables => Interactables;
    }
}
