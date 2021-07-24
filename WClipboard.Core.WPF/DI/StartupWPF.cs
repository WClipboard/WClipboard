using Microsoft.Extensions.DependencyInjection;
using WClipboard.Core.DI;
using WClipboard.Core.WPF.LifeCycle;
using WClipboard.Core.WPF.Defaults;
using WClipboard.Core.WPF.Defaults.Interactables;
using WClipboard.Core.WPF.Managers;
using WClipboard.Core.WPF.Utilities;
using WClipboard.Core.WPF.ViewModels;
using WClipboard.Core.WPF.Extensions;
using WClipboard.Core.WPF.Settings;
using WClipboard.Core.WPF.Clipboard;
using WClipboard.Core.Settings.Defaults;
using WClipboard.Core.WPF.Themes;
using WClipboard.Core.WPF.Settings.Local;
using WClipboard.Core.WPF.Clipboard.Metadata.Defaults;
using WClipboard.Core.WPF.Settings.Defaults;
using WClipboard.Core.WPF.Clipboard.ViewModel.Filters;
using WClipboard.Core.WPF.Clipboard.ViewModel.Filters.Defaults;
using WClipboard.Core.WPF.Clipboard.Filter;
using WClipboard.Core.WPF.Clipboard.Format;

namespace WClipboard.Core.WPF.DI
{
    public sealed class StartupWpf : IStartup, IAfterWPFAppStartupListener
    {
        void IStartup.ConfigureServices(IServiceCollection services, IStartupContext context)
        {
            context.IOSettingsManager.AddSettings(new KeyedCollectionSetting<string, Theme, IThemesManager>(SettingConsts.ThemeKey, SettingConsts.ThemeDefaultName));
            context.IOSettingsManager.AddSettings(new ListSetting<string>(SettingConsts.OwnerProgramClipboardFilterKey));
            context.IOSettingsManager.AddSettings(new BasicSetting<bool>(SettingConsts.Windows10HistoryFilterKey, () => true));
            context.IOSettingsManager.AddSettings(new BasicSetting<bool>(SettingConsts.Windows10CloudFilterKey, () => false));

            services.AddSingleton<IViewModelFactoriesManager, ViewModelFactoriesManager>();

            services.AddSingleton<IImageDownloader, ImageDownloader>();

            services.AddSingleton<IClipboardObjectManager, ClipboardObjectManager>();
            services.AddSingleton<IClipboardObjectsManager, ClipboardObjectsManager>();
            services.AddSingleton<ITypeDataTemplateManager, TypeDataTemplateManager>();
            services.AddSingleton<IProgramManager, ProgramManager>();
            services.AddSingleton<IFiltersManager, FiltersManager>();

            services.AddSingleton<IUISettingsManager, UISettingsManager>();
            services.AddSingleton<IThemesManager, ThemesManager>();

            services.AddSingleton<IInteractablesManager, InteractablesManager>();

            services.AddInteractable<CopyClipboardObjectInteractable>();
            services.AddInteractable<CopyFormatInteractable>();

            services.AddSingleton<IGlobalKeyEventsManager, GlobalKeyEventsManager>();
            services.AddSingletonWithAutoInject<ClipboardKeyListener>();

            services.AddTransientWithAutoInject<CreateTriggerAfterMainWindowLoaded>();

            services.AddUISettingsFactory<WPFUISettingsFactory>();

            services.AddClipboardObjectMetadataFactory<DefaultClipboardObjectMetadataFactory>();
            services.AddFiltersProvider<FormatFiltersProvider>();
            services.AddFiltersProvider<ProgramFiltersProvider>();

            services.AddSingletonWithAutoInject<ClipboardViewerListener>();

            services.AddClipboardFilter<OwnerProgramClipboardFilter>();
            services.AddClipboardFilter<Windows10ClipboardFilter>();

            services.AddFormatsExtractor<Windows10FormatsExtractor>();
        }

        void IAfterWPFAppStartupListener.AfterWPFAppStartup()
        {
            var serviceProvider = DiContainer.SP!;

            serviceProvider.AddTypeDateTemplate<InteractableState>("Views/InteractableView.xaml");
            serviceProvider.AddTypeDateTemplate<ToggleableInteractableState>("Views/InteractableToggleView.xaml");

            serviceProvider.AddTypeDateTemplate<TextSettingViewModel>("Settings/Defaults/DefaultSettingViews.xaml");
            serviceProvider.AddTypeDateTemplate<CheckBoxSettingViewModel>("Settings/Defaults/DefaultSettingViews.xaml");
            serviceProvider.AddTypeDateTemplate<ComboBoxSettingViewModel>("Settings/Defaults/DefaultSettingViews.xaml");

            serviceProvider.AddTypeDateTemplate<ProgramFilterSettingViewModel>("Settings/Local/LocalSettingViews.xaml");

            serviceProvider.AddTypeDateTemplate<Theme>("Views/ComboBoxViews.xaml");

            serviceProvider.AddTypeDateTemplate<MessageBarViewModel>("Views/MessageBarView.xaml");

            serviceProvider.AddTypeDateTemplate<FormatsMetadata>("Clipboard/Metadata/MetadataViews.xaml");
            serviceProvider.AddTypeDateTemplate<TriggersMetadata>("Clipboard/Metadata/MetadataViews.xaml");
        }
    }
}
