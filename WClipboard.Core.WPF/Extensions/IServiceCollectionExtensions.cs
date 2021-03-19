using Microsoft.Extensions.DependencyInjection;
using WClipboard.Core.WPF.Clipboard.Implementation;
using WClipboard.Core.WPF.Clipboard.Implementation.ViewModel;
using WClipboard.Core.WPF.Clipboard.Metadata;
using WClipboard.Core.WPF.Clipboard.ViewModel.Filters;
using WClipboard.Core.WPF.Models;
using WClipboard.Core.WPF.Settings;
using WClipboard.Core.WPF.Themes;
using WClipboard.Core.WPF.Utilities;

namespace WClipboard.Core.WPF.Extensions
{
    public static class IServiceCollectionExtensions
    {
        public static void AddUISettingsFactory<TFactory>(this IServiceCollection services) where TFactory : BaseUISettingsFactory
        {
            services.AddSingleton<BaseUISettingsFactory, TFactory>();
        }

        public static void AddInteractable<TInteractable>(this IServiceCollection services) where TInteractable : Interactable
        {
            services.AddSingleton<Interactable, TInteractable>();
        }

        public static void AddTheme(this IServiceCollection services, params Theme[] themes)
        {
            foreach (var theme in themes)
            {
                services.AddSingleton(theme);
            }
        }

        public static void AddViewModelFactory<TViewModelFactory>(this IServiceCollection services) where TViewModelFactory : class, IViewModelFactory
        {
            services.AddSingleton<IViewModelFactory, TViewModelFactory>();
        }

        public static void AddClipboardImplementationFactory<TImplementationFactory>(this IServiceCollection services) where TImplementationFactory : ClipboardImplementationFactory
        {
            services.AddSingleton<ClipboardImplementationFactory, TImplementationFactory>();
        }

        public static void AddClipboardImplementationViewModelFactory<TImplementationViewModelFactory>(this IServiceCollection services) where TImplementationViewModelFactory : ClipboardImplementationViewModelFactory
        {
            services.AddSingleton<IViewModelFactory, TImplementationViewModelFactory>();
        }

        public static void AddClipboardObjectMetadataFactory<TMetadataFactory>(this IServiceCollection services) where TMetadataFactory : ClipboardObjectMetadataFactory
        {
            services.AddSingleton<ClipboardObjectMetadataFactory, TMetadataFactory>();
        }

        public static void AddFiltersProvider<TFiltersProvider>(this IServiceCollection services) where TFiltersProvider : class, IFiltersProvider
        {
            services.AddSingleton<IFiltersProvider, TFiltersProvider>();
        }
    }
}
