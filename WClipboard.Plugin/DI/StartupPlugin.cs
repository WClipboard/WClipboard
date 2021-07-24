using Microsoft.Extensions.DependencyInjection;
using System.Windows.Media.Imaging;
using WClipboard.Core.DI;
using WClipboard.Core.Settings.Defaults;
using WClipboard.Core.WPF.Extensions;
using WClipboard.Core.WPF.LifeCycle;
using WClipboard.Core.WPF.Managers;
using WClipboard.Plugin.ClipboardImplementations.Bitmap;
using WClipboard.Plugin.ClipboardImplementations.Path;
using WClipboard.Plugin.ClipboardImplementations.Text;
using WClipboard.Plugin.ClipboardImplementations.Text.Filters;
using WClipboard.Plugin.ClipboardImplementations.Text.LinkedContent;
using WClipboard.Plugin.ClipboardImplementations.Text.LinkedContent.Defaults;
using WClipboard.Plugin.Defaults;
using WClipboard.Plugin.Pinned;
using WClipboard.Plugin.Settings;

namespace WClipboard.Plugin.DI
{
    public sealed class StartupPlugin : IStartup, IAfterWPFAppStartupListener
    {
        void IStartup.ConfigureServices(IServiceCollection services, IStartupContext context)
        {
            DefaultClipboardFormatCategories.Setup(services);
            DefaultClipboardFormats.Setup(services);

            services.AddFormatsExtractor<DefaultFormatsExtractor>();
            services.AddClipboardImplementationFactory<DefaultClipboardImplementationFactory>();
            services.AddClipboardImplementationViewModelFactory<DefaultClipboardImplementationViewModelFactory>();

            //Bitmap
            services.AddSingleton(BitmapFileOption.Create<BmpBitmapEncoder>());
            services.AddSingleton(BitmapFileOption.Create<GifBitmapEncoder>());
            services.AddSingleton(BitmapFileOption.Create<PngBitmapEncoder>());
            services.AddSingleton(BitmapFileOption.Create<JpegBitmapEncoder>(extension: ".jpg"));
            services.AddSingleton(BitmapFileOption.Create<TiffBitmapEncoder>());
            services.AddSingleton(BitmapFileOption.Create<WmpBitmapEncoder>());

            services.AddSingleton<IBitmapFileOptionsManager, BitmapFileOptionsManager>();

            context.IOSettingsManager.AddSettings(new KeyedCollectionSetting<string, BitmapFileOption, IBitmapFileOptionsManager>(SettingConsts.ToFileBitmapEncoderKey, SettingConsts.DefaultToFileBitmapEncoder));

            services.AddInteractable<ClipboardImplementations.Bitmap.Interactables.ConvertToFileInteractable>();

            //Path
            services.AddSingleton<ILinkedTextContentFactory, PathLinkedImplementationFactory>();

            //Text
            services.AddSingleton<LinkedTextContentFactoriesManager>();
            services.AddInteractable<ClipboardImplementations.Text.Interactables.ConvertToFileInteractable>();
            services.AddFiltersProvider<TextSearchFilterProvider>();

            //Settings
            services.AddUISettingsFactory<PluginUISettingsFactory>();

            //Extensions
            services.AddInteractable<PinnedInteractable>();
            services.AddSingletonWithAutoInject<PinnedManager>();
            services.AddFiltersProvider<PinnedFilterProvider>();

            //Pluggins
            var pluginManager = new PluginManager(context);
            services.AddSingleton<IPluginManager>(pluginManager);
            pluginManager.ConfigureServices(services, context);
        }

        void IAfterWPFAppStartupListener.AfterWPFAppStartup()
        {
            var serviceProvider = DiContainer.SP;

            serviceProvider.AddTypeDateTemplate<TextClipboardImplementationViewModel>("ClipboardImplementations/Text/TextClipboardImplementationView.xaml");
            serviceProvider.AddTypeDateTemplate<LinkedTextClipboardImplementationViewModel>("ClipboardImplementations/Text/TextClipboardImplementationView.xaml");
            serviceProvider.AddTypeDateTemplate<MultiPathsViewModel>("ClipboardImplementations/Path/MultiPathsView.xaml");
            serviceProvider.AddTypeDateTemplate<SinglePathViewModel>("ClipboardImplementations/Path/SinglePathView.xaml");
            serviceProvider.AddTypeDateTemplate<BitmapImplementationViewModel>("ClipboardImplementations/Bitmap/BitmapImplementationView.xaml");

            serviceProvider.AddTypeDateTemplate<BitmapFileOption>("Settings/SettingViews.xaml");
        }
    }
}
