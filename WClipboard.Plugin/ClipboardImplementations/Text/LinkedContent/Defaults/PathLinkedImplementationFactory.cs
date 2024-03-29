﻿using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using WClipboard.Core.WPF.Extensions;

#nullable enable

namespace WClipboard.Plugin.ClipboardImplementations.Text.LinkedContent.Defaults
{
    public class PathLinkedImplementationFactory : BaseLinkedTextContentFactory
    {
        private readonly Regex urlLinuxRegex;
        private readonly Regex windowsEscaped;

        public PathLinkedImplementationFactory() : base(new Regex[] { 
            new Regex(@"(?<![\w])(?:file:\/\/(?:localhost)?)?\/([A-Z]([:$])?\/[^\?<>:\\|\*""\s]*)(?![\w])", RegexOptions.Compiled | RegexOptions.IgnoreCase), 
            new Regex(@"""([A-Z]:\\[^\?<>:\/|\*""]*)""|\b[A-Z]:\\[^\?<>:\/|\*\s""]*\b", RegexOptions.Compiled | RegexOptions.IgnoreCase)})
        {
            using (var enumerator = Regexes.GetEnumerator())
            {
                enumerator.MoveNext();
                urlLinuxRegex = enumerator.Current;
                enumerator.MoveNext();
                windowsEscaped = enumerator.Current;
            }
        }

        public override Task<ILinkedTextContent?> Create(TextClipboardImplementation textClipboardImplementation, Regex regex, Match match)
        {
            string? path = null;
            if(ReferenceEquals(urlLinuxRegex, regex))
            {
                path = match.Groups[1].Value;
                if(match.Groups[2].Value == "$")
                {
                    path = path[0..1] + ":" + path[2..];
                } 
                else if(match.Groups[2].Value == "")
                {
                    path = path[0..1] + ":" + path[1..];
                }
                path = path.Replace('/', '\\');
            } 
            else if (ReferenceEquals(windowsEscaped, regex))
            {
                path = match.Groups[1].Success ? match.Groups[1].Value : match.Value;
            }

            if(path is null)
            {
                return Task.FromResult<ILinkedTextContent?>(null);
            }
            else
            {
                var dataObject = new DataObject();
                dataObject.SetFileDropList(path);
                return Task.FromResult<ILinkedTextContent?>(new LinkedTextContent<DataObject>(match.Captures[0], dataObject, "path"));
            }
        }
    }
}
