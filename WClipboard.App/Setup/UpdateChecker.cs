﻿using Microsoft.Toolkit.Uwp.Notifications;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text.Json;
using System.Threading.Tasks;
using WClipboard.Core;
using WClipboard.Core.Extensions;
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
        private readonly ILogger<UpdateChecker> logger;

        private Release? newestRelease;

        public UpdateChecker(IAppInfo appInfo, INotificationsManager notificationsManager, ILogger<UpdateChecker> logger)
        {
            this.appInfo = appInfo;
            this.notificationsManager = notificationsManager;
            this.logger = logger;
        }

        public void AfterMainWindowLoaded(IMainWindowViewModel _)
        {
            Task.Run(CheckForUpdates);
        }

        private async void CheckForUpdates()
        {
            try
            {
                using (var webclient = new WebClient())
                {
                    webclient.Headers.Add(HttpRequestHeader.UserAgent, appInfo.Name);
                    var releases = await JsonSerializer.DeserializeAsync<List<Release>>(await webclient.OpenReadTaskAsync($"https://api.github.com/repos/WClipboard/{appInfo.Name}/releases"), new JsonSerializerOptions(JsonSerializerDefaults.Web) { PropertyNamingPolicy = new SnakeCaseJsonNamingPolicy() });

                    newestRelease = releases?.Where(r => !r.Draft).MaxBy(r => r.GetVersion());

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
                logger.Log(LogLevel.Critical, "Could not parse newest release html page to uri");
                logger.Log(LogLevel.Critical, ex);
            }
            catch (JsonException ex)
            {
                logger.Log(LogLevel.Warning, "Could not parse github api response to Json");
                logger.Log(LogLevel.Warning, ex);
            }
            catch (WebException ex)
            {
                logger.Log(LogLevel.Info, "Something went wrong when checking for updates");
                logger.Log(LogLevel.Info, ex);
            }
        }
    }
}
