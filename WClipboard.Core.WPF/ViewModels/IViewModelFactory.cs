
namespace WClipboard.Core.WPF.ViewModels
{
    public interface IViewModelFactory
    {
        BaseViewModel? Create(object model, object? parent);
    }

    public interface IViewModelFactory<TReturner, TModel, TParent> : IViewModelFactory where TReturner : BaseViewModel<TModel>? where TParent : class where TModel : notnull
    {
        TReturner Create(TModel model, TParent? parent);
    }

    public abstract class ViewModelFactory<TModel> : IViewModelFactory<BaseViewModel<TModel>?, TModel, object> where TModel : notnull
    {
        BaseViewModel? IViewModelFactory.Create(object model, object? parent) => model is TModel tModel ? Create(tModel, parent) : null;
        public abstract BaseViewModel<TModel>? Create(TModel model, object? parent);
    }
}
