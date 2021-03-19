using System.Collections.Generic;

#nullable enable

namespace WClipboard.Core.WPF.Clipboard.ViewModel.Filters
{
    public interface IFiltersProvider
    {
        IEnumerable<Filter> GetFilters(string text);
    }
}
