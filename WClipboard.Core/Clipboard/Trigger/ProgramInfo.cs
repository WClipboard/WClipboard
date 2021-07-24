using System;
using System.Diagnostics;
using WClipboard.Core.Extensions;

namespace WClipboard.Core.Clipboard.Trigger
{
    public class ProgramInfo : IEquatable<ProgramInfo>, IEquatable<Process>
    {
        public int? ProcessId { get; }
        public string? Path { get; }

        public ProgramInfo(Process process)
        {
            ProcessId = process.Id;
            Path = GetPath(process);
        }

        public ProgramInfo(int id)
        {
            using (var process = Process.GetProcessById(id))
            {
                ProcessId = process.Id;
                Path = GetPath(process);
            }
        }

        private string GetPath(Process process)
        {
            var path = process.GetPath();
            if (path != null)
            {
                path = System.IO.Path.GetFullPath(path);
            }
            return path;
        }

        public ProgramInfo(string path)
        {
            Path = System.IO.Path.GetFullPath(path);
        }

        public override bool Equals(object? obj)
        {
            if (obj is ProgramInfo info)
                return Equals(info);
            else if (obj is Process proc)
                return Equals(proc);
            else
                return base.Equals(obj);
        }

        public bool Equals(ProgramInfo? other)
        {
            return !(other is null) && other.ProcessId == ProcessId && other.Path == other.Path;
        }

        public bool Equals(Process? other)
        {
            return !(other is null) && Equals(new ProgramInfo(other));
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(ProcessId, Path);
        }

        public static bool operator ==(ProgramInfo? first, ProgramInfo? second)
        {
            return first is null ? second is null : first.Equals(second);
        }

        public static bool operator !=(ProgramInfo? first, ProgramInfo? second)
        {
            return first is null ? !(second is null) : !first.Equals(second);
        }

        public static bool operator ==(ProgramInfo? first, Process? second)
        {
            return first is null ? second is null : first.Equals(second);
        }

        public static bool operator !=(ProgramInfo? first, Process? second)
        {
            return first is null ? !(second is null) : !first.Equals(second);
        }
    }
}
