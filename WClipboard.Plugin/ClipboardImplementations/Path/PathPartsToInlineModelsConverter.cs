using System;
using System.Collections.Generic;
using System.Globalization;
using WClipboard.Core.WPF.Converters;
using WClipboard.Core.WPF.Models.Text;
using WClipboard.Core.WPF.Themes;

namespace WClipboard.Plugin.ClipboardImplementations.Path
{
    public class PathPartsToInlineModelsConverter : BaseConverter<IEnumerable<PathPart>, IEnumerable<InlineModel>>
    {
        public override IEnumerable<InlineModel> Convert(IEnumerable<PathPart> value, Type targetType, object parameter, CultureInfo culture)
        {
            bool isFirst = true;
            foreach (var pathPart in value)
            {
                if (isFirst)
                    isFirst = false;
                else
                    yield return new RunModel() { Text = "\\", Foreground = new FromPalette() };

                var hyperlink = new HyperlinkModel() { Command = PathPart.OpenCommand, CommandParameter = pathPart, ToolTip = $"Open {pathPart.FullPath}", Foreground = new FromPalette() };
                hyperlink.Inlines.Add(new RunModel() { Text = pathPart.Name });
                yield return hyperlink;
            }
        }
    }
}
