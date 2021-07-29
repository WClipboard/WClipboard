using System.Collections.Generic;
using System.Text.RegularExpressions;
using WClipboard.Core.Utilities.Collections;
using WClipboard.Core.WPF.Managers;
using WClipboard.Core.WPF.Models;

#nullable enable

namespace WClipboard.Core.WPF.Clipboard.ViewModel.Filters.Defaults
{
    public class ProgramFiltersProvider : IFiltersProvider
    {
        private readonly IProgramManager programManager;
        private readonly KeyedCollectionFunc<Program, ProgramFilter> filterCache;

        public ProgramFiltersProvider(IProgramManager programManager)
        {
            this.programManager = programManager;
            filterCache = new KeyedCollectionFunc<Program, ProgramFilter>(pf => pf.Program);
        }

        public IEnumerable<Filter> GetFilters(string text)
        {
            if (!string.IsNullOrEmpty(text)) {
                var programs = programManager.GetCurrentKnownPrograms();

                foreach (var program in programs)
                {
                    if (Regex.IsMatch(program.Name, $@"^(.*\s)?{text}", RegexOptions.IgnoreCase | RegexOptions.CultureInvariant))
                    {
                        if (!filterCache.TryGetValue(program, out var filter)) {
                            filter = new ProgramFilter(program);
                            filterCache.Add(filter);
                        }
                        yield return filter;
                    }
                }
            }
        }

        private class ProgramFilter : Filter
        {
            public Program Program { get; }

            public ProgramFilter(Program program) : base(program.Name, program.IconSource)
            {
                Program = program;
            }

            public override bool Passes(ClipboardObjectViewModel clipboardObjectViewModel)
            {
                return clipboardObjectViewModel.MainTrigger?.DataSourceProgram == Program || clipboardObjectViewModel.MainTrigger?.ForegroundProgram == Program;
            }
        }
    }
}
