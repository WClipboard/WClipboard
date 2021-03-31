using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows;
using WClipboard.Core;
using WClipboard.Core.Extensions;
using WClipboard.Core.Utilities.Json;
using WClipboard.Core.WPF.LifeCycle;

namespace WClipboard.App.Setup
{
    internal class Release
    {
        public string? TagName { get; set; }
        public string? HtmlUrl { get; set; }
        public bool Prerelease { get; set; }
        public bool Draft { get; set; }

        internal Version GetVersion()
        {
            return Version.Parse((TagName ?? throw new NullReferenceException($"{nameof(TagName)} is null, cannot find version"))[1..]);
        }
    }

    internal class UpdateChecker : IAfterMainWindowLoadedListener
    {
        private readonly IAppInfo appInfo;

        public UpdateChecker(IAppInfo appInfo)
        {
            this.appInfo = appInfo;
        }

        public void AfterMainWindowLoaded()
        {
            Task.Run(CheckForUpdates);
        }

        private async void CheckForUpdates()
        {
            using (var webclient = new WebClient())
            {
                webclient.Headers.Add(HttpRequestHeader.UserAgent, appInfo.Name);
                var releases = await JsonSerializer.DeserializeAsync<List<Release>>(await webclient.OpenReadTaskAsync($"https://api.github.com/repos/WClipboard/{appInfo.Name}/releases"), new JsonSerializerOptions(JsonSerializerDefaults.Web) { PropertyNamingPolicy =  new SnakeCaseJsonNamingPolicy()});

                var newestRelease = releases?.Where(r => !r.Draft).MaxBy(r => r.GetVersion());

                if (newestRelease != null && newestRelease.GetVersion() > appInfo.Version)
                {
                    if(MessageBox.Show($"New version of {appInfo.Name} available v{appInfo.Version} => {newestRelease.TagName}{(newestRelease.Prerelease ? " (prerelease)" : "")}.\nWould you like to download the update?", "Update available", MessageBoxButton.YesNo, MessageBoxImage.Information) == MessageBoxResult.Yes)
                    {
                        Process.Start(new ProcessStartInfo(newestRelease.HtmlUrl!) {
                            UseShellExecute = true,
                            Verb = "open"
                        });
                    }
                }
            }
        }
    }
}
