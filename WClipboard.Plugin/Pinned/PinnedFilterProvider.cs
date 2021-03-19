using System;
using System.Collections.Generic;
using WClipboard.Core.WPF.Clipboard.ViewModel;
using WClipboard.Core.WPF.Clipboard.ViewModel.Filters;

#nullable enable

namespace WClipboard.Plugin.Pinned
{
    public class PinnedFilterProvider : IFiltersProvider
    {
        private readonly Filter pinnedFilter;
        public PinnedFilterProvider()
        {
            pinnedFilter = new PinnedFilter();
        }

        public IEnumerable<Filter> GetFilters(string text)
        {
            if (pinnedFilter.Text.StartsWith(text, StringComparison.CurrentCultureIgnoreCase))
                yield return pinnedFilter;
        }

        private class PinnedFilter : Filter
        {
            public PinnedFilter() : base("Pinned", "PinIcon")
            {
            }

            public override bool Passes(ClipboardObjectViewModel clipboardObjectViewModel)
            {
                return PinnedManager.IsPinned(clipboardObjectViewModel.Model);
            }
        }
    }
}
