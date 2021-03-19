using System;
using System.Collections.Generic;
using System.Linq;
using WClipboard.Core.Clipboard.Format;

#nullable enable

namespace WClipboard.Core.WPF.Clipboard.ViewModel.Filters.Defaults
{
    public class FormatFiltersProvider : IFiltersProvider
    {
        private readonly List<Filter> filters;

        public FormatFiltersProvider(IClipboardFormatsManager formatsManager)
        {
            filters = new List<Filter>();

            foreach (var category in formatsManager.Values.Select(f => f.Category).Distinct())
            {
                filters.Add(new CategoryFilter(category));
            }

            foreach (var format in formatsManager.Values)
            {
                filters.Add(new FormatFilter(format));
            }
        }

        public IEnumerable<Filter> GetFilters(string text)
        {
            if (string.IsNullOrEmpty(text))
                return filters.OfType<CategoryFilter>();

            return filters.Where(f => f.Text.StartsWith(text, StringComparison.CurrentCultureIgnoreCase));
        }

        private class CategoryFilter : Filter
        {
            private readonly ClipboardFormatCategory category;

            public CategoryFilter(ClipboardFormatCategory category) : base(category.Name, category.IconSource)
            {
                this.category = category;
            }

            public override bool Passes(ClipboardObjectViewModel clipboardObjectViewModel)
            {
                return clipboardObjectViewModel.Implementations.Any(i => i.Model.Format.Category == category);
            }
        }

        private class FormatFilter : Filter
        {
            private readonly ClipboardFormat format;

            public FormatFilter(ClipboardFormat format) : base(format.Name, format.IconSource)
            {
                this.format = format;
            }

            public override bool Passes(ClipboardObjectViewModel clipboardObjectViewModel)
            {
                return clipboardObjectViewModel.Implementations.Any(i => i.Model.Format == format);
            }
        }
    }
}
