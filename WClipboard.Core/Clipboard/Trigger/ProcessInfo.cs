using System;
using System.Collections.Generic;
using System.Diagnostics;
using WClipboard.Core.Extensions;

namespace WClipboard.Core.Clipboard.Trigger
{
    public class ProcessInfo : IEquatable<ProcessInfo>, IEquatable<Process>
    {
        public int Id { get; }
        public string? Path { get; }

        public ProcessInfo(Process process)
        {
            if (process == null)
                throw new ArgumentNullException(nameof(process));

            Id = process.Id;
            Path = process.GetPath();
        }

        public ProcessInfo(int id)
        {
            using (var process = Process.GetProcessById(id))
            {
                Id = process.Id;
                Path = process.GetPath();
            }
        }

        public override bool Equals(object obj)
        {
            if (obj is ProcessInfo info)
                return Equals(info);
            else if (obj is Process proc)
                return Equals(proc);
            else
                return base.Equals(obj);
        }

        public bool Equals(ProcessInfo other)
        {
            return other.Id == Id && other.Path == other.Path;
        }

        public bool Equals(Process other)
        {
            return Equals(new ProcessInfo(other));
        }

        public override int GetHashCode()
        {
            int hashCode = 1763280214;
            hashCode = hashCode * -1521134295 + Id.GetHashCode();
            hashCode = hashCode * -1521134295 + EqualityComparer<string?>.Default.GetHashCode(Path);
            return hashCode;
        }

        public static bool operator ==(ProcessInfo first, ProcessInfo second)
        {
            return first.Equals(second);
        }

        public static bool operator !=(ProcessInfo first, ProcessInfo second)
        {
            return !first.Equals(second);
        }

        public static bool operator ==(ProcessInfo first, Process second)
        {
            return first.Equals(second);
        }

        public static bool operator !=(ProcessInfo first, Process second)
        {
            return !first.Equals(second);
        }
    }
}
