using System;
using System.Collections.Generic;
using WClipboard.Core.Clipboard.Trigger;
using WClipboard.Core.Utilities.Collections;
using WClipboard.Core.WPF.Models;

namespace WClipboard.Core.WPF.Managers
{
    public interface IProgramManager
    { 
        Program GetProgram(string path);
        IEnumerable<Program> GetCurrentKnownPrograms();
    }

    public class ProgramManager : IProgramManager
    {
        private readonly KeyedCollectionFunc<string, Program> cache;

        public ProgramManager()
        {
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
    }

    public static class IProgramManagerExtensions
    {
        public static Program GetProgram(this IProgramManager programManager, ProcessInfo processInfo) => programManager.GetProgram(processInfo.Path ?? throw new ArgumentException(nameof(processInfo), $"{nameof(processInfo)}.{nameof(processInfo.Path)} must not be null"));
    }
}
