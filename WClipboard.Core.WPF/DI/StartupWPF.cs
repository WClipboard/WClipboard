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
using WClipboard.Core.WPF.Clipboard.Implementation.LinkedContent;
using WClipboard.Core.WPF.Clipboard.Metadata.Defaults;
using WClipboard.Core.WPF.Settings.Defaults;
using WClipboard.Core.WPF.Clipboard.ViewModel.Filters;
using WClipboard.Core.WPF.Clipboard.ViewModel.Filters.Defaults;

namespace WClipboard.Core.WPF.DI
{
    public sealed class StartupWpf : IStartup, IAfterWPFAppStartupListener
    {
        void IStartup.ConfigureServices(IServiceCollection services, IStartupContext context)
        {
            context.IOSettingsManager.AddSettings(new KeyedCollectionSetting<string, Theme, IThemesManager>(SettingConsts.ThemeKey, SettingConsts.ThemeDefaultName));


            services.AddSingleton<IImageDownloader, ImageDownloader>();

            services.AddSingleton<IClipboardObjectManager, ClipboardObjectManager>();
            services.AddSingleton<IClipboardObjectsManager, ClipboardObjectsManager>();
            services.AddSingleton<ITypeDataTemplateManager, TypeDataTemplateManager>();
            services.AddSingleton<IProgramManager, ProgramManager>();
            services.AddSingleton<IFiltersManager, FiltersManager>();

            services.AddSingleton<ILinkedContentFactoriesManagersManager, LinkedContentFactoriesManagersManager>();

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
        }

        void IAfterWPFAppStartupListener.AfterWPFAppStartup()
        {
            var serviceProvider = DiContainer.SP!;

            serviceProvider.AddTypeDateTemplate<InteractableState>("Views/InteractableView.xaml");
            serviceProvider.AddTypeDateTemplate<ToggleableInteractableState>("Views/InteractableToggleView.xaml");

            serviceProvider.AddTypeDateTemplate<TextSettingViewModel>("Views/SettingViews.xaml");
            serviceProvider.AddTypeDateTemplate<CheckBoxSettingViewModel>("Views/SettingViews.xaml");
            serviceProvider.AddTypeDateTemplate<ComboBoxSettingViewModel>("Views/SettingViews.xaml");

            serviceProvider.AddTypeDateTemplate<Theme>("Views/ComboBoxViews.xaml");

            serviceProvider.AddTypeDateTemplate<MessageBarViewModel>("Views/MessageBarView.xaml");

            serviceProvider.AddTypeDateTemplate<FormatsMetadata>("Views/MetadataViews.xaml");
            serviceProvider.AddTypeDateTemplate<TriggersMetadata>("Views/MetadataViews.xaml");
        }
    }
}
