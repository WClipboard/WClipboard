using System.Collections;
using System.Collections.Generic;
using System.Linq;
using WClipboard.Core.Extensions;

namespace WClipboard.Core.WPF.ViewModels
{
    public interface IViewModelFactoriesManager : IEnumerable<IViewModelFactory>
    {
        BaseViewModel? GetViewModel(object model, object? parent);
    }

    public class ViewModelFactoriesManager : IViewModelFactoriesManager
    {
        private readonly List<IViewModelFactory> factories;

        public ViewModelFactoriesManager(IEnumerable<IViewModelFactory> factories)
        {
            this.factories = new List<IViewModelFactory>(factories);
        }

        public IEnumerator<IViewModelFactory> GetEnumerator() =>  factories.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => ((IEnumerable)factories).GetEnumerator();

        public BaseViewModel? GetViewModel(object model, object? parent)
        {
            return factories.Select(f => f.Create(model, parent)).NotNull().FirstOrDefault();
        }
    }
}
