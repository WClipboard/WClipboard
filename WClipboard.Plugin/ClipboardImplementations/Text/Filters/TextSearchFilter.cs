using System;
using System.Collections.Generic;
using System.Linq;
using WClipboard.Core.WPF.Clipboard.ViewModel;
using WClipboard.Core.WPF.Clipboard.ViewModel.Filters;

namespace WClipboard.Plugin.ClipboardImplementations.Text.Filters
{
    public class TextSearchFilterProvider : IFiltersProvider
    {
        public IEnumerable<Filter> GetFilters(string text)
        {
            if (!string.IsNullOrEmpty(text))
            {
                yield return new TextSearchFilter(text);
            }
        }
    }

    public class TextSearchFilter : Filter, IEquatable<Filter>
    {
        private readonly string searchText;

        public TextSearchFilter(string searchText) : base($"Search \"{searchText}\"", "T:T")
        {
            this.searchText = searchText;
        }

        public bool Equals(Filter other)
        {
            if (other is TextSearchFilter textSearchFilter)
            {
                return textSearchFilter.Text == searchText;
            }
            else
            {
                return base.Equals(other);
            }
        }

        public override bool Passes(ClipboardObjectViewModel clipboardObjectViewModel)
        {
            var textImplemenationVM = clipboardObjectViewModel.Implementations.OfType<TextClipboardImplementationViewModel>().FirstOrDefault();

            if (textImplemenationVM != null)
            {
                return textImplemenationVM.Model.Source.Contains(searchText);
            }
            else
            {
                return false;
            }
        }
    }
}
