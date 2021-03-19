using System.Collections.Generic;
using System.Linq;

#nullable enable

namespace WClipboard.Core.WPF.Clipboard.ViewModel.Filters
{
    public interface IFiltersManager
    {
        IEnumerable<Filter> GetFilters(string text);
    }

    public class FiltersManager : IFiltersManager
    {
        private readonly List<IFiltersProvider> filterProviders;

        public FiltersManager(IEnumerable<IFiltersProvider> filterProviders)
        {
            this.filterProviders = new List<IFiltersProvider>(filterProviders);
        }

        public IEnumerable<Filter> GetFilters(string text)
        {
            foreach (var filterProvider in filterProviders)
            {
                var filters = filterProvider.GetFilters(text);
                foreach (var filter in filters ?? Enumerable.Empty<Filter>())
                {
                    yield return filter;
                }
            }
        }
    }
}
