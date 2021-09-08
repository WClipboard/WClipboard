using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using WClipboard.Core.Clipboard.Trigger;
using WClipboard.Core.Utilities.Collections;
using WClipboard.Core.WPF.Models;
using WClipboard.Windows;
using ShellLinkPlus;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using WClipboard.Core.Utilities;

namespace WClipboard.Core.WPF.Managers
{
    public interface IProgramManager
    { 
        Program GetProgram(string path);
        IEnumerable<Program> GetCurrentKnownPrograms();
        Task ScanStartMenu();
    }

    public class ProgramManager : IProgramManager
    {
        private readonly KeyedCollectionFunc<string, Program> cache;
        private readonly ILogger<ProgramManager> logger;

        public ProgramManager(ILogger<ProgramManager> logger)
        {
            this.logger = logger;
            cache = new KeyedCollectionFunc<string, Program>(p => p.Path!);
        }

        public IEnumerable<Program> GetCurrentKnownPrograms() => cache;

        public Program GetProgram(string path)
        {
            if (!cache.TryGetValue(path, out var program))
            {
                program = new Program(path);
                cache.Add(program);
            }
            return program;
        }

        public Task ScanStartMenu()
        {
            return Task.Run(() =>
            {
                var userStartMenu = KnownFoldersHelper.GetPath(KnownFolder.Programs);
                var allStartMenu = KnownFoldersHelper.GetPath(KnownFolder.CommonPrograms);

                foreach (var file in Directory.EnumerateFiles(userStartMenu, "*.lnk", SearchOption.AllDirectories).Concat(Directory.EnumerateFiles(allStartMenu, "*.lnk", SearchOption.AllDirectories)))
                {
                    if (Path.GetFileName(file).Contains("uninstall", StringComparison.InvariantCultureIgnoreCase))
                        continue;

                    try
                    {
                        using (var shellLink = new ShellLink(file))
                        {
                            var targetPath = shellLink.TargetPath;

                            if (!targetPath.EndsWith(".exe", StringComparison.InvariantCultureIgnoreCase))
                                continue;

                            if (Path.GetFileName(targetPath).Contains("uninstall", StringComparison.InvariantCultureIgnoreCase))
                                continue;

                            if (!File.Exists(targetPath))
                                continue;

                            GetProgram(targetPath);
                        }
                    }
                    catch(COMException ex)
                    {
                        logger.Log(LogLevel.Info, $"Could not read shortcut {file}", ex);
                    }
                }
            });
        }
    }

    public static class IProgramManagerExtensions
    {
        public static Program GetProgram(this IProgramManager programManager, ProgramInfo processInfo) => programManager.GetProgram(processInfo.Path ?? throw new ArgumentException($"{nameof(processInfo)}.{nameof(processInfo.Path)} must not be null", nameof(processInfo)));
    }
}
