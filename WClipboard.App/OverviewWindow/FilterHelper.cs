using System;
using WClipboard.Core.DI;
using WClipboard.Core.Utilities;
using WClipboard.Core.WPF.Clipboard.ViewModel.Filters;
using System.Linq;
using WClipboard.Core.WPF.Utilities;
using System.Windows.Input;
using System.Windows.Data;
using WClipboard.Core.WPF.Clipboard.ViewModel;

namespace WClipboard.App.OverviewWindow
{
    public class FilterHelper : BindableBase
    {
        private readonly Lazy<IFiltersManager> filtersManager = DiContainer.SP!.GetLazy<IFiltersManager>();

        private string searchText = string.Empty;
        public string SearchText
        {
            get => searchText;
            set => SetProperty(ref searchText, value, string.IsNullOrEmpty(value) || !isSelectedSearchFilterUpdating).OnChanged(RefreshSearchFilters);
        }

        private ConcurrentBindableList<Filter> searchFilters;

        public ConcurrentBindableList<Filter> SearchFilters 
        {
            get => searchFilters;
            set => SetProperty(ref searchFilters, value);
        }

        private bool isSelectedSearchFilterUpdating = false;

        public Filter? SelectedSearchFilter
        {
            get => null;
            set
            {
                isSelectedSearchFilterUpdating = true;
                if (!(value is null))
                {
                    SelectedFilters.Add(value);

                    if (!string.IsNullOrEmpty(SearchText))
                    {
                        SearchText = string.Empty;
                    }
                    else
                    {
                        SearchFilters.Remove(value);
                    }
                }
                isSelectedSearchFilterUpdating = false;
            }
        }

        public ConcurrentBindableList<Filter> SelectedFilters { get; }

        public ICommand RemoveSelectedFilterCommand { get; }
        public ICommand RemoveAllSelectedFiltersCommand { get; }

        private readonly ListCollectionView collectionView;

        public FilterHelper(ListCollectionView collectionView)
        {
            searchFilters = new ConcurrentBindableList<Filter>();
            this.collectionView = collectionView;
            collectionView.Filter = CollectionFilter;

            SelectedFilters = new ConcurrentBindableList<Filter>();
            SelectedFilters.CollectionChanged += SelectedFilters_CollectionChanged;

            RemoveSelectedFilterCommand = SimpleCommand.Create<Filter>(OnRemoveSelectedFilter);
            RemoveAllSelectedFiltersCommand = SimpleCommand.Create(_ => SelectedFilters.Clear());

            RefreshSearchFilters();
        }

        private void SelectedFilters_CollectionChanged(object? sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            collectionView.Refresh();
        }

        private void RefreshSearchFilters()
        {
            searchFilters.ReplaceAll(filtersManager.Value.GetFilters(SearchText).Except(SelectedFilters));
        }

        private void OnRemoveSelectedFilter(Filter filter)
        {
            SelectedFilters.Remove(filter);
            RefreshSearchFilters();
        }

        private bool CollectionFilter(object obj)
        {
            if (!(obj is ClipboardObjectViewModel clipboardObjectViewModel))
                return false;

            foreach(var filter in SelectedFilters)
            {
                if (!filter.Passes(clipboardObjectViewModel))
                    return false;
            }

            return true;
        }
    }
}
