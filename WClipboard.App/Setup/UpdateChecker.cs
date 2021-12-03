using Microsoft.Net.Http.Headers;
using Microsoft.Toolkit.Uwp.Notifications;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using WClipboard.App.Settings;
using WClipboard.Core;
using WClipboard.Core.Settings;
using WClipboard.Core.Utilities;
using WClipboard.Core.Utilities.Json;
using WClipboard.Core.WPF.LifeCycle;
using WClipboard.Core.WPF.ViewModels;
using WClipboard.Windows.Notifications;

namespace WClipboard.App.Setup
{
    internal class Release
    {
        public string TagName { get; set; } = string.Empty;
        public string HtmlUrl { get; set; } = string.Empty;
        public bool Prerelease { get; set; }
        public bool Draft { get; set; }

        internal Version GetVersion()
        {
            return Version.Parse(TagName[1..]);
        }
    }

    internal class UpdateChecker : IAfterMainWindowLoadedListener
    {
        private readonly IAppInfo appInfo;
        private readonly INotificationsManager notificationsManager;
        private readonly IIOSetting checkPrereleasesSetting;
        private readonly IIOSetting checkUpdatesOnStartUp;
        private readonly ILogger<UpdateChecker> logger;

        private Release? newestRelease;

        public UpdateChecker(IAppInfo appInfo, INotificationsManager notificationsManager, IIOSettingsManager ioSettingsManager, ILogger<UpdateChecker> logger)
        {
            this.appInfo = appInfo;
            this.notificationsManager = notificationsManager;
            this.logger = logger;

            checkPrereleasesSetting = ioSettingsManager.GetSetting(AppUISettingsFactory.CheckForPrereleases);
            checkUpdatesOnStartUp = ioSettingsManager.GetSetting(AppUISettingsFactory.CheckUpdatesOnStartUp);
        }

        public void AfterMainWindowLoaded(IMainWindowViewModel _)
        {
            if (checkUpdatesOnStartUp.GetValue<bool>())
            {
                Task.Run(CheckForUpdates);
            }
        }

        private async void CheckForUpdates()
        {
            try
            {
                using (var httpClient = new HttpClient())
                {
                    var request = new HttpRequestMessage(HttpMethod.Get, $"https://api.github.com/repos/WClipboard/{appInfo.Name}/releases");
                    request.Headers.Add(HeaderNames.UserAgent, appInfo.Name);
                    var response = await httpClient.SendAsync(request, HttpCompletionOption.ResponseHeadersRead, new CancellationTokenSource(TimeSpan.FromMinutes(1)).Token);
                    response.EnsureSuccessStatusCode();

                    var content = await response.Content.ReadAsStreamAsync();

                    var releases = await JsonSerializer.DeserializeAsync<List<Release>>(content, new JsonSerializerOptions(JsonSerializerDefaults.Web) { PropertyNamingPolicy = new SnakeCaseJsonNamingPolicy() });

                    var checkForPrereleases = checkPrereleasesSetting.GetValue<bool>();
                    newestRelease = releases?.Where(r => !r.Draft && (!r.Prerelease || checkForPrereleases))
                                             .MaxBy(r => r.GetVersion());

                    if (newestRelease != null && newestRelease.GetVersion() > appInfo.Version)
                    {
                        var toast = new ToastContentBuilder()
                            .AddText($"New version of {appInfo.Name} available")
                            .AddText($"v{appInfo.Version} => {newestRelease.TagName}{(newestRelease.Prerelease ? " (prerelease)" : "")}")
                            .AddButton(new ToastButton().SetContent("Download").SetProtocolActivation(new Uri(newestRelease.HtmlUrl)))
                            .SetToastDuration(ToastDuration.Long);
                        await notificationsManager.ShowNotification(toast, (n) =>
                        {
                            n.ExpirationTime = new DateTimeOffset(DateTime.Now).Add(new TimeSpan(0, 10, 0));
                            n.ExpiresOnReboot = true;
                        });
                    }
                }
            }
            catch (UriFormatException ex)
            {
                logger.Log(LogLevel.Critical, "Could not parse newest release html page to uri", ex);
            }
            catch (JsonException ex)
            {
                logger.Log(LogLevel.Warning, "Could not parse github api response to Json", ex);
            }
            catch (HttpRequestException ex)
            {
                logger.Log(LogLevel.Info, "Something went wrong when checking for updates", ex);
            }
        }
    }
}
