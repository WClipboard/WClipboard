using System;
using System.Diagnostics;

namespace WClipboard.Core.Extensions
{
    public static class ProcessExtensions
    {
        public static string GetName(this Process process)
        {
            try
            {
                return process.MainModule.FileVersionInfo.FileDescription;
            }
            catch (Exception)
            {
                return process.ProcessName;
            }
        }

        public static string? GetPath(this Process process)
        {
            try
            {
                return process.MainModule.FileName;
            }
            catch (Exception) {
                return null;
            }
        }
    }
}
