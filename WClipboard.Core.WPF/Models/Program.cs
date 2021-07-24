using System;
using System.Collections.Generic;
using System.Diagnostics;
using WClipboard.Core.Extensions;
using WClipboard.Windows.Helpers;

namespace WClipboard.Core.WPF.Models
{
    public class Program : IEquatable<Program>
    {
        public string Name { get; }
        public string? Path { get; }
        public object? IconSource { get; }

        public Program(Process process)
        {
            Name = process.GetName();
            Path = process.GetPath();
            if(!(Path is null))
            {
                Path = System.IO.Path.GetFullPath(Path);
                IconSource = PathInfoHelper.GetIcon(Path, IconType.Small);
            }
        }

        public Program(string path)
        {
            path = System.IO.Path.GetFullPath(path);
            Path = path;
            IconSource = PathInfoHelper.GetIcon(Path, IconType.Small);
            Name = FileVersionInfo.GetVersionInfo(path).FileDescription ?? "";
            if (string.IsNullOrWhiteSpace(Name))
            {
                Name = System.IO.Path.GetFileNameWithoutExtension(path);
            }
        }

        public override bool Equals(object? obj)
        {
            return Equals(obj as Program);
        }

        public bool Equals(Program? other)
        {
            return !(other is null) && string.Equals(Path, other?.Path, StringComparison.OrdinalIgnoreCase);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Path);
        }

        public static bool operator ==(Program? left, Program? right)
        {
            return EqualityComparer<Program>.Default.Equals(left, right);
        }

        public static bool operator !=(Program? left, Program? right)
        {
            return !(left == right);
        }
    }
}
